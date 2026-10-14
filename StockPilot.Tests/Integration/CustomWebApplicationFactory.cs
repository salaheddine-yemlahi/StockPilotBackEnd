
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using StockPilot.Infrastructure.Persistence;

namespace StockPilot.Tests.Integration;

public class CustomWebApplicationFactory
    : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            // Supprimer la configuration SQL Server existante.
            services.RemoveAll<DbContextOptions<StockPilotDbContext>>();
            services.RemoveAll<StockPilotDbContext>();
            services.RemoveAll<IDbContextOptionsConfiguration<StockPilotDbContext>>();

            // Utiliser uniquement InMemory pour les tests.
            services.AddDbContext<StockPilotDbContext>(options =>
                options.UseInMemoryDatabase(
                    $"StockPilotIntegrationTests_{Guid.NewGuid()}"));
        });
    }
}
