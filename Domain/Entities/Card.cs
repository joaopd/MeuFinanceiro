namespace Domain.Entities;

public class Card : BaseEntity
{
    public string Name { get; private set; }
    public decimal CreditLimit { get; private set; }
    public Guid UserId { get; private set; }
    public int ClosingDay { get; private set; }
    public int DueDay { get; private set; }
    public string Color { get; private set; } 

    protected Card() { }

    public Card(string name, decimal creditLimit, Guid userId, int closingDay, int dueDay, string color)
    {
        Name = name;
        CreditLimit = creditLimit;
        UserId = userId;
        ClosingDay = closingDay;
        DueDay = dueDay;
        Color = color; 
        CreatedAt = DateTime.UtcNow;
        IsDeleted = false;
    }

    public void Update(string name, decimal creditLimit, int closingDay, int dueDay, string color)
    {
        Name = name;
        CreditLimit = creditLimit;
        ClosingDay = closingDay;
        DueDay = dueDay;
        Color = color; 
        UpdatedAt = DateTime.UtcNow;
    }

    public void Delete()
    {
        IsDeleted = true;
        UpdatedAt = DateTime.UtcNow;
    }
}