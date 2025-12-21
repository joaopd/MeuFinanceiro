namespace Domain.Entities;

public class User : BaseEntity
{
    public string Name { get; private set; }
    public string Email { get; private set; }
    public Guid? ParentUserId { get; private set; }

    protected User() { }

    public User(string name, string email, Guid? parentUserId = null)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name is required", nameof(name));

        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("Email is required", nameof(email));

        Name = name;
        Email = email;
        ParentUserId = parentUserId;
    }

    public void UpdateName(string name, Guid updatedBy)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name is required");

        Name = name;
        Touch(updatedBy);
    }

    public void UpdateEmail(string email, Guid updatedBy)
    {
        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("Email is required");

        Email = email;
        Touch(updatedBy);
    }
}