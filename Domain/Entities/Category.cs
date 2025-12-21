using Domain.Enums;

namespace Domain.Entities;

public class Category : BaseEntity
{
    public string Name { get; private set; } = default!;
    public ExpenseType ExpenseType { get; private set; }

    protected Category() { }

    public Category(string name, ExpenseType expenseType)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Category name is required");

        Name = name;
        ExpenseType = expenseType;
    }

    public void UpdateName(string name, Guid updatedBy)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Category name is required");

        Name = name;
        Touch(updatedBy);
    }

    public void ChangeExpenseType(ExpenseType expenseType, Guid updatedBy)
    {
        ExpenseType = expenseType;
        Touch(updatedBy);
    }
}