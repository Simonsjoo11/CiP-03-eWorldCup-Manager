using EWorldCup.Application.Interfaces;
using EWorldCup.Domain.Entities;
using EWorldCup.Domain.Interfaces;
using EWorldCup.Infrastructure.Repositories;


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

        public async Task<Player> AddPlayerAsync(string name, CancellationToken ct = default)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("Player name cannot be empty", nameof(name));
            }

            var player = new Player
            {
                Name = name.Trim(),
                Uid = Guid.NewGuid()
            };

            return await _repository.AddAsync(player, ct);
        }

        public async Task<bool> RemovePlayerByUidAsync(Guid uid, CancellationToken ct = default)
        {
            var player = await _repository.GetByUidAsync(uid, ct);

            if (player == null)
            {
                return false;
            }

            await _repository.DeleteAsync(player, ct);
            return true;
        }
    }
}
