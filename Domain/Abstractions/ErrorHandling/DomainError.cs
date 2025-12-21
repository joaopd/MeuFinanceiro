using FluentResults;

namespace Domain.Abstractions.ErrorHandling;

public class DomainError : Error
{
    public const string ErrorCodeLiteral = "ErrorCode";

    protected DomainError(string code, string message)
        : base(message)
    {
        WithMetadata(ErrorCodeLiteral, code);
        ErrorCode = code;
    }

    public string ErrorCode { get; }
}