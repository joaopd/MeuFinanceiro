namespace Application.Services.Dashboard.GetDashboardService;

public class GetDashboardResponse
{
    public decimal Balance { get; set; }      
    public decimal TotalIncome { get; set; }  
    public decimal TotalExpense { get; set; } 
    public decimal TotalPaid { get; set; }    
    
    public List<ChartItem> ExpensesByCategory { get; set; } = [];
    public List<ChartItem> IncomesAndExpenses { get; set; } = [];
}

public class ChartItem
{
    public string Name { get; set; } = string.Empty; 
    public decimal Value { get; set; }
    public string Type { get; set; } = string.Empty;
}