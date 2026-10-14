
using Microsoft.AspNetCore.Mvc;
using StockPilot.Application.Dashboard.DTOs;
using StockPilot.Application.Dashboard.Interfaces;
using Microsoft.AspNetCore.Authorization;

namespace StockPilot.API.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class DashboardController : ControllerBase
{
    private readonly IDashboardService _service;

    public DashboardController(IDashboardService service)
    {
        _service = service;
    }

    [HttpGet("summary")]
    public async Task<ActionResult<DashboardDto>> GetSummary(
        CancellationToken cancellationToken)
    {
        var summary = await _service.GetSummaryAsync(cancellationToken);
        return Ok(summary);
    }
}
