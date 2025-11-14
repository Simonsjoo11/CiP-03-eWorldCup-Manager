using EWorldCup.Application.Interfaces;
using EWorldCup.Domain.Entities;
using EWorldCup.Domain.Interfaces;


namespace EWorldCup.Infrastructure.Services
{
    public class PlayerService : IPlayerService
    {
        private readonly IPlayerRepository _repository;

        public PlayerService(IPlayerRepository repository)
        {
            _repository = repository;
        }

        public async Task<IReadOnlyList<Player>> GetAllAsync(CancellationToken ct)
        {
            return await _repository.GetAllAsync(ct);
        }

        public async Task<int> GetCountAsync(CancellationToken ct)
        {
            return await _repository.GetCountAsync(ct);
        }

        public async Task<Player?> GetByIndexAsync(int index, CancellationToken ct)
        {
            if (index < 0)
                throw new ArgumentException("Index must be non-negative.", nameof(index));

            return await _repository.GetByIndexAsync(index, ct);
        }

    }
}
