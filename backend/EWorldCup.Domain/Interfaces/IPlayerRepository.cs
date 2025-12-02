using EWorldCup.Domain.Entities;

namespace EWorldCup.Domain.Interfaces
{
    public interface IPlayerRepository
    {
        /// <summary>
        /// Retrieves all players in the tournament
        /// </summary>
        /// <returns>Read-only list of all players</returns>
        Task<IReadOnlyList<Entities.Player>> GetAllAsync(CancellationToken ct = default);

        /// <summary>
        /// Gets a player by their index
        /// </summary>
        /// <returns>The player if found, null otherwise</returns>
        Task<Entities.Player?> GetByIndexAsync(int index, CancellationToken ct = default);

        /// <summary>
        /// Gets a player by their Uid
        /// </summary>
        /// <returns>The player if found, null otherwise</returns>
        Task<Entities.Player?> GetByUidAsync(Guid uid, CancellationToken ct = default);

        /// <summary>
        /// Gets the total count of players in the tournament
        /// </summary>
        /// <returns>Total number of players</returns>
        Task<int> GetCountAsync(CancellationToken ct = default);

        /// <summary>
        /// Adds a new player
        /// </summary>
        /// <returns>The added player with generated ID</returns>
        Task<Entities.Player> AddAsync(Entities.Player player, CancellationToken ct = default);

        /// <summary>
        /// Deletes a player from the tournament by their ID
        /// </summary>
        /// <param name="id">The ID of the player to delete</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns>True if player was deleted, false if not found</returns>
        Task<bool> DeleteAsync(Player player, CancellationToken ct = default);
    }
}
