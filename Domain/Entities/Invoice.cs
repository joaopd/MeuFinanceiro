namespace Domain.Entities;

public class Invoice : BaseEntity
{
    public Guid CardId { get; private set; }
    public DateTime ReferenceDate { get; private set; }
    public DateTime DueDate { get; private set; }      
    public decimal TotalAmount { get; private set; }
    public bool IsPaid { get; private set; }

    public Invoice(Guid cardId, DateTime referenceDate, DateTime dueDate)
    {
        Id = Guid.NewGuid();
        CardId = cardId;
        ReferenceDate = referenceDate;
        DueDate = dueDate;
        TotalAmount = 0;
        IsPaid = false;
    }

    public void MarkAsPaid() => IsPaid = true;
    public void UpdateTotal(decimal amount) => TotalAmount += amount;
}