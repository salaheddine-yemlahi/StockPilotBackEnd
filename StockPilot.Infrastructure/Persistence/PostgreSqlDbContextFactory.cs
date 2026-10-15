
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace StockPilot.Infrastructure.Persistence;

public class PostgreSqlDbContextFactory
    : IDesignTimeDbContextFactory<PostgreSqlDbContext>
{
    public PostgreSqlDbContext CreateDbContext(string[] args)
    {
        var connectionString =
            Environment.GetEnvironmentVariable(
                "ConnectionStrings__DefaultConnection");

        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException(
                "La variable ConnectionStrings__DefaultConnection est absente.");
        }

        var optionsBuilder =
            new DbContextOptionsBuilder<PostgreSqlDbContext>();

        optionsBuilder.UseNpgsql(
            connectionString,
            options => options.MigrationsHistoryTable(
                "__EFMigrationsHistory_PostgreSQL"));

        return new PostgreSqlDbContext(optionsBuilder.Options);
    }
}
