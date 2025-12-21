using FluentResults;

namespace Application.Services.Dashboard.GetDashboardService;

public interface IGetDashboardService
{
    Task<Result<GetDashboardResponse>> ExecuteAsync(
        Guid userId, 
        DateTime startDate, 
        DateTime endDate);
}