using AppShared.Models;
using AppShared.Response;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TaskDomain.Entities;

namespace TaskPersistence.TasksManagementIdentity.Contract
{
   public interface ITokenService
    {
        Task<UserManagerResponse> GenerateJwtToken(ApplicationUser user);
        Task<UserManagerResponse> VerifyAndGenerateToken(TokenRequest tokenRequest);
    }
}
