using EWorldCup.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace EWorldCup.Infrastructure.Data
{
    public class PlayerSeeder
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<PlayerSeeder> _logger;

        public PlayerSeeder(ApplicationDbContext context, ILogger<PlayerSeeder> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task SeedAsync()
        {
            if (await _context.Players.AnyAsync())
            {
                _logger.LogInformation("Players already exist in the database. Seeding skipped.");
                return;
            }

            _logger.LogInformation("Seeding players from JSON file...");

            // Read JSON file
            var jsonPath = Path.Combine(AppContext.BaseDirectory, "Data", "players.json");

            if (!File.Exists(jsonPath))
            {
                _logger.LogWarning("players.json not found at {Path}", jsonPath);
                return;
            }

            var jsonContent = await File.ReadAllTextAsync(jsonPath);
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
            var playerData = JsonSerializer.Deserialize<List<PlayerSeedData>>(jsonContent, options);

            if (playerData == null || !playerData.Any())
            {
                _logger.LogWarning("No player data found in JSON file");
                return;
            }

            // Create players with auto-generated Guids
            var players = playerData.Select(p => new Player
            {
                Name = p.Name,
                Uid = Guid.NewGuid() // Auto-generate Guid for each player
            }).ToList();

            // Add to database
            await _context.Players.AddRangeAsync(players);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Successfully seeded {Count} players", players.Count);
        }

        private class PlayerSeedData
        {
            public int Id { get; set; }
            public string Name { get; set; } = string.Empty;
        }
    }
}
