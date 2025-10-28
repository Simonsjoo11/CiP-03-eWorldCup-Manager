using EWorldCup.Api.Models;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace EWorldCup.Api.Data
{
    public class DataSeeder
    {
        private readonly AppDbContext _db;
        private readonly IWebHostEnvironment _env;
        private readonly ILogger<DataSeeder> _log;

        public DataSeeder(AppDbContext db, IWebHostEnvironment env, ILogger<DataSeeder> log)
        {
            _db = db;
            _env = env;
            _log = log;
        }

        public async Task SeedAsync(CancellationToken ct = default)
        {
            // Ensure DB and tables exists
            await _db.Database.MigrateAsync(ct);

            // Only seed if database is empty
            if (await _db.Participants.AnyAsync(ct))
            {
                _log.LogInformation("Participants already preset, skipping seed");
                return;
            }

            // Locate participants.json
            var path = Path.Combine(AppContext.BaseDirectory, "Data", "participants.json");
            if (!File.Exists(path))
            {
                _log.LogWarning("Seed file not found at {Path}. Skipping seed.", path);
                return;
            }

            var json = await File.ReadAllTextAsync(path, ct);
            var items = JsonSerializer.Deserialize<List<SeedParticipant>>(json) ?? new();

            var entities = items.Select(x => new Participant
            {
                Id = x.id,
                Name = x.name,
            });

            await _db.Participants.AddRangeAsync(entities, ct);
            await _db.SaveChangesAsync(ct);

            _log.LogInformation("Seeded {Count} participants", items.Count);
        }

        private record SeedParticipant(int id, string name);
    }
}
