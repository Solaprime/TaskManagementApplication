using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using TaskApplication.Models;

namespace TaskApplication.CustomAnnotations
{
   public  class DateTimeMustNotBeLessThanToday : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            // var taskModel = (TaskApplication.Models.CreateTasksModel)value;
            var taskModel = (CreateTasksModel)value;
            if (taskModel.DueDate >= DateTimeOffset.Now)
            {
                return ValidationResult.Success;
            }
            return new ValidationResult(ErrorMessage ?? "Make sure ur date is " +
                ">= than today");
        }
    }
}

