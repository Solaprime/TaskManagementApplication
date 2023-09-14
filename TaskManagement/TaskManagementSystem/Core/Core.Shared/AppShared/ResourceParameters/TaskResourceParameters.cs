using System;
using System.Collections.Generic;
using System.Text;
using TaskDomain.Entities;

namespace AppShared.ResourceParameters
{
  public  class TaskResourceParameters
    {
        //public int TaskStatus { get; set; }
        //public int TaskPriority { get; set; }
        //public int DueForCurrent { get; set; }
        public TaskStatuses TaskStatus { get; set; }
        public TaskPriority TaskPriority { get; set; }
        public int DueForCurrent { get; set; }

    }
}
