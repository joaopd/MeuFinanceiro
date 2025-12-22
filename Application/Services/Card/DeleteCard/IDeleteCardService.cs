using FluentResults;

namespace Application.Services.Card.DeleteCard;

public interface IDeleteCardService
{
    Task<Result> ExecuteAsync(Guid id);
}