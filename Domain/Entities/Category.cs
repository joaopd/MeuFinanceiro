namespace Domain.Entities;

public class Category : BaseEntity
{
    public string Name { get; private set; } = default!;
    
    protected Category() { }

    public Category(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Category name is required");

        Name = name;
    }

    public void UpdateName(string name, Guid updatedBy)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Category name is required");

        Name = name;
        Touch(updatedBy);
    }
}