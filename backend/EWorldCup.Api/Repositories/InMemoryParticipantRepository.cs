using System.Text.Json;
using EWorldCup.Api.Models;


namespace EWorldCup.Api.Repositories
{
    public class InMemoryParticipantRepository : IParticipantRepository
    {
        private readonly List<Participant> _items;

        public InMemoryParticipantRepository()
        {
            // Load once at startup from Data/participants.json
            var path = Path.Combine(AppContext.BaseDirectory, "Data", "participants.json");
            var json = File.Exists(path) ? File.ReadAllText(path) : "[]";
            _items = JsonSerializer.Deserialize<List<Participant>>(
                json,
                new JsonSerializerOptions {  PropertyNameCaseInsensitive = true }
                ) ?? new List<Participant>();
        }

        public Task<IReadOnlyList<Participant>> GetAllAsync(CancellationToken ct = default)
            => Task.FromResult<IReadOnlyList<Participant>>(_items);

        public Task<int> CountAsync(CancellationToken ct = default)
            => Task.FromResult(_items.Count);

        public Task<Participant> GetByIndexAsync(int index, CancellationToken ct = default)
            => Task.FromResult(index >= 0 && index < _items.Count ? _items[index] : null);
    }
}
