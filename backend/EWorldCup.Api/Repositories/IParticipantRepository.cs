using EWorldCup.Api.Models;

namespace EWorldCup.Api.Repositories
{
    public interface IParticipantRepository
    {
        Task<IReadOnlyList<Participant>> GetAllAsync(CancellationToken ct = default);
        Task<int> CountAsync(CancellationToken ct = default);
        Task<Participant?> GetByIndexAsync(int index, CancellationToken ct = default);
    }
}
