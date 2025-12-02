using EWorldCup.Domain.Entities;

namespace EWorldCup.Application.Interfaces
{
    public interface IPlayerService
    {
        /// <summary>
        /// Gets all players in the tournament
        /// </summary>
        Task<IReadOnlyList<Player>> GetAllAsync(CancellationToken ct = default);
        
        /// <summary>
        /// Gets the total count of players
        /// </summary>
        Task<int> GetCountAsync(CancellationToken ct = default);

        /// <summary>
        /// Gets a player by their index
        /// </summary>
        Task<Player?> GetByIndexAsync(int index, CancellationToken ct = default);

        Task<Player> AddPlayerAsync(string name, CancellationToken ct = default);
        Task<bool> RemovePlayerByUidAsync(Guid uid, CancellationToken ct = default);
    }
}
