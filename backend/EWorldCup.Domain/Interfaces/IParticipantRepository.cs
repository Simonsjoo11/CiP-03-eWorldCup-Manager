namespace EWorldCup.Domain.Interfaces
{
    public interface IParticipantRepository
    {
        /// <summary>
        /// Retrieves all participants in the tournament
        /// </summary>
        /// <returns>Read-only list of all participants</returns>
        Task<IReadOnlyList<Entities.Participant>> GetAllAsync(CancellationToken ct = default);

        /// <summary>
        /// Gets a participant by their index
        /// </summary>
        /// <returns>The participant if found, null otherwise</returns>
        Task<Entities.Participant?> GetByIndexAsync(int index, CancellationToken ct = default);

        /// <summary>
        /// Gets the total count of participants in the tournament
        /// </summary>
        /// <returns>Total number of participants</returns>
        Task<int> GetCountAsync(CancellationToken ct = default);

        /// <summary>
        /// Adds a new participant
        /// </summary>
        /// <returns>The added participant with generated ID</returns>
        Task<Entities.Participant> AddAsync(Entities.Participant participant, CancellationToken ct = default);

        /// <summary>
        /// Deletes a participant from the tournament by their ID
        /// </summary>
        /// <returns>True if participant was deleted, false if not found</returns>
        Task<bool> DeleteAsync(CancellationToken ct = default);
    }
}
