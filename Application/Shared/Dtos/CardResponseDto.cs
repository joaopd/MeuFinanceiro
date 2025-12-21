using Domain.Enums;

namespace Application.Shared.Dtos;

public class CardResponseDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = default!;
    public PaymentMethod PaymentMethod { get; set; }
    public decimal? CreditLimit { get; set; }
}