using AppShared.ConfigurationDto;
using Hangfire;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using TaskApplication.Contracts;
using TaskDomain.Entities;
using TaskPersistence.Repositories;
using TaskPersistence.TasksManagementIdentity;
using TaskPersistence.TasksManagementIdentity.Contract;
using TaskPersistence.TasksManagementIdentity.Services;

namespace TaskPersistence
{
   public  static class PersistenceServicerRegistration
    {

        public static IServiceCollection AddPersistenceServices(this IServiceCollection services, IConfiguration configuration)
        {

            //Config For Jwt
            services.Configure<JwtConfig>(configuration.GetSection("JwtConfig"));

            services.AddDbContext<PersistentDBContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("TaskManagementConnectionString")));

            services.AddScoped(typeof(IAsyncRepository<>), typeof(BaseRepository<>));
            services.AddScoped(typeof(IAsyncRepository<Project>), typeof(BaseRepository<Project>));
           // services.AddScoped(typeof(IAsyncRepository<Project>), typeof(BaseRepository<Project>));
            services.AddScoped<ITaskRepository, TaskRepository>();
            services.AddScoped<ITokenService, TokenService>();
            services.AddScoped<IAuthenticationService, AuthenticationService>();
            services.AddScoped<INotificationRepository, NotificationRepository>();

            //var key = Encoding.ASCII.GetBytes(Configuration.GetSection("JwtConfig:Secret").Value);
            var key = Encoding.ASCII.GetBytes(configuration["JwtConfig:Secret"]);
            var tokenValidationParams = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = false, // this is only for development mode 
                ValidateAudience = false, // this is only for development mode
                // check the requireexpirationtime property properlu
                RequireExpirationTime = false,  // this is only for development mode in real life token expired and theny need to be refreshed
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero

            };
            services.AddSingleton(tokenValidationParams);
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
              .AddJwtBearer(jwt =>
              {
                jwt.SaveToken = true;
                  jwt.TokenValidationParameters = tokenValidationParams;
                //jwt.TokenValidationParameters = new TokenValidationParameters {

                //    ValidateIssuerSigningKey = true,
                //    IssuerSigningKey = new SymmetricSecurityKey
                //    (Encoding.ASCII.GetBytes(configuration["JwtConfig:Secret"])),
                //    ValidateIssuer = false, // this is only for development mode 
                //    ValidateAudience = false, // this is only for development mode
                //                              // check the requireexpirationtime property properlu
                //    RequireExpirationTime = false,  // this is only for development mode in real life token expired and theny need to be refreshed
                //    ValidateLifetime = true,
                //    ClockSkew = TimeSpan.Zero

                //};
                 jwt.Events = new JwtBearerEvents()
                  {
                      OnAuthenticationFailed = c =>
                      {
                          c.NoResult();
                          c.Response.StatusCode = 500;
                          c.Response.ContentType = "text/plain";
                          return c.Response.WriteAsync(c.Exception.ToString());
                      },
                      OnChallenge = context =>
                      {
                          context.HandleResponse();
                          context.Response.StatusCode = 401;
                          context.Response.ContentType = "application/json";
                          var result = JsonConvert.SerializeObject("401 Not authorized");
                          return context.Response.WriteAsync(result);
                      },
                      OnForbidden = context =>
                      {
                          context.Response.StatusCode = 403;
                          context.Response.ContentType = "application/json";
                          var result = JsonConvert.SerializeObject("403 Not authorized");
                          return context.Response.WriteAsync(result);
                      },
                  };

              });
 
            services.AddIdentity<ApplicationUser, IdentityRole>(
               options =>
               {
                   options.Password.RequireUppercase = false;
                   options.Password.RequireNonAlphanumeric = false; //  CHANGE TO TRUE
                   options.Password.RequireDigit = false;    // chNGE TO TRUE
                   options.Password.RequireLowercase = false;   // CHANGE TO TRUE
                   options.Password.RequiredLength = 5;
                   options.User.RequireUniqueEmail = true;

                   //options.SignIn.RequireConfirmedAccount = false;
               }).AddEntityFrameworkStores<PersistentDBContext>().AddDefaultTokenProviders();
            //HangFireConfiguration
            services.AddHangfire(x => x.UseSqlServerStorage(configuration.GetSection("HangfireDb:TaskManagementHangfireDb").Value));
            services.AddHangfireServer();
            return services;
        }

    }
}
