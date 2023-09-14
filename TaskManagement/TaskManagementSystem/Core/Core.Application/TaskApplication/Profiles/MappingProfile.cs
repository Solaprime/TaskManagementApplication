using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;
using TaskApplication.Models;
using TaskDomain.Entities;

namespace TaskApplication.Profiles
{
   public  class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Tasks, CreateTasksModel>().ReverseMap();
            CreateMap<Project, CreateProjectModel>().ReverseMap();
            CreateMap<Notification, NotificationModel>().ReverseMap();
        }
       
    }
}
