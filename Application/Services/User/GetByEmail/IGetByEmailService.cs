using Application.Shared.Dtos;
using FluentResults;

namespace Application.Services.User.GetByEmail;

public interface IGetByEmailService
{
    Task<Result<UserResponseDto>> ExecuteAsync(string email);
}