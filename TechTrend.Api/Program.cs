using Microsoft.EntityFrameworkCore;
using TechTrend.Api.Data;
using TechTrend.Api.Models;
using TechTrend.Api.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("Default")));

builder.Services.AddHttpClient<DataCollector>();
builder.Services.AddScoped<TrendCalculator>();
builder.Services.AddHostedService<UpdateWorker>();
builder.Services.AddScoped<TechnologyService>();

var app = builder.Build();

app.UseDefaultFiles();
app.UseStaticFiles();

app.MapGet("/api/health", () => new { status = "ok" });

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

    await db.Database.MigrateAsync();

    if (!await db.Technologies.AnyAsync())
    {
        db.Technologies.AddRange(
            new Technology
            {
                Name = "C#",
                Category = "Backend",
                Language = "C#"
            },
            new Technology
            {
                Name = "Java",
                Category = "Backend",
                Language = "Java"
            },
            new Technology
            {
                Name = "Python",
                Category = "Backend",
                Language = "Python"
            },
            new Technology
            {
                Name = "JavaScript",
                Category = "Frontend",
                Language = "JavaScript"
            },
            new Technology
            {
                Name = "Go",
                Category = "Backend",
                Language = "Go"
            },
            new Technology
            {
                Name = "Rust",
                Category = "Backend",
                Language = "Rust"
            }
        );

        await db.SaveChangesAsync();
    }
}

app.MapGet("/api/technologies", async (
    TechnologyService service,
    string? search,
    string? category,
    string? sort) =>
{
    return Results.Ok(
        await service.GetTechnologiesAsync(search, category, sort));
});

app.Run();