using System;
using System.Diagnostics;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using CorrelationId;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace cats_api
{
    internal static class HttpExtensions
    {
        private static JsonSerializer js;

        static HttpExtensions()
        {
            js = new JsonSerializer
            {
                NullValueHandling = NullValueHandling.Ignore,
                DateFormatHandling = DateFormatHandling.IsoDateFormat,
                DateTimeZoneHandling = DateTimeZoneHandling.RoundtripKind // time zone information is preserved
            };
        }

        public static void WriteJson<T>(this HttpResponse response, T obj, string contentType = "application/json") where T : class
        {
            response.ContentType = contentType;
            using (var streamWriter = new HttpResponseStreamWriter(response.Body, Encoding.UTF8))
            using (var jsonWriter = new JsonTextWriter(streamWriter))
            {
                js.Serialize(jsonWriter, obj);
            }
        }

        public static void UseProblemDetailErrorHandler(this IApplicationBuilder app, Boolean isDevelopment, ILogger<ILogger<Startup>> logger)
        {
            var correlationContextAccessor = app.ApplicationServices.GetService<ICorrelationContextAccessor>();
            Action<IApplicationBuilder> configureExpHandler;

            configureExpHandler = errorApp =>
            {
                errorApp.Run(cxt =>
                {
                    var exception = cxt.Features.Get<IExceptionHandlerFeature>().Error;

                    var errorDetail = isDevelopment
                        ? exception.Demystify().ToString()
                        : "The instance value should be used to identify the problem when calling customer support";

                    var correlationId = cxt.TraceIdentifier ?? correlationContextAccessor.CorrelationContext.CorrelationId;

                    int? statusCode = (int)HttpStatusCode.InternalServerError;

                    var problemDetails = new ProblemDetails
                    {
                        Title = "An unexpected error has occurred!",
                        Status = statusCode,
                        Detail = errorDetail,
                        Instance = $"urn:myorganization:correlation_id:{correlationId}"
                    };

                    if (exception is BadHttpRequestException badRequestException)
                    {
                        problemDetails.Title = "Bad Request";
                        problemDetails.Status = badRequestException.StatusCode;
                        problemDetails.Detail = badRequestException.Message;
                    }
                    
                    logger.LogCritical($"{correlationId} {DateTime.Now.ToUniversalTime()} {problemDetails.Detail}");
                    cxt.Response.WriteJson(problemDetails, "application/problem+json");
                    return Task.CompletedTask;
                });
            };

            app.UseExceptionHandler(configureExpHandler);
        }
    }

    
}
