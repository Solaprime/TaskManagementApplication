using AppShared.Models;
using AppShared.Response;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskDomain.Entities;
using TaskPersistence.TasksManagementIdentity.Contract;

namespace TaskPersistence.TasksManagementIdentity.Services
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly ILogger<AuthenticationService> _logger;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ITokenService _tokenService;
        private readonly RoleManager<IdentityRole> _roleManager;



        public AuthenticationService(ILogger<AuthenticationService> logger, UserManager<ApplicationUser> userManager,
            ITokenService tokenService, RoleManager<IdentityRole> roleManager)
        {
            _logger = logger;
            _userManager = userManager;
            _tokenService = tokenService;
            _roleManager = roleManager;

        }
        public Task<BaseResponse> AddUserToRole(RoleEmail roleEmail)
        {
            throw new NotImplementedException();
        }

        public async Task<UserManagerResponse> ForgetPasswordAsync(string email)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(email);
                if (user == null)
                {
                    _logger.LogInformation($"User with{email} cannot be Found");
                    return new UserManagerResponse
                    {
                        Success = false,
                        Message = "No user with this email exist"
                    };
                }
                var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                // this token mught contain some special charcters like forward slash, --- dat might not load well or be rejected on browser
                // so we need to encode it
                var encodedEmailToken = Encoding.UTF8.GetBytes(token);
                var validToken = WebEncoders.Base64UrlEncode(encodedEmailToken);
                _logger.LogInformation("Token Generated successfully");
                return new UserManagerResponse
                {
                    Success = true,
                    // Message = "Reset Password Url has been sent to YOU succesfully"
                    Message = validToken,
                    Id = user.Id
                };
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error occured at method <<ForgetPasswordAsync>>  Error Message : {ex.Message}");
                throw;
            }
        }

        public async Task<UserManagerResponse> LoginUserAsync(LoginViewModel model)
        {
            try
            {
                _logger.LogInformation($"User with{model.Email} is about to login");
                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user == null)
                {
                    _logger.LogInformation($"User can not be Found {model.Email}");
                    return new UserManagerResponse
                    {
                        Message = "No user with this Email exist in our Databse",
                        Success = false
                    };
                }
                var result = await _userManager.CheckPasswordAsync(user, model.PassWord);
                if (!result)
                {
                    _logger.LogInformation("Passwors is not In coorect Formatc");
                    return new UserManagerResponse
                    {
                        Message = "Invalid Password ",
                        Success = false
                    };
                }

                var jwtTokenResponse = await _tokenService.GenerateJwtToken(user);
                _logger.LogInformation($"User with  {model.Email}  Loged in Succesfully");

                //  var request = new EmailRequest() { To = model.Email, Subject = "Login Notification", Body = "A login to ur account was observed" };
                //   var jobId = BackgroundJob.Enqueue(() => _emailService.SendEmail(request));
                //LOg to signify these JobId
                //  await _emailService.SendEmail(request);
                //lOg the JobID 
                return jwtTokenResponse;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error occured when {model.Email} was about to login  Error Message : {ex.Message}");
                return new UserManagerResponse
                {
                    Message = $"an exception occured =>>>>>>>> {ex.ToString()}",
                    Success = false
                };
            }
        }

        public async Task<UserManagerResponse> RegisterUserAsync(RegisterViewModel model)
        {
            try
            {
                if (model == null)
                {
                    throw new NullReferenceException("Our registerModel is null");
                }
                var user_Exist = await _userManager.FindByEmailAsync(model.Email);
                if (user_Exist != null)
                {
                    return new UserManagerResponse
                    {
                        Message = "This user, with this Email  already exist",
                        Success = false,
                    };
                }
                var identityUser = new ApplicationUser()
                {
                    Email = model.Email,
                    UserName = model.Email,
                };
                var result = await _userManager.CreateAsync(identityUser, model.PassWord);
                if (result.Succeeded)
                {
                    _logger.LogInformation($"User Created Succefully {model.Email}");
                    await _userManager.AddToRoleAsync(identityUser, "AppUser");
                    //  var jwtTokenResponse = await GenerateJwtToken(identityUser);
                    var jwtTokenResponse = await _tokenService.GenerateJwtToken(identityUser);
                    //return new UserManagerResponse
                    //{
                    //    Message = "User Created Succesfully",
                    //    IsSuccess = true,
                    //    Token = jwtToken,
                    //};
                    return jwtTokenResponse;
                }
                _logger.LogInformation($"somehting bad Happend {result.Errors.Select(e => e.Description)}");
                return new UserManagerResponse
                {
                    Message = "Unable to create user, Kindly try again",
                    Success = false,
                    Error = result.Errors.Select(e => e.Description),


                };
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error occured when {model.Email} was about to register  Error Message : {ex.Message}");
                return new UserManagerResponse
                {
                    Message = "Something went wrong , please try again later",
                    Success = false,
                };
            }
        }

        public async Task<UserManagerResponse> ResetPasswordAsync(ResetPasswordViewModel model)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user == null)
                {
                    _logger.LogInformation("User Can Not be Found ");
                    return new UserManagerResponse
                    {
                        Success = false,
                        Message = "User can not be found",
                    };
                }
                var decodedToken = WebEncoders.Base64UrlDecode(model.Token);
                string normalToken = Encoding.UTF8.GetString(decodedToken);
                var result = await _userManager.ResetPasswordAsync(user, normalToken, model.Password);
                if (result.Succeeded)
                {
                    _logger.LogInformation($"Password Resetted Succesfully ");
                    return new UserManagerResponse
                    {
                        Success = true,
                        Message = "Password has been Resetted succesfully"
                    };
                }
                _logger.LogInformation($"somehting bad Happend {result.Errors.Select(e => e.Description)}");
                return new UserManagerResponse
                {
                    Success = false,
                    Message = "Somwthing went wrong Kindly try again later",
                    Error = result.Errors.Select(e => e.Description)
                };
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error occured when {model.Email} was about to Reset Password  Error Message : {ex.Message}");
                return new UserManagerResponse
                {
                    Message = $"an exception occured =>>>>>>>> {ex.ToString()}",
                    Success = false
                };
            }

        }


        public async Task<UserManagerResponse> CreateRole(string rolename)
        {
            try
            {
                var roleExist = await _roleManager.RoleExistsAsync(rolename);
                // If Role Does not exist ,
                if (!roleExist)
                {

                    // Create the Role 
                    var roleResult = await _roleManager.CreateAsync(new IdentityRole(rolename));
                    if (roleResult.Succeeded)
                    {
                        _logger.LogInformation($"Role was creatd Succesffuly {rolename}");
                        return new UserManagerResponse
                        {
                            Message = $"The role {rolename} has been Created succesfully",
                            Success = true,
                        };
                    }
                    else
                    {
                        _logger.LogInformation($"Role was not creatd Succesffuly {roleResult.Errors.Select(e => e.Description)}");
                        return new UserManagerResponse
                        {
                            Success = false,
                            Message = $"The role {rolename} was not  created, Kindly try Again "
                            //  Message =   roleResult.Errors.ToString()
                        };
                    }
                }

                return new UserManagerResponse
                {
                    Success = false,
                    Message = "Role already exist, You can  not  create an Existing Role"
                };

            }
            catch (Exception ex)
            {
                _logger.LogInformation($"AN ErroR occured {ex.ToString()}");
                return new UserManagerResponse
                {
                    Success = false,
                    Message = $"An error  Occured try again Later ======>>>> {ex.Message}",
                    //  Error = result.Errors.Select(e => e.Description)
                };
                //  throw;
            }


        }
    }

}