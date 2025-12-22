using Application.Shared.Dtos;
using FluentResults;

namespace Application.Services.User.CreateUser;

public interface ICreateUserService
{
    Task<Result<UserResponseDto>> ExecuteAsync(
        CreateUserRequestDto request);
}