namespace Application.Services.Transaction.ImportInvoice;

public class AIInvoiceItemDto
{
    public string Description { get; set; } = default!;
    public decimal Amount { get; set; }
    public DateTime TransactionDate { get; set; }
    public string? CategoryId { get; set; } 
    public string? CardId { get; set; } 
    public string? NewCategoryName { get; set; }
}