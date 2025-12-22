using Application.Shared.Dtos;
using Application.Shared.Mappers;
using Domain.Abstractions.ErrorHandling;
using Domain.InterfaceRepository;
using FluentResults;
using Microsoft.Extensions.Logging;

namespace Application.Services.User.GetDependents;

public class GetDependentsService : IGetDependentsService
{
    private readonly IUserRepository _userRepository;
    private readonly ILogger<GetDependentsService> _logger;

    public GetDependentsService(
        IUserRepository userRepository,
        ILogger<GetDependentsService> logger)
    {
        _userRepository = userRepository;
        _logger = logger;
    }

    public async Task<Result<List<UserResponseDto>>> ExecuteAsync(Guid userId)
    {
        try
        {
            var parentUser = await _userRepository.GetByIdAsync(userId);
            if (parentUser is null)
            {
                return Result.Fail(FinanceErrorMessage.UserNotFound);
            }
            
            var dependentsEntities = await _userRepository.GetByParentIdAsync(userId);
            
            var dependentsDtos = dependentsEntities
                .Select(user => user.ToDto()) 
                .ToList();
            
            return Result.Ok(dependentsDtos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting dependents for user {Id}", userId);
            return Result.Fail(FinanceErrorMessage.DatabaseError);
        }
    }
}