
using Microsoft.AspNetCore.Mvc;
using StockPilot.Application.Categories.DTOs;
using StockPilot.Application.Categories.Interfaces;
using Microsoft.AspNetCore.Authorization;
namespace StockPilot.API.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class CategoriesController : ControllerBase
{
    private readonly ICategoryService _categoryService;

    public CategoriesController(ICategoryService categoryService)
    {
        _categoryService = categoryService;
    }

    [HttpGet]
    public async Task<ActionResult<List<CategoryDto>>> GetAll(
        CancellationToken cancellationToken)
    {
        var categories = await _categoryService.GetAllAsync(
            cancellationToken);

        return Ok(categories);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<CategoryDto>> GetById(
        int id,
        CancellationToken cancellationToken)
    {
        var category = await _categoryService.GetByIdAsync(
            id, cancellationToken);

        if (category is null)
        {
            return NotFound(new
            {
                message = $"Catégorie {id} introuvable."
            });
        }

        return Ok(category);
    }

    [HttpPost]
    public async Task<ActionResult<CategoryDto>> Create(
        [FromBody] CreateCategoryDto dto,
        CancellationToken cancellationToken)
    {
        try
        {
            var category = await _categoryService.CreateAsync(
                dto, cancellationToken);

            return CreatedAtAction(
                nameof(GetById),
                new { id = category.Id },
                category);
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message });
        }
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<CategoryDto>> Update(
        int id,
        [FromBody] UpdateCategoryDto dto,
        CancellationToken cancellationToken)
    {
        try
        {
            var category = await _categoryService.UpdateAsync(
                id, dto, cancellationToken);

            if (category is null)
            {
                return NotFound(new
                {
                    message = $"Catégorie {id} introuvable."
                });
            }

            return Ok(category);
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message });
        }
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(
        int id,
        CancellationToken cancellationToken)
    {
        try
        {
            var deleted = await _categoryService.DeleteAsync(
                id, cancellationToken);

            if (!deleted)
            {
                return NotFound(new
                {
                    message = $"Catégorie {id} introuvable."
                });
            }

            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message });
        }
    }
}
