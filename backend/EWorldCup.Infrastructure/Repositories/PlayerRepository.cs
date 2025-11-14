using EWorldCup.Domain.Entities;
using EWorldCup.Domain.Interfaces;
using EWorldCup.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace EWorldCup.Infrastructure.Repositories
{
    public class PlayerRepository : IPlayerRepository
    {
        private readonly ApplicationDbContext _context;

        public PlayerRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Retrieves all players in the tournament
        /// </summary>
        public async Task<IReadOnlyList<Player>> GetAllAsync(CancellationToken ct = default)
        {
            return await _context.Players
                .OrderBy(p => p.Id)
                .ToListAsync(ct);
        }

        /// <summary>
        /// Gets a player by their 0-based index
        /// </summary>
        public async Task<Player?> GetByIndexAsync(int index, CancellationToken ct = default)
        {
            return await _context.Players
                .OrderBy(p => p.Id)
                .Skip(index)
                .FirstOrDefaultAsync(ct);
        }

        /// <summary>
        /// Gets the total count of players
        /// </summary>
        public async Task<int> GetCountAsync(CancellationToken ct = default)
        {
            return await _context.Players.CountAsync(ct);
        }

        /// <summary>
        /// Adds a new player to the database
        /// </summary>
        public async Task<Player> AddAsync(Player player, CancellationToken ct = default)
        {
            if (player.Uid == Guid.Empty)
            {
                player.Uid = Guid.NewGuid();
            }

            _context.Players.Add(player);
            await _context.SaveChangesAsync(ct);

            return player;
        }

        /// <summary>
        /// Deletes a player by their ID
        /// </summary>
        public async Task<bool> DeleteAsync(int id, CancellationToken ct = default)
        {
            var player = await _context.Players.FindAsync([id], ct);

            if (player == null)
            {
                return false;
            }

            _context.Players.Remove(player);
            await _context.SaveChangesAsync(ct);

            return true;
        }
    }
}
