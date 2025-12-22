namespace Domain.Entities;

public class Invoice : BaseEntity
{
    public Guid CardId { get; private set; }
    public DateTime ReferenceDate { get; private set; }
    public DateTime DueDate { get; private set; }      
    public decimal TotalAmount { get; private set; }
    public bool IsPaid { get; private set; }

    protected Invoice() { }

    public Invoice(Guid cardId, DateTime referenceDate, DateTime dueDate)
    {
        Id = Guid.NewGuid();
        CardId = cardId;
        ReferenceDate = referenceDate;
        DueDate = dueDate;
        TotalAmount = 0;
        IsPaid = false;
    }

    public void MarkAsPaid(Guid updatedBy)
    {
        if (IsPaid) return;

        IsPaid = true;
        Touch(updatedBy);
    }

    public void AddTransactionAmount(decimal amount)
    {
        if (IsPaid)
            throw new InvalidOperationException("Cannot add amount to a paid invoice");

        if (amount <= 0)
            throw new ArgumentException("Amount must be greater than zero");

        TotalAmount += amount;
    }
}