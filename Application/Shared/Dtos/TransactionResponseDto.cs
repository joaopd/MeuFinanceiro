using Domain.Enums;

namespace Application.Shared.Dtos;

public class TransactionResponseDto
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public Guid CategoryId { get; set; }
    public decimal Amount { get; set; }
    public DateTime TransactionDate { get; set; }
    public TransactionType TransactionType { get; set; }
    public Guid? CardId { get; set; }
    public PaymentMethod? PaymentMethod { get; set; }
    public Guid? InvoiceId { get; set; }
    public int InstallmentNumber { get; set; }
    public int TotalInstallments { get; set; }
    public bool IsFixed { get; set; }
    public bool IsPaid { get; set; }
    public string? Observation { get; set; }
}