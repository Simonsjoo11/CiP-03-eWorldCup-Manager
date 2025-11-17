using EWorldCup.Domain.Entities;
using EWorldCup.Domain.Interfaces;
using EWorldCup.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Numerics;

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

        public async Task<Player?> GetByUidAsync(Guid uid, CancellationToken ct = default)
        {
            return await _context.Players.FirstOrDefaultAsync(p => p.Uid == uid, ct);
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
            await _context.Players.AddAsync(player, ct);
            await _context.SaveChangesAsync(ct);
            return player;
        }

        /// <summary>
        /// Deletes a player
        /// </summary>
        public async Task<bool> DeleteAsync(Player player, CancellationToken ct = default)
        {
            _context.Players.Remove(player);
            var result = await _context.SaveChangesAsync(ct);
            return result > 0;
        }
    }
}
