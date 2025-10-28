using EWorldCup.Api.Data;
using EWorldCup.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace EWorldCup.Api.Repositories
{
    public class ParticipantRepository : IParticipantRepository
    {
        private readonly AppDbContext _db;
        public ParticipantRepository(AppDbContext db) => _db = db;

        public async Task<IReadOnlyList<Participant>> GetAllAsync(CancellationToken ct = default)
        {
            return await _db.Participants
                .AsNoTracking()
                .OrderBy(p => p.Id)
                .Select(p => new Participant { Id = p.Id, Name = p.Name , Uid = p.Uid})
                .ToListAsync(ct);
        }

        public Task<int> CountAsync(CancellationToken ct = default)
        {
            return _db.Participants.AsNoTracking().CountAsync(ct);
        }


        public async Task<Participant> GetByIndexAsync(int index, CancellationToken ct = default)
        {
            if (index < 0) return null;

            return await _db.Participants
                .AsNoTracking()
                .OrderBy(p => p.Id)
                .Skip(index)
                .Take(1)
                .Select(p => new Participant { Id = p.Id, Name = p.Name, Uid = p.Uid })
                .FirstOrDefaultAsync(ct);
        }
    }
}
