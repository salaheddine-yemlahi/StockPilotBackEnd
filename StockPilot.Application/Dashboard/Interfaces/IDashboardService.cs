
using StockPilot.Application.Dashboard.DTOs;

namespace StockPilot.Application.Dashboard.Interfaces;

public interface IDashboardService
{
    Task<DashboardDto> GetSummaryAsync(
        CancellationToken cancellationToken = default);
}
