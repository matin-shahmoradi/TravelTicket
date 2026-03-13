using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace BuildingBlocks.Extensions
{
    public static class HttpExtensions
    {
        public static IResult ToHttpResult<T>(this Result<T> result, HttpContext context)
        {
            var error = result.Error!.Value;
            var statusCode = MapStatusCode(error.ErrorType);

            var problemDetails = new ProblemDetails()
            {
                Type = MapErrorTypeToProblemType(error.ErrorType),
                Title = MapErrorTypeToProblemTitle(error.ErrorType),
                Detail = error.Message,
                Status = statusCode,
                Instance = context.Request.Path
            };
            problemDetails.Extensions["traceId"] =
                Activity.Current?.Id ?? context.TraceIdentifier;
            
            return Results.Problem(problemDetails);
        }
        private static string MapErrorTypeToProblemType(ErrorType errorType)
        {
            return errorType switch
            {
                ErrorType.VALIDATION_ERROR => "Validation Error",
                ErrorType.NOT_FOUND => "Resource Not Found",
                _ => string.Empty,
            };
        }
        private static string MapErrorTypeToProblemTitle(ErrorType errorType)
        {
            return errorType switch
            {
                ErrorType.VALIDATION_ERROR => "https://datatracker.ietf.org/doc/html/rfc9110#section-15.5.1",
                ErrorType.NOT_FOUND => "https://datatracker.ietf.org/doc/html/rfc9110#section-15.5.5",
                _ => string.Empty,
            };
        }
        private static int MapStatusCode(ErrorType errorType)
        {
            return errorType switch
            {
                ErrorType.VALIDATION_ERROR => StatusCodes.Status400BadRequest,
                ErrorType.NOT_FOUND => StatusCodes.Status404NotFound,
                _ => StatusCodes.Status500InternalServerError,
            };
        }

    }
}
