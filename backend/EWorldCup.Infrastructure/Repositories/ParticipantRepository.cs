using EWorldCup.Domain.Entities;
using EWorldCup.Domain.Interfaces;
using EWorldCup.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace EWorldCup.Infrastructure.Repositories
{
    public class ParticipantRepository : IParticipantRepository
    {
        private readonly ApplicationDbContext _context;

        public ParticipantRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Retrieves all participants in the tournament
        /// </summary>
        public async Task<IReadOnlyList<Participant>> GetAllAsync(CancellationToken ct = default)
        {
            return await _context.Participants
                .OrderBy(p => p.Id)
                .ToListAsync(ct);
        }

        /// <summary>
        /// Gets a participant by their 0-based index
        /// </summary>
        public async Task<Participant?> GetByIndexAsync(int index, CancellationToken ct = default)
        {
            return await _context.Participants
                .OrderBy(p => p.Id)
                .Skip(index)
                .FirstOrDefaultAsync(ct);
        }

        /// <summary>
        /// Gets the total count of participants
        /// </summary>
        public async Task<int> GetCountAsync(CancellationToken ct = default)
        {
            return await _context.Participants.CountAsync(ct);
        }

        /// <summary>
        /// Adds a new participant to the database
        /// </summary>
        public async Task<Participant> AddAsync(Participant participant, CancellationToken ct = default)
        {
            if (participant.Uid == Guid.Empty)
            {
                participant.Uid = Guid.NewGuid();
            }

            _context.Participants.Add(participant);
            await _context.SaveChangesAsync(ct);

            return participant;
        }

        /// <summary>
        /// Deletes a participant by their ID
        /// </summary>
        public async Task<bool> DeleteAsync(int id, CancellationToken ct = default)
        {
            var participant = await _context.Participants.FindAsync([id], ct);

            if (participant == null)
            {
                return false;
            }

            _context.Participants.Remove(participant);
            await _context.SaveChangesAsync(ct);

            return true;
        }
    }
}
