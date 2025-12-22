namespace Application.Services.Card.CreateCard;

public record CreateCardRequestDto(
    string Name,
    decimal CreditLimit,
    Guid UserId,
    int ClosingDay,
    int DueDay,
    string Color 
);