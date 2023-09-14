using AppShared.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TaskPersistence.TasksManagementIdentity.Contract;

namespace TaskManagemantApi.Controllers.V1
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly ITokenService _tokenService;
        private readonly IAuthenticationService _authService;
        // ITokenService tokenService,
        public AuthController(ITokenService tokenService, IAuthenticationService authService)
        {
            _tokenService = tokenService;
            _authService = authService;
        }
        [HttpPost("Register")]
        public async Task<IActionResult> RegisterAsync(RegisterViewModel model)
        {
                var result = await _authService.RegisterUserAsync(model);
                if (result.Success)
                {
                    //Find a way to Blu these Email
                    return Ok(result);
                }
                return BadRequest(result);
        }

        [HttpPost("Login")]
        public async Task<IActionResult> LoginAsync([FromBody] LoginViewModel model)
        {
                var result = await _authService.LoginUserAsync(model);
                if (result.Success)
                {
                    return Ok(result);
                }
                return BadRequest(result);
        }
        [HttpPost("RefreshToken")]
        public async Task<IActionResult> RefreshToken(TokenRequest tokenRequest)
        {
                var result = await _tokenService.VerifyAndGenerateToken(tokenRequest);
                if (result.Success == false)
                {
                    return BadRequest(result);
                }
                return Ok(result);
        }

        [HttpPost("ForgetPassword/{email}")]
        public async Task<IActionResult> ForgetPassword(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                return BadRequest("The string is empty");
            }
            var result = await _authService.ForgetPasswordAsync(email);
            if (result.Success)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }

        [HttpPost("ResetPassword")]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
                var result = await _authService.ResetPasswordAsync(model);
                if (result.Success)
                {
                    return Ok(result);
                }
                else
                {
                    return BadRequest(result);
                }
      
        }

    }
}
//Register, RefreshToken, ResetPassword