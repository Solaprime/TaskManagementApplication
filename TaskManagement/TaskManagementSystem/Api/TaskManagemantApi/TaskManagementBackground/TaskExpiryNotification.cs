using AppShared.Helpers;
using AppShared.Models;
using Hangfire;
using Infrastructure.Contract;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TaskApplication.Contracts;
using TaskDomain.Entities;
using TaskPersistence;

namespace TaskManagemantApi.TaskManagementBackground
{
    public class TaskExpiryNotification : BackgroundService
    {
        private readonly ILogger<TaskExpiryNotification> _logger;
        private readonly ITaskRepository _taskRepository;
        private readonly IMailService _emailService;
        private readonly PersistentDBContext _context;
       private readonly INotificationHelper _notificationHelper;
        public TaskExpiryNotification(ILogger<TaskExpiryNotification> logger, ITaskRepository taskRepository,
           IMailService emailService, PersistentDBContext context, INotificationHelper notificationHelper)
        {
            _logger = logger;
            _taskRepository = taskRepository;
            _emailService = emailService;
            _context = context;
            _notificationHelper = notificationHelper;

        }

        protected override  async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                int expiryDays = 2;
                var collection = _context.Tasks as IQueryable<Tasks>;
                //Checkk for IsCompleted
                collection = collection.Where(a => a.DueDate.IsDueSoon(expiryDays)
                && !string.IsNullOrWhiteSpace(a.CreatedBy) && !a.GenerateTaskExpiryNotification && a.TaskStatus != TaskStatuses.Completed);
                foreach (var item in collection)
                {
                    var emailRequest = new EmailRequest() { To = item.CreatedBy, Subject = "A Task DueDate", Body = _notificationHelper.GenerateTaskExpiryEmailTemplate(item) };

                    var jobId = BackgroundJob.Enqueue(() => _emailService.SendEmail(emailRequest));
                  //  var result = _emailService.SendEmail(emailRequest);
                    item.GenerateTaskExpiryNotification = true;
                   
                }
                _context.Tasks.UpdateRange(collection);
                await _context.SaveChangesAsync();
               
            }
            catch (Exception ex)
            {
                _logger.LogError($"An Exception Occur {ex.ToString()}");
            //    throw;
            }
        }
       
    }
}
