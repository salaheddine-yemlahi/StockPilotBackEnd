
using System.ComponentModel.DataAnnotations;

namespace StockPilot.Application.Categories.DTOs;

public class CreateCategoryDto
{
    [Required]
    [StringLength(100, MinimumLength = 2)]
    public string Name { get; set; } = string.Empty;

    [StringLength(500)]
    public string? Description { get; set; }
}
