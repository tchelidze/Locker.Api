using Locker.Domain.Features.Shared.ExecutionResult;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Net;

namespace Locker.Api.Features.Shared
{
    public static class ExecutionResultToIActionResultConverter
    {
        public static IActionResult ToActionResult(this ExecutionResult executionResult)
        {
            if (executionResult.IsValid())
            {
                return new OkResult();
            }

            if (executionResult.Message != null)
            {
                return new ObjectResult(new
                {
                    message = executionResult.Message
                })
                {
                    StatusCode = (int)StatusCodeFromExecutionTypeResult(executionResult.ResultType)
                };
            }

            return new StatusCodeResult((int)StatusCodeFromExecutionTypeResult(executionResult.ResultType));
        }

        public static IActionResult ToActionResult<T>(this ExecutionResult<T> executionResult)
        {
            if (executionResult.IsValid())
            {
                return new OkObjectResult(new
                {
                    result = executionResult.Value
                });
            }

            return ToActionResult((ExecutionResult)executionResult);
        }

        public static HttpStatusCode StatusCodeFromExecutionTypeResult(ExecutionResultType type)
        {
            return type switch
            {
                ExecutionResultType.Unauthorized => HttpStatusCode.Unauthorized,
                ExecutionResultType.Forbidden => HttpStatusCode.Forbidden,
                ExecutionResultType.Conflict => HttpStatusCode.Conflict,
                ExecutionResultType.UpstreamError => HttpStatusCode.BadGateway,
                ExecutionResultType.UpstreamErrorNoResponse => HttpStatusCode.GatewayTimeout,
                ExecutionResultType.ValidationError => HttpStatusCode.BadRequest,
                ExecutionResultType.Exception => HttpStatusCode.InternalServerError,
                ExecutionResultType.NotFound => HttpStatusCode.NotFound,
                ExecutionResultType.NoContent => HttpStatusCode.NoContent,
                ExecutionResultType.Ok => HttpStatusCode.OK,
                _ => throw new InvalidCastException($"value {type} cannot be converted to ${nameof(HttpStatusCode)} enum")
            };
        }
    }
}