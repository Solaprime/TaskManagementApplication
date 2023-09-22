using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
//using TaskApplication.Models;
///using TaskApplication.Models;


namespace AppShared.CustomAnnotations
{
    //U want to use these  Attrivutes insdie  TaskApplication.Models.CreateTaskModel class 
    //but u are also here casting an Object into  CreateTasksModel MODEL 
    //SO WE HAE CIRCULAR DEPENDENCE 
    //
    public class DateTimeMustNotBeLessThanTodayRefactoFlow :  ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            
          ////  var taskModel = (TaskAppliCreateTasksModel)value;
          //  var taskModel = (CreateTasksModel)value;
          //  if (taskModel.DueDate >= DateTimeOffset.Now)
          //  {
          //      return ValidationResult.Success;
          //  }
            return new ValidationResult(ErrorMessage ?? "Make sure ur date is " +
                ">= than today");
        }
    }
}

///Add these Flow But Dependenciees to add   using TaskApplication.Models; and  CreateTasksModel is giving me issues
