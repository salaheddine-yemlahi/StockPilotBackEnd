
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using StockPilot.Domain.Entities;
using StockPilot.Domain.Entities.Identity;

namespace StockPilot.Infrastructure.Persistence;

public class StockPilotDbContext
    : IdentityDbContext<ApplicationUser>
{
    public StockPilotDbContext(
        DbContextOptions<StockPilotDbContext> options)
        : base(options)
    {
    }

    public DbSet<Product> Products => Set<Product>();

    public DbSet<Category> Categories => Set<Category>();

    public DbSet<StockMovement> StockMovements => Set<StockMovement>();
}
