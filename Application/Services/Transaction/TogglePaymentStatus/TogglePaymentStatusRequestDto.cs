namespace Application.Services.Transaction.TogglePaymentStatus;

public class TogglePaymentStatusRequestDto
{
    public Guid TransactionId { get; set; }
    public bool IsPaid { get; set; }
    public Guid UpdatedBy { get; set; }
}