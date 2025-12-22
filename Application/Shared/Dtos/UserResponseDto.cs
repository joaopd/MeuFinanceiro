namespace Application.Shared.Dtos;

public class UserResponseDto
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public Guid? ParentUserId { get; set; }

    public List<UserResponseDto> Dependents { get; set; } = new(); 
}