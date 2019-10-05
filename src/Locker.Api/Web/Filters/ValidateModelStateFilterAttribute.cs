using System;
using System.Linq;
using System.Threading.Tasks;
using Locker.Api.Features.Shared;
using Locker.Domain.Features.Shared.ExecutionResult;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Locker.Api.Web.Filters
{
    public class ValidateModelStateFilterAttribute : Attribute, IAsyncActionFilter
    {
        public Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            if (!context.ModelState.IsValid)
            {
                var validationError = context.ModelState.Values
                    .SelectMany(it => it.Errors.Select(error => error.ErrorMessage))
                    .FirstOrDefault(error => !string.IsNullOrEmpty(error));

                context.Result = new ExecutionResult
                {
                    ResultType = ExecutionResultType.ValidationError,
                    Message = validationError
                }.ToActionResult();

                return Task.CompletedTask;
            }

            return next();
        }
    }
}
