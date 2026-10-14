
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using StockPilot.Infrastructure.Persistence;

namespace StockPilot.Tests.Integration;

public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    private readonly string _databaseName =
        $"StockPilotIntegrationTests_{Guid.NewGuid()}";

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            services.RemoveAll<DbContextOptions<StockPilotDbContext>>();
            services.RemoveAll<StockPilotDbContext>();
            services.RemoveAll<IDbContextOptionsConfiguration<StockPilotDbContext>>();

            services.AddDbContext<StockPilotDbContext>(options =>
                options.UseInMemoryDatabase(_databaseName));
        });
    }
}
