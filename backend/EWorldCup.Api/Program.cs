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

// Configure Database
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")
        ?? "Data Source=eworldcup.db"));

// Register Repository
builder.Services.AddScoped<IParticipantRepository, ParticipantRepository>();

// Register Services
builder.Services.AddScoped<IParticipantService, ParticipantService>();
builder.Services.AddScoped<IRoundSchedulingService, RoundSchedulingService>();
builder.Services.AddScoped<ITournamentQueryService, TournamentQueryService>();

// Register Seeder
builder.Services.AddScoped<ParticipantSeeder>();

// Configure CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins("http://localhost:3000")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var app = builder.Build();

// Seed the database
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<ApplicationDbContext>();
        var logger = services.GetRequiredService<ILogger<Program>>();

        context.Database.EnsureCreated();

        var seeder = services.GetRequiredService<ParticipantSeeder>();
        await seeder.SeedAsync();

        logger.LogInformation("Database initialized successfully");
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while seeding the database");
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

app.UseHttpsRedirection();

app.UseCors("AllowFrontend");

app.UseAuthorization();

app.MapControllers();

app.Run();
