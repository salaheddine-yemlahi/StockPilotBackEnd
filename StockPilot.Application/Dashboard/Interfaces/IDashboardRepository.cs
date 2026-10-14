
using StockPilot.Application.Dashboard.DTOs;

namespace StockPilot.Application.Dashboard.Interfaces;

public interface IDashboardRepository
{
    Task<DashboardDto> GetSummaryAsync(
        CancellationToken cancellationToken = default);
}
