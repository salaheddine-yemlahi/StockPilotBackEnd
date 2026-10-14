
using Microsoft.EntityFrameworkCore;
using StockPilot.Domain.Entities;

namespace StockPilot.Infrastructure.Persistence;

public class StockPilotDbContext : DbContext
{
    public StockPilotDbContext(
        DbContextOptions<StockPilotDbContext> options)
        : base(options)
    {
    }

    public DbSet<Product> Products => Set<Product>();

    public DbSet<Category> Categories => Set<Category>();
}
