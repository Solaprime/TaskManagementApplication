using AppShared.Helpers;
using AppShared.Models;
using Hangfire;
using Infrastructure.Contract;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TaskApplication.Contracts;
using TaskApplication.ContractsImplementation;
using TaskDomain.Entities;
using TaskPersistence;

namespace TaskManagemantApi.TaskManagementBackground
{
    //Hosted service is sinfleton  whike TaskRepository is Scoped  u cantcall  scoped insdie singleton so we use 
    //servicescopeFactory for these 
    public class TaskExpiryNotification : BackgroundService
    {
       private readonly ILogger<TaskExpiryNotification> _logger;
     //   private readonly IConfiguration _config;
      //  private readonly IMailService _emailService;
      //  private readonly PersistentDBContext _context;
      // private readonly INotificationHelper _notificationHelper;
        private readonly IServiceScopeFactory _scopeFactory;
        //  PersistentDBContext context
        //  IMailService emailService,
        // IConfiguration config
        public TaskExpiryNotification(ILogger<TaskExpiryNotification> logger, 
           IServiceScopeFactory scopeFactory)
        {
            _logger = logger;
         //   _emailService = emailService;
           // _context = context;
            _scopeFactory = scopeFactory;
          //  _config = config;
        }
        protected override  async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                 //  var demo =  _config.GetSection("JwtConfig");
                    _logger.LogInformation($"we are inside a background service in the backgorud job");
                    using var scope = _scopeFactory.CreateScope();
                    var context = scope.ServiceProvider.GetRequiredService<PersistentDBContext>();
                    var notificationHelper = scope.ServiceProvider.GetRequiredService<INotificationHelper>();
                    var emailService = scope.ServiceProvider.GetRequiredService<IMailService>();
                    int expiryDays = 2;
                    var collection = context.Tasks as IQueryable<Tasks>;
                    //Checkk for IsCompleted
                    var taskInMemory = await collection.ToListAsync();
                    List<Tasks> result = taskInMemory.Where(a => a.DueDate.IsDueSoon(2)
                    && !string.IsNullOrWhiteSpace(a.CreatedBy) && !a.GenerateTaskExpiryNotification && a.TaskStatus != TaskStatuses.Completed)
                        .ToList();
                    foreach (var item in result)
                    {
                        var emailRequest = new EmailRequest() { To = item.CreatedBy, Subject = "A Task DueDate FromBackground", Body = notificationHelper.GenerateTaskExpiryEmailTemplate(item) };
                        var jobId = BackgroundJob.Enqueue(() => emailService.SendEmail(emailRequest));
                        item.GenerateTaskExpiryNotification = true;
                    }
                    context.Tasks.UpdateRange(result);
                    await context.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    _logger.LogError($"An Exception Occur inside background--jobs");
                    _logger.LogError($"An Exception Occur {ex.ToString()}");
                    throw;
                }
            }
      
        }
       
    }
}
