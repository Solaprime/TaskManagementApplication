using System;
using System.Collections.Generic;
using System.Text;
using TaskDomain.Entities;

namespace TaskApplication.Contracts
{
   public  interface INotificationHelper
    {
        Notification GenerateNotificationFromTask(Tasks task);
        Notification GenerateNotificationForTaskCompleted(Tasks task);
        string GenerateEmailNotficationTemplate(Tasks task);
        string GenerateTaskCompleteEmailTemplate(Tasks task);
        Notification GenerateNotificationForTaskExpiry(Tasks task);
        string GenerateTaskExpiryEmailTemplate(Tasks task);

        string GenerateTaskAssignedToProjectEmailTemplate(Tasks task);
        Notification GenerateNotificationForTaskAssgnedToProject(Tasks task);
        string GenerateTaskRemoveFromProjectEmailTemplate(Tasks task);
        Notification GenerateNotificationForTaskRemovedFromProject(Tasks task);
    }
}
