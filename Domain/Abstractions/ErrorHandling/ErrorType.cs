namespace Domain.Abstractions.ErrorHandling;

public enum ErrorType
{
    Validation,
    NotFound,
    Conflict,
    Unauthorized,
    InternalServerError
}