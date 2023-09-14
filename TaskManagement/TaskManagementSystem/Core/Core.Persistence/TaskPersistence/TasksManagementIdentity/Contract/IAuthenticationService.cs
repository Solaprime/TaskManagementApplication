using AppShared.Models;
using AppShared.Response;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace TaskPersistence.TasksManagementIdentity.Contract
{
    public interface IAuthenticationService
    {
        Task<UserManagerResponse> RegisterUserAsync(RegisterViewModel model);
        Task<UserManagerResponse> LoginUserAsync(LoginViewModel model);
        Task<UserManagerResponse> ForgetPasswordAsync(string email);
        Task<UserManagerResponse> ResetPasswordAsync(ResetPasswordViewModel model);
        Task<BaseResponse> AddUserToRole(RoleEmail roleEmail);
        Task<UserManagerResponse> CreateRole(string rolename);
        //Task GenerateConfirmEmailToken(ApplicationUser user);
        //Task<UserManagerResponse> ConfirmEmail(string userId, string token);
    }
}
