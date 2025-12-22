using Application.Shared.Dtos;
using Application.Shared.Mappers;
using Domain.Abstractions.ErrorHandling;
using Domain.InterfaceRepository;
using FluentResults;
using Microsoft.Extensions.Logging;

namespace Application.Services.User.GetByEmail;

public class GetByEmailService : IGetByEmailService
{
    private readonly IUserRepository _userRepository;
    private readonly ILogger<GetByEmailService> _logger;

    public GetByEmailService(
        IUserRepository userRepository,
        ILogger<GetByEmailService> logger)
    {
        _userRepository = userRepository;
        _logger = logger;
    }

    public async Task<Result<UserResponseDto>> ExecuteAsync(string email)
    {
        try
        {
            var user = await _userRepository.GetByEmailAsync(email);
            if (user is null)
                return Result.Fail(FinanceErrorMessage.UserNotFound);
            
            return Result.Ok(user.ToDto());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting dependents for user {Email}", email);
            return Result.Fail(FinanceErrorMessage.DatabaseError);
        }
    }
}