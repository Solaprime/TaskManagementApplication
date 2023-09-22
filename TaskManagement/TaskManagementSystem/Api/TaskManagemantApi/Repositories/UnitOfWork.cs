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
    public class UnitOfWork : IUnitOfWork
    {
        public ITaskRepository _taskRepository { get; private set; }

        public IMapper _mapper { get; private set; }

        public IAsyncRepository<Project> _projectRepository { get; private set; }

        public INotificationRepository _notifcationRepository { get; private set; }

        public ITokenService _tokenService { get; private set; }

        public IAuthenticationService _authService { get; private set; }
        public UnitOfWork(ITaskRepository taskRepository, IMapper mapper, IAsyncRepository<Project> projectRepository,
            INotificationRepository notifcationRepository, ITokenService tokenService, IAuthenticationService authService)
        {
            
            _taskRepository = taskRepository;
            _projectRepository = projectRepository;
            _notifcationRepository = notifcationRepository;
            _tokenService = tokenService;
            _authService = authService;
            _mapper = mapper;
        }

        public Task CompleteAsync()
        {
            throw new NotImplementedException();
        }
    }
}
