using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace TaskDomain.Entities
{
   public  class Tasks : BaseEntity
    {
        [Required]
        public string  Title { get; set; }
        [Required]
        public string Description { get; set; }
        [Required]
        public DateTimeOffset DueDate { get; set; }
        [Required]
        public TaskPriority  TaskPriority { get; set; }

        public TaskStatuses   TaskStatus { get; set; }
        public Guid?   ProjectId { get; set; }
        public bool   GenerateTaskExpiryNotification { get; set; }
       
        public string   AssignedTo { get; set; }
     //   public Guid?   ProjectId { get; set; }  u can Put Null here
     //put in user.Email here to reference the pserson dat owns
     //this task so mail can be sent

    }
}
