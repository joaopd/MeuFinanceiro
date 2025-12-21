namespace Domain.Abstractions.ErrorHandling;

public class FinanceError : DomainError
{
    public FinanceError(string code, string message, ErrorType errorType)
        : base(code, message)
    {
        WithMetadata(nameof(ErrorType), errorType);
    }
}