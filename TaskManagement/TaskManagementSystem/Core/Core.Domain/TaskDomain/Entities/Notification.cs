using System;
using System.Collections.Generic;
using System.Text;

namespace TaskDomain.Entities
{
   public class Notification : BaseEntity
    {
        public DateTime DueDate { get; set; }
        public string Message { get; set; }
        public string StatusUpdate { get; set; }
        public Tasks Tasks { get; set; }
        public DateTime TimeStamp { get; set; }
        public NotificationStatus ReadStatus { get; set; }

    }
}
//Get all user Notification
//Mark nOTfication as read or Unread 
