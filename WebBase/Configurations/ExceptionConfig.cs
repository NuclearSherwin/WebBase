using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Common.Constants;
using Common.Exceptions;
using Common.Utility;
using Data.ViewModels;
using DnsClient.Internal;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Service.IServices;
using Service.Utility;

namespace WebBase.Configurations
{
    public static class ExceptionConfig
    {
        public static void UserExceptionMiddleWare(this IApplicationBuilder app, IWebHostEnvironment env)
        {
            var logger = app.ApplicationServices.GetService<ILogger>();
            var sendMailService = app.ApplicationServices.GetService<ISendMailErrorService>();

            app.UseExceptionHandler(new ExceptionHandlerOptions()
            {
                ExceptionHandler = async context =>
                {
                    var ex = context.Features.Get<IExceptionHandlerFeature>()?.Error;
                    if (ex == null)
                        return;

                    var error = new AppErrorModel(ex.Message, ex.StackTrace);

                    switch (ex)
                    {
                        case AppException appException when !string.IsNullOrEmpty(appException.Message):
							context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
							error = new AppErrorModel(appException.GetMessage(), appException.ErrorCode);
							break;
						
						// ReSharper disable once PatternAlwaysOfType
						case Exception exception when !string.IsNullOrEmpty(exception.Message):
							context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
							error.Message = exception.Message;
							if (!env.IsDevelopment())
								await WriteLog(context, ex, logger, sendMailService);
							
							break;
                        
                    }
                    
                    // return as json
                    context.Response.ContentType = "application/json";
                    await using var writer = new StreamWriter(context.Response.Body);
                    await writer.WriteAsync(JsonConvert.SerializeObject(error, new JsonSerializerSettings
                    {
                        ContractResolver = new CamelCasePropertyNamesContractResolver(),
                        ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                        DateTimeZoneHandling = DateTimeZoneHandling.Local,
                    }));

                    await writer.FlushAsync();
                }
            });
        }

        private static async Task WriteLog(HttpContext context, Exception exception, ILogger logger,
            ISendMailErrorService sendMailErrorService)
        {
            try
            {
                var appKey = $"AppKey: {context.Request.Headers["App-Key"].FirstOrDefault()}";
                var bodyMessage =
                    $"Exception: {exception.GetMessage()} \nStacktrace: {exception.StackTrace} \nUser Claim: {GetUserClaim(context)}";

                var message = string.Join("\n", new List<string> { appKey, bodyMessage });
                logger.LogError(message);

                await EmailLogger.Throw(StringEnums.EmailExceptionType.Error, sendMailErrorService, message);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                await EmailLogger.Throw(StringEnums.EmailExceptionType.Error, sendMailErrorService,
                    $"Cant' write logs for exception: {exception.GetMessage()}, The error is from {exception.Message}");
                
                
            }

        }
        
        
        private static string GetUserClaim(HttpContext httpContext)
        {
            var userClaim = string.Empty;
            var userClaims = httpContext.User?.Claims.ToList();

            return userClaims.Aggregate(userClaim, (current, claim) => current + $"\n{claim.Type}: {claim.Value}");
        }
    }
}