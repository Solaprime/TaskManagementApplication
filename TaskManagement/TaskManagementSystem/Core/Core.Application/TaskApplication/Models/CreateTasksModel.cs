using System;
using System.Collections.Generic;
using System.Text;
using TaskDomain.Entities;

namespace TaskApplication.Models
{
   public  class CreateTasksModel
    {
        public string Title { get; set; }
        public string Description { get; set; }

        public DateTimeOffset DueDate { get; set; }
        public TaskPriority TaskPriority { get; set; }
        public string CreatedBy { get; set; }

        //U can add Projectid to these as weel

    }
}
