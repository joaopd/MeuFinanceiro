using Domain.Abstractions.ErrorHandling;
using FluentResults;

namespace Api.Extensions;

public static class ResultExtensions
{
    public static IResult ToProblemDetails(this ResultBase result)
    {
        if (result.IsSuccess)
        {
            throw new InvalidOperationException("Cannot convert a successful result to a problem details response.");
        }

        var error = result.Errors.FirstOrDefault();
        if (error is null)
        {
            return Results.Problem(
                statusCode: StatusCodes.Status500InternalServerError, 
                title: "UnknownError", 
                detail: "An unexpected error occurred.");
        }

        var statusCode = StatusCodes.Status500InternalServerError;
        
        if (error.HasMetadataKey(nameof(ErrorType)) && 
            error.Metadata[nameof(ErrorType)] is ErrorType errorType)
        {
            statusCode = errorType switch
            {
                ErrorType.Validation => StatusCodes.Status400BadRequest,
                ErrorType.NotFound => StatusCodes.Status404NotFound,
                ErrorType.Conflict => StatusCodes.Status409Conflict,
                ErrorType.Unauthorized => StatusCodes.Status401Unauthorized,
                ErrorType.InternalServerError => StatusCodes.Status500InternalServerError,
                _ => StatusCodes.Status500InternalServerError
            };
        }

        var title = "Error";
        if (error is DomainError domainError)
        {
            title = domainError.ErrorCode;
        }

        return Results.Problem(
            statusCode: statusCode,
            title: title,
            detail: error.Message
        );
    }
}