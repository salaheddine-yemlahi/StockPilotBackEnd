
using Microsoft.EntityFrameworkCore;
using StockPilot.Application.Products;
using StockPilot.Application.Products.Interfaces;
using StockPilot.Infrastructure.Persistence;
using StockPilot.Infrastructure.Repositories;
using StockPilot.Application.Categories;
using StockPilot.Application.Categories.Interfaces;
using StockPilot.Application.StockMovements;
using StockPilot.Application.StockMovements.Interfaces;
using StockPilot.Application.Dashboard;
using StockPilot.Application.Dashboard.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// Controllers
builder.Services.AddControllers();

// Swagger / OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddOpenApi();

// Standardized error responses
builder.Services.AddProblemDetails();

// Database
builder.Services.AddDbContext<StockPilotDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")));

// Dependency Injection
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<IStockMovementRepository, StockMovementRepository>();
builder.Services.AddScoped<IStockMovementService, StockMovementService>();
builder.Services.AddScoped<IDashboardRepository, DashboardRepository>();
builder.Services.AddScoped<IDashboardService, DashboardService>();

var app = builder.Build();

// Global exception handling
app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

public partial class Program { }