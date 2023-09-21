using Hangfire;
using Infrastructure;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TaskApplication;
using TaskManagemantApi.TaskManagementBackground;
using TaskPersistence;


namespace TaskManagemantApi
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers()
                .AddNewtonsoftJson(setupAction =>
                {
                    setupAction.SerializerSettings.ContractResolver =
                       new CamelCasePropertyNamesContractResolver();
                });
            //Registering for Perssistence
            services.AddPersistenceServices(Configuration);
            //Registering for Application
            services.AddApplicationServices();
            //Registering for Infrastructure
            services.AddInfrastructureServices(Configuration);
            //SwaggerConfiguration
            services.AddSwaggerGen(setupAction =>
            {
                  setupAction.SwaggerDoc($"LibraryOpenApiSpecification",
                  new Microsoft.OpenApi.Models.OpenApiInfo()
                  {
                   Title = "TaskManageManetApi",
                  Description = "A Task Management Application",
                  });
               setupAction.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
               {
                   Name = "Authorization",
                   Type = SecuritySchemeType.ApiKey,
                   Scheme = "Bearer",
                   BearerFormat = "JWT",
                   In = ParameterLocation.Header,
                   Description = "Enter 'Bearer' [space] and then your valid token in the text input below .\r\n\r\nExample: \"Bearer eyjhGciojdkrandomshitjvnvlvlvkkossps\"",
               });
               setupAction.AddSecurityRequirement(new OpenApiSecurityRequirement
               {
                  {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer" }
                        }, new List<string>()
                   }
               });
            });
            services.AddHostedService<TaskExpiryNotification>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();
            app.UseSwagger();

            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/LibraryOpenApiSpecification/swagger.json", "Ticket Management API");
            });
            app.UseHangfireDashboard();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
