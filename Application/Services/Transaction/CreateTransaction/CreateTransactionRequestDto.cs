using Domain.Enums;

namespace Application.Services.Transaction.CreateTransaction;

public class CreateTransactionRequestDto
{
    public Guid UserId { get; set; }
    public Guid CategoryId { get; set; }
    public decimal Amount { get; set; }
    public DateTime TransactionDate { get; set; }
    public TransactionType TransactionType { get; set; }
    public Guid? CardId { get; set; }
    public PaymentMethod? PaymentMethod { get; set; }
    public  int TotalInstallments  { get; set; }
    
    public bool IsFixed { get; set; }
}