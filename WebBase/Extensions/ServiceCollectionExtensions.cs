using System;
using System.Collections.Generic;
using Common.Constants;
using Data.Context;
using Data.IRepository.IBaseRepository;
using Data.MongoDbSettings;
using Data.Repository.BaseRepository;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Service.IServices;
using Service.Services;
using Service.Utility;
using WebBase.Configurations;
using WebBase.Initializer;

namespace WebBase.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddService(this IServiceCollection service)
        {
            // context
            service.AddSingleton<IMongoContext, MongoContext>();
            
            // repository
            service.AddTransient(typeof(IBaseRepository<>), typeof(BaseRepository<>));
            
            // service
            service.AddTransient<ISendMailErrorService, SendMailErrorService>();
            service.AddTransient<ISendMailBusinessService, SendMailBusinessService>();
            service.AddScoped<IUserService, UserService>();
            service.AddScoped<IJwtUtils, JwtUtils>();
            service.AddScoped<IDbInitializer, DbInitializer>();

            return service;
        }
        
        public static IServiceCollection AddAutoMapper(this IServiceCollection service)
        {
            //auto mapper config
            var mapper = MappingConfig.RegisterMap().CreateMapper();
            service.AddSingleton(mapper);
            service.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
            return service;
        }
        
        public static IServiceCollection AddCustomCors(this IServiceCollection service)
        {
            service.AddCors(options =>
            {
                options.AddPolicy("Policy", builder =>
                {
                    var corsOrigins = AppSettings.CORS ?? new string[0]; // Null check and fallback to an empty array
                    builder.WithOrigins(corsOrigins)
                        .AllowAnyMethod()
                        .AllowAnyHeader()
                        .AllowCredentials();
                });
            });

            return service;
        }


        public static IServiceCollection AddSwagger(this IServiceCollection service)
        {
            service.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Web api Duoc Si Thau Dau", Version = "v1" });

                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description =
                        "JWT Authorization header using the Bearer scheme. \r\n\r\n " +
                        "Enter 'Bearer' [space] and then your token in the text input below.\r\n\r\n" +
                        "Example: \"Bearer 12345abcdef\"",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer"
                });

                c.AddSecurityRequirement(new OpenApiSecurityRequirement()
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            },
                            Scheme = "oauth2",
                            Name = "Bearer",
                            In = ParameterLocation.Header,
                        },
                        new List<string>()
                    }
                });

            });
            
            return service;
        }
        
        public static IServiceCollection BackgroundService(this IServiceCollection services)
        {
            // background service
            // services.AddHostedService<AppointmentSenderService>();
           
            return services;
        }
        
        public static IServiceCollection MailSenderService(this IServiceCollection services, IConfiguration configuration)
        {
            // email setting
            services.AddOptions ();                                         
            var mailErrorSettings = configuration.GetSection ("MailErrorSettings"); 
            services.Configure<MailErrorSettings> (mailErrorSettings);
            
            var mailBusinessSettings = configuration.GetSection ("MailBusinessSettings"); 
            services.Configure<MailBusinessSettings> (mailBusinessSettings);
           
            return services;
        }
        
        public static IServiceCollection DatabaseService(this IServiceCollection services, IConfiguration configuration)
        {
            //database
            services.Configure<MongoDbSettings>(
                configuration.GetSection("DatabaseSetting"));

            services.AddSingleton<IMongoDbSettings>(sp =>
                sp.GetRequiredService<IOptions<MongoDbSettings>>().Value);
           
            return services;
        }
    }
}