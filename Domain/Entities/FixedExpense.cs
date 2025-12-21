using Domain.Enums;

namespace Domain.Entities;

public class FixedExpense : BaseEntity
{
    public Guid UserId { get; private set; }
    public string Description { get; private set; } = default!;
    public decimal Amount { get; private set; }
    public DateTime StartDate { get; private set; }
    public DateTime? EndDate { get; private set; }
    public RecurrenceType Recurrence { get; private set; }
    public Guid CategoryId { get; private set; }
    public Guid? CardId { get; private set; }

    protected FixedExpense() { }

    public FixedExpense(
        Guid userId,
        string description,
        decimal amount,
        DateTime startDate,
        RecurrenceType recurrence,
        Guid categoryId,
        Guid? cardId = null,
        DateTime? endDate = null)
    {
        if (string.IsNullOrWhiteSpace(description))
            throw new ArgumentException("Description is required");

        if (amount <= 0)
            throw new ArgumentException("Amount must be greater than zero");

        if (endDate.HasValue && endDate < startDate)
            throw new ArgumentException("End date cannot be before start date");

        UserId = userId;
        Description = description;
        Amount = amount;
        StartDate = startDate;
        EndDate = endDate;
        Recurrence = recurrence;
        CategoryId = categoryId;
        CardId = cardId;
    }

    public void UpdateAmount(decimal amount, Guid updatedBy)
    {
        if (amount <= 0)
            throw new ArgumentException("Amount must be greater than zero");

        Amount = amount;
        Touch(updatedBy);
    }

    public void UpdateDescription(string description, Guid updatedBy)
    {
        if (string.IsNullOrWhiteSpace(description))
            throw new ArgumentException("Description is required");

        Description = description;
        Touch(updatedBy);
    }

    public void End(DateTime endDate, Guid updatedBy)
    {
        if (endDate < StartDate)
            throw new ArgumentException("End date cannot be before start date");

        EndDate = endDate;
        Touch(updatedBy);
    }

    public bool IsActiveAt(DateTime referenceDate)
    {
        return referenceDate >= StartDate &&
               (EndDate is null || referenceDate <= EndDate);
    }
}