using EWorldCup.Api.Repositories;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddAuthorization();

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
builder.Services.AddSingleton<IParticipantRepository, InMemoryParticipantRepository>();
builder.Services.AddSingleton<IRoundRepository, InMemoryRoundRepository>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors();
app.UseAuthorization();

app.MapControllers();
app.Run();
