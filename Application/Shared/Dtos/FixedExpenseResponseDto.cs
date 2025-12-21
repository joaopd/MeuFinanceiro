namespace Application.Shared.Dtos;

using Domain.Enums;

public class FixedExpenseResponseDto
{
    public Guid Id { get; set; }
    public string Description { get; set; } = default!;
    public decimal Amount { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public RecurrenceType Recurrence { get; set; }
    public Guid CategoryId { get; set; }
    public Guid? CardId { get; set; }
}