using System;
using System.Collections.Generic;
using System.Text;
using TaskApplication.Contracts;
using TaskDomain.Entities;

namespace TaskApplication.ContractsImplementation
{
    public class NotificationHelper : INotificationHelper
    {
        public string GenerateEmailNotficationTemplate(Tasks task)
        {
            return $"A new Task {task.Title} created by {task.CreatedBy}  with due date {task.DueDate} was assigned to you with";
        }

        public Notification GenerateNotificationForTaskAssgnedToProject(Tasks task)
        {
            return new Notification() { Tasks = task, Message = $"task{task.Title } has just been assigned to Project" };
        }

        public Notification GenerateNotificationForTaskCompleted(Tasks task)
        {
            return new Notification() { Tasks = task, Message = $"task{task.Title } has just been assigned Completed" };
        }

        public Notification GenerateNotificationForTaskExpiry(Tasks task)
        {
            return new Notification() { Tasks = task, Message = $"task{task.Title }Will expire at {task.DueDate}" };
        }

        public Notification GenerateNotificationForTaskRemovedFromProject(Tasks task)
        {

            return new Notification() { Tasks = task, Message = $"task{task.Title } has just been Removed from Project" };
        }

        public Notification GenerateNotificationFromTask(Tasks task)
        {
            return new Notification() { Tasks = task, Message = $"task{task.Title } has just been assigned to U" };
        }

        public string GenerateTaskAssignedToProjectEmailTemplate(Tasks task)
        {
            return $"A Task {task.Title} created by {task.CreatedBy}  with description {task.Description} has been assgned to project {task.ProjectId}  ";
        }

        public string GenerateTaskCompleteEmailTemplate(Tasks task)
        {
            return $"A Task {task.Title} created by {task.CreatedBy}  with description {task.Description} has been Completed ";
        }

        public string GenerateTaskExpiryEmailTemplate(Tasks task)
        {
            return $"A Task {task.Title} created by {task.CreatedBy}  will expire {task.DueDate} Withing 48Hours ";
        }

        public string GenerateTaskRemoveFromProjectEmailTemplate(Tasks task)
        {
            return $"A Task {task.Title} created by {task.CreatedBy}  with description {task.Description} has been remove from project {task.ProjectId}  ";
        }
    }
}
