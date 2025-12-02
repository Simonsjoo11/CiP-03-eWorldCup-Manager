using EWorldCup.Application.Interfaces;
using EWorldCup.Domain.Interfaces;
using EWorldCup.Infrastructure.Data;
using EWorldCup.Infrastructure.Repositories;
using EWorldCup.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();

// Configure Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "eWorldCup Manager API",
        Version = "v1",
        Description = "API for managing round-robin tournament schedules"
    });
});

// Configure Database - Use SQL Server
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        sqlOptions => sqlOptions.EnableRetryOnFailure(
            maxRetryCount: 5,
            maxRetryDelay: TimeSpan.FromSeconds(30),
            errorNumbersToAdd: null)
    ));

// Register Repository
builder.Services.AddScoped<IPlayerRepository, PlayerRepository>();

// Register Services
builder.Services.AddScoped<IPlayerService, PlayerService>();
builder.Services.AddScoped<IRoundSchedulingService, RoundSchedulingService>();
builder.Services.AddScoped<ITournamentQueryService, TournamentQueryService>();
builder.Services.AddScoped<ITournamentService, TournamentService>();
builder.Services.AddScoped<IRpsGameService, RpsGameService>();

// Register Seeder
builder.Services.AddScoped<PlayerSeeder>();

// Configure CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var app = builder.Build();

// Apply migrations and seed database
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<ApplicationDbContext>();
        var logger = services.GetRequiredService<ILogger<Program>>();

        logger.LogInformation("Applying database migrations...");
        
        // Use Migrate() instead of EnsureCreated() for proper migrations
        await context.Database.MigrateAsync();
        
        logger.LogInformation("Migrations applied successfully");

        // Seed data
        var seeder = services.GetRequiredService<PlayerSeeder>();
        await seeder.SeedAsync();

        logger.LogInformation("Database seeded successfully");
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while initializing the database");
        throw;
    }
}

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "eWorldCup Manager API v1");
        c.RoutePrefix = string.Empty;
    });
}

//app.UseHttpsRedirection();

app.UseCors("AllowFrontend");

app.UseAuthorization();

app.MapControllers();

app.Run();
