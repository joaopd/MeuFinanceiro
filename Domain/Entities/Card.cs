using Domain.Enums;

namespace Domain.Entities;

public class Card : BaseEntity
{
    public string Name { get; private set; } = default!;
    public PaymentMethod PaymentMethod { get; private set; }
    public decimal? CreditLimit { get; private set; }

    protected Card() { }

    public Card(
        string name,
        PaymentMethod paymentMethod,
        decimal? creditLimit = null)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Card name is required");

        if (paymentMethod == PaymentMethod.CREDIT && creditLimit is null)
            throw new ArgumentException("Credit limit is required for credit cards");

        Name = name;
        PaymentMethod = paymentMethod;
        CreditLimit = creditLimit;
    }

    public void UpdateName(string name, Guid updatedBy)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Card name is required");

        Name = name;
        Touch(updatedBy);
    }
}