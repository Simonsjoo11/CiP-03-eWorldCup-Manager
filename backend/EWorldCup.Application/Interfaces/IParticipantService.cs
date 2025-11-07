using EWorldCup.Domain.Entities;

namespace EWorldCup.Application.Interfaces
{
    public interface IParticipantService
    {
        /// <summary>
        /// Gets all participants in the tournament
        /// </summary>
        Task<IReadOnlyList<Participant>> GetAllAsync(CancellationToken ct = default);
        
        /// <summary>
        /// Gets the total count of participants
        /// </summary>
        Task<int> GetCountAsync(CancellationToken ct = default);

        /// <summary>
        /// Gets a participant by their index
        /// </summary>
        Task<Participant?> GetByIndexAsync(int index, CancellationToken ct = default);
    }
}
