
using StockPilot.Application.Dashboard.DTOs;
using StockPilot.Application.Dashboard.Interfaces;

namespace StockPilot.Application.Dashboard;

public class DashboardService : IDashboardService
{
    private readonly IDashboardRepository _repository;

    public DashboardService(IDashboardRepository repository)
    {
        _repository = repository;
    }

    public async Task<DashboardDto> GetSummaryAsync(
        CancellationToken cancellationToken = default)
    {
        return await _repository.GetSummaryAsync(cancellationToken);
    }
}
