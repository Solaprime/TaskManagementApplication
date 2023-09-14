using AppShared.ConfigurationDto;
using AppShared.Models;
using AppShared.Response;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using TaskDomain.Entities;
using TaskPersistence.TasksManagementIdentity.Contract;

namespace TaskPersistence.TasksManagementIdentity.Services
{
    public class TokenService : ITokenService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly PersistentDBContext _context;
        //    private readonly IConfiguration _configuration;
        public readonly JwtConfig _jwtConfig;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly TokenValidationParameters _tokenValidationParams;
       // IConfiguration configuration,
        public TokenService(UserManager<ApplicationUser> userManager, PersistentDBContext context,
          RoleManager<IdentityRole> roleManager,
           TokenValidationParameters tokenValidationParams, IOptions<JwtConfig> jwtConfig)
        {
            _userManager = userManager;
          //  _configuration = configuration;
            _roleManager = roleManager;
            _context = context;
            _jwtConfig = jwtConfig.Value;

        }
        public async Task<UserManagerResponse> GenerateJwtToken(ApplicationUser user)
        {
            var jwtTokenHandler = new JwtSecurityTokenHandler();
           // var key = Encoding.UTF8.GetBytes(_configuration.GetSection("JwtConfig:Secret").Value);
            var key = Encoding.UTF8.GetBytes(_jwtConfig.Secret);
            var claims = await GetAllValidClaims(user);
            var tokenDescriptor = new SecurityTokenDescriptor()
            {
                Subject = new ClaimsIdentity(claims),
                //Put these one In AppSettings.json
               // Expires = DateTime.Now.AddMinutes(3),    //AddSeconds(20) for test sake
                Expires = DateTime.Now.AddMinutes(_jwtConfig.TokenExpiry),    //AddSeconds(20) for test sake
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key),
                SecurityAlgorithms.HmacSha256Signature),
            };
            var token = jwtTokenHandler.CreateToken(tokenDescriptor);
            var jwtToken = jwtTokenHandler.WriteToken(token);
            var refreshToken = new RefreshToken()
            {
                JwtId = token.Id,
                IsUsed = false,
                IsRevorked = false,
                UserId = user.Id,
                AddedDate = DateTime.UtcNow,
                ExpiryDate = DateTime.UtcNow.AddMonths(6),
                Token = RandomString(35) + Guid.NewGuid()
            };
            await _context.RefreshTokens.AddAsync(refreshToken);
            await _context.SaveChangesAsync();
            return new UserManagerResponse()
            {
                Token = jwtToken,
                Success = true,
                RefreshToken = refreshToken.Token
            };

        }

        public  async Task<UserManagerResponse> VerifyAndGenerateToken(TokenRequest tokenRequest)
        {
            var jwtTokenHandler = new JwtSecurityTokenHandler();
            try
            {
                _tokenValidationParams.ValidateLifetime = false;
                var tokenInVerification = jwtTokenHandler.ValidateToken(tokenRequest.Token, _tokenValidationParams, out var validatedToken);
                _tokenValidationParams.ValidateLifetime = true;
                if (validatedToken is JwtSecurityToken jwtSecurityToken)
                {
                    var result = jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase);
                    if (result == false)
                    {
                        return null;
                    }
                }
                var utcExpiryDate = long.Parse(tokenInVerification.Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Exp).Value);
                var expiryDate = UnixTimeStampToDateTime(utcExpiryDate);
                if (expiryDate > DateTime.UtcNow)
                {
                    return new UserManagerResponse()
                    {
                        Success = false,
                        Error = new List<string>() {
                            "Token has not yet expired"
                        }
                    };
                } 
                var storedToken = await _context.RefreshTokens.FirstOrDefaultAsync(x => x.Token == tokenRequest.RefreshToken);
                if (storedToken == null)
                {
                    return new UserManagerResponse()
                    {
                        Success = false,
                        Error = new List<string>() {
                            "Token does not exist"
                        }
                    };
                }
                if (storedToken.IsUsed)
                {
                    return new UserManagerResponse()
                    {
                        Success = false,
                        Error = new List<string>() {
                            "Token has been used"
                        }
                    };
                }
                if (storedToken.IsRevorked)
                {
                    return new UserManagerResponse()
                    {
                        Success = false,
                        Error = new List<string>() {
                            "Token has been revoked"
                        }
                    };
                }
                var jti = tokenInVerification.Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Jti).Value;
                if (storedToken.JwtId != jti)
                {
                    return new UserManagerResponse()
                    {
                        Success = false,
                        Error = new List<string>() {
                            "Token doesn't match"
                        }
                    };
                }
                if (storedToken.ExpiryDate < DateTime.UtcNow)
                {
                    return new UserManagerResponse()
                    {
                        Success = false,
                        Error = new List<string>() {
                            "Refresh token has expired"
                        }
                    };
                }
                storedToken.IsUsed = true;
                _context.RefreshTokens.Update(storedToken);
                await _context.SaveChangesAsync();
                var dbUser = await _userManager.FindByIdAsync(storedToken.UserId);
                return await GenerateJwtToken(dbUser);
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("Lifetime validation failed. The token is expired."))
                {
                    return new UserManagerResponse()
                    {
                        Success = false,
                        Error = new List<string>() {
                            "Token has expired please re-login"
                        }
                    };

                }
                else
                {
                    return new UserManagerResponse()
                    {
                        Success = false,
                        Error = new List<string>() {
                            "Something went wrong."
                        }
                    };
                }
            }

        }

    
     private async Task<List<Claim>> GetAllValidClaims(ApplicationUser user)
        {
            var claims = new List<Claim>
            {
                new Claim("Id", user.Id),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(JwtRegisteredClaimNames.Sub, user.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            };
            var userClaims = await _userManager.GetClaimsAsync(user);
            claims.AddRange(userClaims); 
            var userRoles = await _userManager.GetRolesAsync(user);
            foreach (var userRole in userRoles)
            { 
                var role = await _roleManager.FindByNameAsync(userRole);
                if (role != null)
                { 
                    claims.Add(new Claim(ClaimTypes.Role, userRole));
                    var roleClaims = await _roleManager.GetClaimsAsync(role);
                    foreach (var roleClaim in roleClaims)
                    {
                        claims.Add(roleClaim);
                    }
                }
            }

            return claims;
        }
        private string RandomString(int length)
        {
            var random = new Random();
          //  var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var chars = _jwtConfig.CharactersConfig;
            return new string(Enumerable.Repeat(chars, length)
                .Select(x => x[random.Next(x.Length)]).ToArray());
        }
        //Wat is these Method Doing 
        private DateTime UnixTimeStampToDateTime(long unixTimeStamp)
        {
            var dateTimeVal = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            dateTimeVal = dateTimeVal.AddSeconds(unixTimeStamp).ToUniversalTime();

            return dateTimeVal;
        }


    }
}
