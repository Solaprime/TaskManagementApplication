using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TaskApplication.Contracts;
using TaskDomain.Entities;
using TaskPersistence.TasksManagementIdentity.Contract;

namespace TaskManagemantApi.Repositories
{
   public  interface IUnitOfWork
    {
        ITaskRepository _taskRepository { get; }
        Task CompleteAsync();
        IMapper _mapper { get; }
        IAsyncRepository<Project> _projectRepository { get; }
        INotificationRepository _notifcationRepository { get; }
        ITokenService _tokenService { get; }
       IAuthenticationService _authService { get; }
 

    }
}
