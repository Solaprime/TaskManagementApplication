using System;
using System.Collections.Generic;
using System.Text;
using TaskDomain.Entities;

namespace TaskApplication.Models
{
   public  class NotificationModel
    {
        public DateTime DueDate { get; set; }
        public string Message { get; set; }
        public string StatusUpdate { get; set; }
        public Tasks Tasks { get; set; }
        public DateTime TimeStamp { get; set; }
    }
}
