using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace cats_api
{
    public class ValidationProblemDetailsResult : IActionResult
    {
        public Task ExecuteResultAsync(ActionContext context)
        {
            var problemExtensionPropertyName = "validationErrors";

            if(context.ModelState.ErrorCount <= 0) return Task.CompletedTask;

            var problem = new ProblemDetails
            {
                Detail = $"See {problemExtensionPropertyName} for details",
                Instance = context.HttpContext.TraceIdentifier,
                Status = (int)HttpStatusCode.BadRequest                
            };
            
            var validationErrors = context.ModelState.Where(ms => ms.Value.ValidationState == ModelValidationState.Invalid)
                                                .SelectMany(ms=> ms.Value.Errors.Select(err=> Tuple.Create(ms.Key, err.ErrorMessage).ToValueTuple()))
                                                .ToList();

            if(validationErrors.Count == 1)
            {
                (string Key, string ErrorMessage) ve = validationErrors.First();
                problem.Detail = ve.ErrorMessage;
            }
            else
            {
                problem.Extensions.Add(problemExtensionPropertyName, validationErrors);
            }

            context.HttpContext.Response.WriteJson<ProblemDetails>(problem, "application/problem+json");

            return Task.CompletedTask;
        }
    }
}
