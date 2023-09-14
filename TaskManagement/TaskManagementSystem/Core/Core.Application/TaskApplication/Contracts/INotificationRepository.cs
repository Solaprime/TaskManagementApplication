using AppShared.Response;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TaskDomain.Entities;

namespace TaskApplication.Contracts
{
   public  interface INotificationRepository : IAsyncRepository<Notification>
    {
        Task<BaseResponse> ChangeNotficationStatus(Guid notificationId, NotificationStatus status);
    }
}
