using Application.Shared.Dtos;
using FluentResults;

namespace Application.Services.User.GetDependents;

public interface IGetDependentsService
{
    Task<Result<List<UserResponseDto>>> ExecuteAsync(Guid userId);
}