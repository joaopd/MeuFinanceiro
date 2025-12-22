using Domain.Enums;

namespace Domain.Entities;

public class Transaction : BaseEntity
{
    public Guid UserId { get; private set; }
    public Guid CategoryId { get; private set; }
    public decimal Amount { get; private set; }
    public DateTime TransactionDate { get; private set; }
    public TransactionType TransactionType { get; private set; }

    public Guid? CardId { get; private set; }
    public PaymentMethod? PaymentMethod { get; private set; }

    public int InstallmentNumber { get; private set; }
    public int TotalInstallments { get; private set; }

    public bool IsFixed { get; private set; }
    public bool IsPaid { get; private set; }
    public Guid? FixedExpenseId { get; private set; }
    public string? Observation { get; private set; } 
    
    public Transaction() { }

    public Transaction(
        Guid userId,
        Guid categoryId,
        decimal amount,
        DateTime transactionDate,
        TransactionType transactionType,
        Guid? cardId,
        PaymentMethod? paymentMethod,
        int installmentNumber,
        int totalInstallments,
        bool isFixed,
        bool isPaid,
        Guid? fixedExpenseId = null,
        string? observation = null)
    {
        if (amount <= 0)
            throw new ArgumentException("Amount must be greater than zero");

        if (cardId.HasValue && paymentMethod is null)
            throw new ArgumentException("Payment method is required when card is informed");

        UserId = userId;
        CategoryId = categoryId;
        Amount = amount;
        TransactionDate = transactionDate;
        TransactionType = transactionType;
        CardId = cardId;
        PaymentMethod = paymentMethod;
        InstallmentNumber = installmentNumber;
        TotalInstallments = totalInstallments;
        IsFixed = isFixed;
        IsPaid = isPaid;
        FixedExpenseId = fixedExpenseId;
        Observation = observation;
    }
    
    public Transaction(
        Guid id,
        Guid userId,
        Guid categoryId,
        decimal amount,
        DateTime transactionDate,
        TransactionType transactionType,
        Guid? cardId,
        PaymentMethod? paymentMethod,
        int installmentNumber,
        int totalInstallments,
        bool isFixed,
        bool isPaid,
        Guid? fixedExpenseId = null,
        string? observation = null)
    {
        if (amount <= 0)
            throw new ArgumentException("Amount must be greater than zero");

        if (cardId.HasValue && paymentMethod is null)
            throw new ArgumentException("Payment method is required when card is informed");

        Id = id;
        UserId = userId;
        CategoryId = categoryId;
        Amount = amount;
        TransactionDate = transactionDate;
        TransactionType = transactionType;
        CardId = cardId;
        PaymentMethod = paymentMethod;
        InstallmentNumber = installmentNumber;
        TotalInstallments = totalInstallments;
        IsFixed = isFixed;
        IsPaid = isPaid;
        FixedExpenseId = fixedExpenseId;
        Observation = observation;
    }
    
    public void SetPaymentStatus(Guid updatedBy)
    {
        IsPaid = !IsPaid;
        Touch(updatedBy);
    }
    
    public void ChangeAmount(decimal newAmount, Guid updatedBy)
    {
        if (newAmount <= 0)
            throw new ArgumentException("Amount must be greater than zero");

        Amount = newAmount;
        Touch(updatedBy);
    }
    
    public void ChangeDate(DateTime newDate, Guid updatedBy)
    {
        TransactionDate = newDate;
        Touch(updatedBy);
    }
    public void UpdateObservation(string? observation, Guid updatedBy)
    {
        Observation = observation;
        Touch(updatedBy);
    }
}
