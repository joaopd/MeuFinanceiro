using Application.Shared.Dtos;
using Domain.Entities;

namespace Application.Shared.Mappers;

public static class UserMapper
{
    public static UserResponseDto ToDto(this User user)
    {
        return new UserResponseDto
        {
            Id = user.Id,
            Name = user.Name,
            Email = user.Email,
            ParentUserId = user.ParentUserId
        };
    }
}