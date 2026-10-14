
using StockPilot.Application.Categories.DTOs;
using StockPilot.Application.Categories.Interfaces;
using StockPilot.Domain.Entities;

namespace StockPilot.Application.Categories;

public class CategoryService : ICategoryService
{
    private readonly ICategoryRepository _repository;

    public CategoryService(ICategoryRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<CategoryDto>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        var categories = await _repository.GetAllAsync(cancellationToken);

        return categories.Select(MapToDto).ToList();
    }

    public async Task<CategoryDto?> GetByIdAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        var category = await _repository.GetByIdAsync(
            id, cancellationToken);

        return category is null ? null : MapToDto(category);
    }

    public async Task<CategoryDto> CreateAsync(
        CreateCategoryDto dto,
        CancellationToken cancellationToken = default)
    {
        var name = dto.Name.Trim();

        if (await _repository.ExistsByNameAsync(name, cancellationToken))
        {
            throw new InvalidOperationException(
                $"Une catégorie nommée '{name}' existe déjà.");
        }

        var category = new Category
        {
            Name = name,
            Description = dto.Description?.Trim()
        };

        await _repository.AddAsync(category, cancellationToken);
        await _repository.SaveChangesAsync(cancellationToken);

        return MapToDto(category);
    }

    public async Task<CategoryDto?> UpdateAsync(
        int id,
        UpdateCategoryDto dto,
        CancellationToken cancellationToken = default)
    {
        var category = await _repository.GetByIdAsync(
            id, cancellationToken);

        if (category is null)
            return null;

        var name = dto.Name.Trim();

        if (!string.Equals(
                category.Name, name,
                StringComparison.OrdinalIgnoreCase) &&
            await _repository.ExistsByNameAsync(name, cancellationToken))
        {
            throw new InvalidOperationException(
                $"Une catégorie nommée '{name}' existe déjà.");
        }

        category.Name = name;
        category.Description = dto.Description?.Trim();

        await _repository.UpdateAsync(category, cancellationToken);
        await _repository.SaveChangesAsync(cancellationToken);

        return MapToDto(category);
    }

    public async Task<bool> DeleteAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        var category = await _repository.GetByIdAsync(
            id, cancellationToken);

        if (category is null)
            return false;

        if (category.Products.Count > 0)
        {
            throw new InvalidOperationException(
                "Impossible de supprimer une catégorie qui contient des produits.");
        }

        await _repository.DeleteAsync(category, cancellationToken);
        await _repository.SaveChangesAsync(cancellationToken);

        return true;
    }

    private static CategoryDto MapToDto(Category category)
    {
        return new CategoryDto
        {
            Id = category.Id,
            Name = category.Name,
            Description = category.Description,
            CreatedAt = category.CreatedAt,
            ProductCount = category.Products.Count
        };
    }
}
