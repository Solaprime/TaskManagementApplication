using AppShared.Helpers;
using AppShared.Models;
using AppShared.ResourceParameters;
using AppShared.Response;
using Hangfire;
using Infrastructure.Contract;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskApplication.Contracts;
using TaskDomain.Entities;

namespace TaskPersistence.Repositories
{
  public  class TaskRepository : BaseRepository<Tasks>, ITaskRepository
    {
        private readonly IAsyncRepository<Project> _projectRepo;
        private readonly ILogger<TaskRepository> _logger;
        private readonly INotificationRepository _notificationRepository;
        private readonly IMailService _emailService;
        private readonly INotificationHelper _notificationHelper;
        public TaskRepository(PersistentDBContext dbContext, IAsyncRepository<Project> projectRepo,
            ILogger<TaskRepository> logger, INotificationRepository notificationRepository, IMailService emailService,
            INotificationHelper notificationHelper) : base(dbContext)
        {
            _projectRepo = projectRepo;
            _logger = logger;
            _notificationRepository = notificationRepository;
            _emailService  = emailService;
            _notificationHelper = notificationHelper;
        }
     
        public async  Task<BaseResponse> AddTaskToProject(Guid taskId, Guid projectId)
        {
            try
            {

                _logger.LogInformation($"An Api Call to Add Task To Projeect ");
                var result = await _projectRepo.ExistAsync(projectId);
                var taskToUpdate = await GetByIdAsync(taskId);
                if (!result && taskToUpdate == null)
                {
                    _logger.LogInformation($"We cant Find Proejct task Combination ");
                    return new BaseResponse() { Success = false, Message = "We cant Find Proejct task Combination" };
                  

                }
                if (taskToUpdate.ProjectId == projectId)
                {
                    _logger.LogInformation($"Task Added To project");
                    return new BaseResponse() { Success = false, Message = "Task Already added to Project" };
                }
                taskToUpdate.ProjectId = projectId;
                //Update Task //Update Project
                await UpdateAsync(taskToUpdate);
                if (taskToUpdate.AssignedTo != null)
                {
                    var emailRequest = new EmailRequest() { To = taskToUpdate.AssignedTo, Subject = "Task Assigned to Project", Body = _notificationHelper.GenerateTaskAssignedToProjectEmailTemplate(taskToUpdate) };
                    var jobId = BackgroundJob.Enqueue(() => _emailService.SendEmail(emailRequest));
                    _logger.LogInformation($"Task was aCompleted email was sent   with  jobId {jobId} for email Notfication sent  ");
                }
                return new BaseResponse() { Success = true, Message = "Task Added Sucesfully to Project" };
            }
            catch (Exception ex)
            {
                _logger.LogError($"An error Occured {ex.ToString()}");
                return new BaseResponse() { Message = ex.ToString(), Success = false };
                
            }
          
        }

