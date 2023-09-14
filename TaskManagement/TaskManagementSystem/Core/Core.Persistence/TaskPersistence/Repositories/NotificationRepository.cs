using AppShared.Response;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TaskApplication.Contracts;
using TaskDomain.Entities;

namespace TaskPersistence.Repositories
{
    public class NotificationRepository: BaseRepository<Notification>, INotificationRepository
    {
        private readonly ILogger<NotificationRepository> _logger;
        public NotificationRepository(PersistentDBContext dbContext, ILogger<NotificationRepository> logger) : base(dbContext)
        {
            _logger = logger;
        }

        public async Task<BaseResponse> ChangeNotficationStatus(Guid notificationId, NotificationStatus status)
        {
            try
            {
                // _logger.LogInFormation($"Pi Call to {ex.ToString()}");
                _logger.LogInformation($"An Api Call to Change  Notification status ");
               
                var notificationToUpdate = await GetByIdAsync(notificationId);
                if (notificationToUpdate == null)
                {
                    _logger.LogInformation($"We cant Find Notification");
                    return new BaseResponse() { Success = false, Message = "We cant Find Notification u ARE looking for" };

                }
                notificationToUpdate.ReadStatus = status;
                 await UpdateAsync(notificationToUpdate);
                //Use Smtp to Fire Email
                return new BaseResponse() { Success = true, Message = "Notifcation Status Updated" };
            }
            catch (Exception ex)
            {
                _logger.LogError($"An error Occured {ex.ToString()}");
                return new BaseResponse() { Message = ex.ToString(), Success = false };
                //   throw;
            }
        }
    }
}
