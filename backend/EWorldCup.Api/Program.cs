using EWorldCup.Api.Data;
using EWorldCup.Api.Repositories;
using EWorldCup.Api.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddAuthorization();

builder.Services.AddDbContext<AppDbContext>(opt =>
{
    var cs = builder.Configuration.GetConnectionString("Default") ?? "Data Source=eworldcup.db";
    opt.UseSqlite(cs);
});

builder.Services.AddScoped<DataSeeder>();

const string Frontend = "Frontend";
builder.Services.AddCors(o =>
{
  o.AddPolicy(Frontend, p =>
  {
    p.WithOrigins("http://localhost:3000")
     .AllowAnyHeader()
     .AllowAnyMethod();
  });
});

// Register repositories
builder.Services.AddScoped<IParticipantRepository, ParticipantRepository>();
builder.Services.AddScoped<IRoundRepository, InMemoryRoundRepository>();

// Register app service
builder.Services.AddScoped<ITournamentService, TournamentService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

using (var scope = app.Services.CreateScope())
{
    var seeder = scope.ServiceProvider.GetRequiredService<DataSeeder>();
    await seeder.SeedAsync();
}

app.UseHttpsRedirection();
app.UseCors(Frontend);
app.UseAuthorization();

app.MapControllers();
app.Run();
