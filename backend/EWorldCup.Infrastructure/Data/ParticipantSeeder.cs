using EWorldCup.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace EWorldCup.Infrastructure.Data
{
    public class ParticipantSeeder
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<ParticipantSeeder> _logger;

        public ParticipantSeeder(ApplicationDbContext context, ILogger<ParticipantSeeder> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task SeedAsync()
        {
            if (await _context.Participants.AnyAsync())
            {
                _logger.LogInformation("Participants already exist in the database. Seeding skipped.");
                return;
            }

            _logger.LogInformation("Seeding participants from JSON file...");

            // Read JSON file
            var jsonPath = Path.Combine(AppContext.BaseDirectory, "Data", "participants.json");

            if (!File.Exists(jsonPath))
            {
                _logger.LogWarning("participants.json not found at {Path}", jsonPath);
                return;
            }

            var jsonContent = await File.ReadAllTextAsync(jsonPath);
            var participantData = JsonSerializer.Deserialize<List<ParticipantSeedData>>(jsonContent);

            if (participantData == null || !participantData.Any())
            {
                _logger.LogWarning("No participant data found in JSON file");
                return;
            }

            // Create participants with auto-generated Guids
            var participants = participantData.Select(p => new Participant
            {
                Id = p.Id,
                Name = p.Name,
                Uid = Guid.NewGuid() // Auto-generate Guid for each participant
            }).ToList();

            // Add to database
            await _context.Participants.AddRangeAsync(participants);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Successfully seeded {Count} participants", participants.Count);
        }

        private class ParticipantSeedData
        {
            public int Id { get; set; }
            public string Name { get; set; } = string.Empty;
        }
    }
}
