using Infrastructure.Contract;
using Infrastructure.Mail;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure
{
   public static class InfrastructureServiceRegistration
    {

        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
        {
           // services.Configure<EmailSettings>(configuration.GetSection("EmailSettings"));

          //  services.AddTransient<ICsvExporter, CsvExporter>();
            services.AddTransient<IMailService, MailSmtp>();

            return services;
        }

    }
}
