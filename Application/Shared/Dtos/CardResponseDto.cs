namespace Application.Shared.Dtos;

public record CardResponseDto(
    Guid Id,
    string Name,
    decimal CreditLimit,
    decimal CurrentInvoiceAmount,
    bool IsCurrentInvoicePaid,
    Guid UserId,
    int ClosingDay,
    int DueDay,
    string Color
);