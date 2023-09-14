using AutoMapper;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using TaskApplication.Contracts;
using TaskApplication.ContractsImplementation;

namespace TaskApplication
{
    public static class ApplicationServiceRegistration
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            services.AddScoped<INotificationHelper, NotificationHelper>();
            services.AddAutoMapper(Assembly.GetExecutingAssembly());
            return services;

        }
    }
}