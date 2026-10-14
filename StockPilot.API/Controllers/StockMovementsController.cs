
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StockPilot.Application.StockMovements.DTOs;
using StockPilot.Application.StockMovements.Interfaces;

namespace StockPilot.API.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class StockMovementsController : ControllerBase
{
    private readonly IStockMovementService _service;

    public StockMovementsController(IStockMovementService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<ActionResult<List<StockMovementDto>>> GetAll(
        CancellationToken cancellationToken)
    {
        var movements = await _service.GetAllAsync(cancellationToken);

        return Ok(movements);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<StockMovementDto>> GetById(
        int id,
        CancellationToken cancellationToken)
    {
        var movement = await _service.GetByIdAsync(
            id,
            cancellationToken);

        if (movement is null)
        {
            return NotFound(new
            {
                message = $"Le mouvement {id} est introuvable."
            });
        }

        return Ok(movement);
    }

    [HttpPost]
    public async Task<ActionResult<StockMovementDto>> Create(
        [FromBody] CreateStockMovementDto dto,
        CancellationToken cancellationToken)
    {
        try
        {
            var movement = await _service.CreateAsync(
                dto,
                cancellationToken);

            return CreatedAtAction(
                nameof(GetById),
                new { id = movement.Id },
                movement);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message });
        }
    }
}
