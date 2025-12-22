using Application.Shared.Dtos;
using Application.Shared.Mappers;
using Domain.Abstractions.ErrorHandling;
using Domain.Entities;
using Domain.InterfaceRepository;
using FluentResults;
using Microsoft.Extensions.Logging;

namespace Application.Services.User.CreateUser;

public class CreateUserService : ICreateUserService
{
    private readonly IUserRepository _userRepository;
    private readonly ILogger<CreateUserService> _logger;

    public CreateUserService(
        IUserRepository userRepository,
        ILogger<CreateUserService> logger)
    {
        _userRepository = userRepository;
        _logger = logger;
    }

    public async Task<Result<UserResponseDto>> ExecuteAsync(
        CreateUserRequestDto request)
    {
        try
        {
            _logger.LogInformation(
                "CreateUser started - Name: {Name}, Email: {Email}",
                request.Name,
                request.Email);

            if (string.IsNullOrWhiteSpace(request.Name) ||
                string.IsNullOrWhiteSpace(request.Email))
            {
                return Result.Fail(
                    FinanceErrorMessage.InvalidUserData);
            }

            var user = new Domain.Entities.User(
                request.Name,
                request.Email,
                request.ParentUserId);

            var userId = await _userRepository.InsertAsync(user);

            _logger.LogInformation(
                "CreateUser finished successfully - UserId: {UserId}",
                userId);

            return Result.Ok(user.ToDto());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while creating user");

            return Result.Fail(
                FinanceErrorMessage.DatabaseError);
        }
    }
}