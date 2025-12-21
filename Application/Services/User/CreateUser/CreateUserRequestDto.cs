namespace Application.Services.User.CreateUser;

public class CreateUserRequestDto
{
    public string Name { get; set; } = default!;
    public string Email { get; set; } = default!;
    public Guid? ParentUserId { get; set; }
}