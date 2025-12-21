namespace Domain.Entities;

public abstract class BaseEntity
{
    public Guid Id { get; protected set; }
    public DateTime CreatedAt { get; protected set; }
    public DateTime UpdatedAt { get; protected set; }
    public Guid? CreatedBy { get; protected set; }
    public Guid? UpdatedBy { get; protected set; }
    public bool IsDeleted { get; protected set; }

    protected BaseEntity()
    {
        Id = Guid.NewGuid();
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
        IsDeleted = false;
    }

    public void MarkAsDeleted(Guid userId)
    {
        IsDeleted = true;
        UpdatedBy = userId;
        UpdatedAt = DateTime.UtcNow;
    }

    protected void Touch(Guid? userId = null)
    {
        UpdatedAt = DateTime.UtcNow;
        UpdatedBy = userId;
    }
}