        public async  Task<List<Tasks>> GetTaskBased(TaskResourceParameters parameters)
        {
            try
            {
                if (parameters == null)
                { 
                    throw new ArgumentNullException(nameof(parameters));
                }
                var collection =  _dbContext.Tasks as IQueryable<Tasks>;
                if (parameters.TaskStatus != 0)
                {
                    collection = collection.Where(a => a.TaskStatus == parameters.TaskStatus);
                }
                if (parameters.TaskPriority != 0)
                {
                    collection = collection.Where(a => a.TaskPriority == parameters.TaskPriority);
                }

                if (parameters.DueForCurrent != 0)
                {
                  //  collection = collection.Where(a => a.DueDate.IsDueSoon(parameters.DueForCurrent));

                    //   return  await collection;
                    //var result = await collection.ToListAsync();
                    //  result = result.WhereAsync(a => a.DueDate.IsDueSoon(parameters.DueForCurrent));
                    //var result = collection.Select(a => a.DueDate.IsDueSoon(parameters.DueForCurrent));
                    //return await result.ToListAsync();
                    
                    //var listMemory = await collection.ToListAsync();
                    // listMemory.Where(a => a.DueDate.IsDueSoon(parameters.DueForCurrent));
                    //return listMemory;s

                    var listMemory = await collection.ToListAsync();
                   List<Tasks> result2 = listMemory.Where(a => a.DueDate.IsDueSoon(parameters.DueForCurrent)).ToList();
                    return result2;
                }
                return await collection.ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError($"An error Occured {ex.ToString()}");
                return null;
            }
        }
        //Mail should be seent to who is assgnrd t0
        public async Task<BaseResponse> RemoveTaskFromProject(Guid taskId, Guid projectId)
        {
            try
            {
                _logger.LogInformation($"An Api Call to RemoveTask To Projeect ");
                var result = await _projectRepo.ExistAsync(projectId);
                var taskToUpdate = await GetByIdAsync(taskId);
                if (result && taskToUpdate != null)
                {
                    _logger.LogInformation($"We cant Find Proejct task Combination ");
                    return new BaseResponse() { Success = false, Message = "We cant Find Proejct task Combination" };

                }
                taskToUpdate.ProjectId = null;
                await UpdateAsync(taskToUpdate);
                if (taskToUpdate.AssignedTo != null)
                {
                    var emailRequest = new EmailRequest() { To = taskToUpdate.AssignedTo, Subject = "Task Removed From Project", Body = _notificationHelper.GenerateTaskRemoveFromProjectEmailTemplate(taskToUpdate) };
                    var jobId = BackgroundJob.Enqueue(() => _emailService.SendEmail(emailRequest));
                    _logger.LogInformation($"Task was aCompleted email was sent   with  jobId {jobId} for email Notfication sent  ");
                }
                return new BaseResponse() { Success = true, Message = "Task Removed Sucesfully From Project" };
            }
            catch (Exception ex)
            {
                _logger.LogError($"An error Occured {ex.ToString()}");
                return null;
            }
          
        }
        public async Task<BaseResponse> ChangeTasksStatus(Guid taskId, TaskStatuses taskStatus)
        {
            try
            {
                _logger.LogInformation($"An Api Call to Add Task To Projeect ");
                var taskToUpdate = await GetByIdAsync(taskId);
                if (taskToUpdate == null)
                {
                    _logger.LogInformation($"We cant Find Proejct task Combination ");
                    return new BaseResponse() { Success = false, Message = "We cant Find Proejct task Combination" };
               
                }
                taskToUpdate.TaskStatus = taskStatus;
                await UpdateAsync(taskToUpdate);
                if (TaskStatuses.Completed == taskStatus)
                {
                    var result = _notificationHelper.GenerateNotificationForTaskCompleted(taskToUpdate);
                    await _notificationRepository.AddAsync(result);
                   
                     var emailRequest = new EmailRequest() { To = taskToUpdate.CreatedBy, Subject = "Task Completed Notfication", Body = _notificationHelper.GenerateTaskCompleteEmailTemplate(taskToUpdate) };
                    var jobId = BackgroundJob.Enqueue(() => _emailService.SendEmail(emailRequest));
                    _logger.LogInformation($"Task was aCompleted email was sent   with  jobId {jobId} for email Notfication sent  ");
                }
                //Use Smtp to Fire Email
                return new BaseResponse() { Success = true, Message = "Task Status Updated" };
            }
            catch (Exception ex)
            {
                _logger.LogError($"An error Occured {ex.ToString()}");
                return new BaseResponse() { Message = ex.ToString(), Success = false };
               
                //   throw;
            }
        }

        public async  Task<BaseResponse> AssignTasksToUser(string userEmailtobeAssignedTo, Guid taskId)
        {
            try
            {
                
                _logger.LogInformation($"An Api Call to Assign Task To a uSER  ");
                var taskToUpdate = await GetByIdAsync(taskId);
                if (taskToUpdate == null)
                {
                    _logger.LogInformation($"We cant Find task you are Looking For ");
                    return new BaseResponse() { Success = false, Message = "We cant task u are Looking for " };
               
                }
                var Notification = _notificationHelper.GenerateNotificationFromTask(taskToUpdate);
                await _notificationRepository.AddAsync(Notification);
                taskToUpdate.AssignedTo = userEmailtobeAssignedTo;
                await UpdateAsync(taskToUpdate);
                var request = new EmailRequest() { To = userEmailtobeAssignedTo, Subject = "A NewTask Assigmente", Body = _notificationHelper.GenerateEmailNotficationTemplate(taskToUpdate) };
                var jobId = BackgroundJob.Enqueue(() => _emailService.SendEmail(request));
                _logger.LogInformation($"Task was assigned to user with {jobId} for email Notfication sent  ");
                return new BaseResponse() { Success = true, Message = $"Task Sucessfully assigned to {userEmailtobeAssignedTo} " };
            }
            catch (Exception ex)
            {
                _logger.LogError($"An error Occured {ex.ToString()}");
                return new BaseResponse() { Message = ex.ToString(), Success = false };
            }
        }

    }
}
///Note
/// Calculated properties (nor unmapped properties) cannot be used in LINQ statements backed by EF.