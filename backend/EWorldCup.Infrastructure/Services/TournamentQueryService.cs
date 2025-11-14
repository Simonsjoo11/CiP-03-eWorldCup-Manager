using EWorldCup.Application.DTOs;
using EWorldCup.Application.Interfaces;
using EWorldCup.Application.Responses;

namespace EWorldCup.Infrastructure.Services
{
    public class TournamentQueryService : ITournamentQueryService
    {
        private readonly IPlayerService _playerService;
        private readonly IRoundSchedulingService _roundSchedulingService;

        public TournamentQueryService(
            IPlayerService playerService,
            IRoundSchedulingService roundSchedulingService)
        {
            _playerService = playerService;
            _roundSchedulingService = roundSchedulingService;
        }

        public async Task<PlayersResponse> GetPlayerAsync(CancellationToken ct = default)
        {
            var players = await _playerService.GetAllAsync(ct);
            var count = players.Count;

            return new PlayersResponse
            {
                PlayerCount = count,
                MaxRounds = _roundSchedulingService.GetMaxRounds(count),
                Players = players.Select(p => new PlayerDto
                {
                    Id = p.Id,
                    Name = p.Name
                }).ToList()
            };
        }

        public async Task<PlayerRoundResponse> GetPlayerInRoundAsync(int playerIndex, int round, CancellationToken ct = default)
        {
            var players = await _playerService.GetAllAsync(ct);
            var count = players.Count;
            var maxRounds = _roundSchedulingService.GetMaxRounds(count);

            if (playerIndex < 0 || playerIndex >= count)
            {
                throw new ArgumentException($"Player index must be between 0 and {count - 1}", nameof(playerIndex));
            }

            if (round < 1 || round > maxRounds)
            {
                throw new ArgumentException($"Round must be between 1 and {maxRounds}", nameof(round));
            }

            var player = players[playerIndex];
            var playerNames = players.Select(p => p.Name).ToList();

            // Generate pairs for this round
            var pairs = _roundSchedulingService.GenerateRoundPairs(round, playerNames);

            // Find this player's match
            var matchPair = pairs.FirstOrDefault(p =>
                p.Home == player.Name || p.Away == player.Name);

            if (matchPair == null)
            {
                throw new InvalidOperationException($"Player {player.Name} has no match in round {round}");
            }

            var opponentName = matchPair.Home == player.Name ? matchPair.Away : matchPair.Home;
            var opponentIndex = players.ToList().FindIndex(p => p.Name == opponentName);

            return new PlayerRoundResponse
            {
                Round = round,
                PlayerIndex = playerIndex,
                Player = player.Name,
                OpponentIndex = opponentIndex,
                Opponent = opponentName
            };
        }

        public async Task<PlayerScheduleResponse> GetPlayerScheduleAsync(int playerIndex, CancellationToken ct = default)
        {
            var players = await _playerService.GetAllAsync(ct);
            var count = players.Count;

            if (playerIndex < 0 || playerIndex >= count)
            {
                throw new ArgumentException($"Player index must be between 0 and {count - 1}", nameof(playerIndex));
            }

            var player = players[playerIndex];
            var playerNames = players.Select(p => p.Name).ToList();
            var maxRounds = _roundSchedulingService.GetMaxRounds(count);

            var schedule = new List<PlayerScheduleItemDto>();

            // Generate schedule for each round
            for (int round = 1; round <= maxRounds; round++)
            {
                var pairs = _roundSchedulingService.GenerateRoundPairs(round, playerNames);

                // Find which match this player is in
                var matchPair = pairs.FirstOrDefault(p =>
                    p.Home == player.Name || p.Away == player.Name);

                if (matchPair != null)
                {
                    var opponentName = matchPair.Home == player.Name ? matchPair.Away : matchPair.Home;
                    var opponentIndex = players.ToList().FindIndex(p => p.Name == opponentName);

                    schedule.Add(new PlayerScheduleItemDto
                    {
                        Round = round,
                        OpponentIndex = opponentIndex,
                        Opponent = opponentName
                    });
                }
            }

            return new PlayerScheduleResponse
            {
                N = count,
                PlayerIndex = playerIndex,
                Player = player.Name,
                Schedule = schedule
            };
        }

        public async Task<RemainingPairsResponse> GetRemainingPairsAsync(int? playerCount, int? roundsPlayed, CancellationToken ct = default)
        {
            int n = playerCount ?? await _playerService.GetCountAsync(ct);
            int d = roundsPlayed ?? 0;

            if (n < 2 || n % 2 != 0)
            {
                throw new ArgumentException("Player count must be even and at least 2");
            }

            var maxRounds = _roundSchedulingService.GetMaxRounds(n);

            if (d < 0 || d > maxRounds)
            {
                throw new ArgumentException($"Rounds played must be between 0 and {maxRounds}");
            }

            // Total unique pairs in round-robin: C(n,2) = n * (n-1) / 2
            int totalPairs = n * (n - 1) / 2;

            // Matches played per round: n / 2
            int matchesPerRound = n / 2;

            // Total matches played: D * (n/2)
            int pairsPlayed = d * matchesPerRound;

            // Remaining pairs
            int remaining = totalPairs - pairsPlayed;

            return new RemainingPairsResponse
            {
                PlayerCount = n,
                RoundsPlayed = d,
                TotalPairs = totalPairs,
                Remaining = remaining
            };
        }

        public async Task<RoundResponse> GetRoundAsync(int round, CancellationToken ct = default)
        {
            var players = await _playerService.GetAllAsync(ct);
            var count = players.Count;
            var maxRounds = _roundSchedulingService.GetMaxRounds(count);

            if (round < 1 || round > maxRounds)
            {
                throw new ArgumentException($"Round must be between 1 and {maxRounds}", nameof(round));
            }

            // Get player names in order
            var playerNames = players.Select(p => p.Name).ToList();

            // Generate pairs for this round
            var pairs = _roundSchedulingService.GenerateRoundPairs(round, playerNames);

            return new RoundResponse
            {
                Round = round,
                Pairs = pairs.Select(p => new MatchPairDto
                {
                    Home = p.Home,
                    Away = p.Away
                }).ToList()
            };
        }
    }
}
