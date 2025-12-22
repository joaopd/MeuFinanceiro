namespace Application.Services.Card.UpdateCard;

public record UpdateCardRequestDto(
    Guid Id,
    string Name,
    decimal CreditLimit,
    int ClosingDay,
    int DueDay,
    string Color 
);