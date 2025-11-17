using EWorldCup.Application.DTOs;
using EWorldCup.Application.Interfaces;
using EWorldCup.Application.Responses;
using EWorldCup.Domain.Entities;
using EWorldCup.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace EWorldCup.Infrastructure.Services
{
    public class TournamentService : ITournamentService
    {
        private readonly ApplicationDbContext _context;
        private readonly IRpsGameService _rpsGameService;
        private readonly IRoundSchedulingService _roundSchedulingService;
        private readonly ILogger<TournamentService> _logger;

        public TournamentService(
            ApplicationDbContext context,
            IRpsGameService rpsGameService,
            IRoundSchedulingService roundSchedulingService,
            ILogger<TournamentService> logger)
        {
            _context = context;
            _rpsGameService = rpsGameService;
            _roundSchedulingService = roundSchedulingService;
            _logger = logger;
        }

        public async Task<TournamentStartResponse> StartTournamentAsync(string playerName, int totalPlayers, CancellationToken ct = default)
        {
            _logger.LogInformation("Starting new tournament: Player={PlayerName}, TotalPlayers={TotalPlayers}", playerName, totalPlayers);

            // Validate input
            if (string.IsNullOrWhiteSpace(playerName))
                throw new ArgumentException("Player name cannot be empty", nameof(playerName));
            if (totalPlayers < 2)
                throw new ArgumentException("Must have at least 2 players", nameof(totalPlayers));
            if (totalPlayers % 2 != 0)
                throw new ArgumentException("Total players must be even", nameof(totalPlayers));

            // Fetch all players from database
            var allPlayers = await _context.Players.OrderBy(p => p.Id).ToListAsync(ct);

            if (allPlayers.Count < totalPlayers)
                throw new InvalidOperationException("Not enough players available to start the tournament");

            // Find the human player
            var humanPlayer = allPlayers.FirstOrDefault(p => p.Name.Equals(playerName.Trim(), StringComparison.OrdinalIgnoreCase));
            if (humanPlayer == null)
                throw new InvalidOperationException("Human player not found in the database");

            // Select random players for tournament (excluding human player)
            var otherPlayers = allPlayers.Where(p => p.Id != humanPlayer.Id).ToList();
            var selectedOtherPlayers = otherPlayers
                .OrderBy(_ => Random.Shared.Next())
                .Take(totalPlayers - 1)
                .ToList();

            // Build final player list (human at index 0, others randomized
            var tournamentPlayers = new List<Player> { humanPlayer };
            tournamentPlayers.AddRange(selectedOtherPlayers);

            var playerNames = tournamentPlayers.Select(p => p.Name).ToList();

            // Create tournament
            var tournament = new Tournament
            {
                PlayerName = humanPlayer.Name,
                TotalPlayers = totalPlayers,
                CurrentRound = 1,
                Status = TournamentStatus.InProgress,
                StartedAt = DateTime.UtcNow,
            };

            _context.Tournaments.Add(tournament);
            await _context.SaveChangesAsync(ct);

            _logger.LogInformation("Tournament created with ID: {TournamentId}", tournament.Id);

            // Initialize scoreboard
            for (int i = 0; i < totalPlayers; i++)
            {
                _context.PlayerScores.Add(new PlayerScore
                {
                    TournamentId = tournament.Id,
                    PlayerIndex = i,
                    PlayerName = playerNames[i],
                    Wins = 0,
                    Losses = 0,
                    Points = 0,
                });
            }
            await _context.SaveChangesAsync(ct);

            // Generate Round 1 matches using round-robin scheduling
            var round1Pairs = _roundSchedulingService.GenerateRoundPairs(1, playerNames);

            foreach (var pair in round1Pairs)
            {
                var player1Index = playerNames.IndexOf(pair.Home);
                var player2Index = playerNames.IndexOf(pair.Away);

                var match = new Match
                {
                    TournamentId = tournament.Id,
                    Round = 1,
                    Player1Index = player1Index,
                    Player1Name = pair.Home,
                    Player2Index = player2Index,
                    Player2Name = pair.Away,
                    IsPlayerMatch = player1Index == 0 || player2Index == 0, // Human is index 0
                    Status = MatchStatus.NotStarted,
                    Player1Wins = 0,
                    Player2Wins = 0,
                };

                _context.Matches.Add(match);
            }

            await _context.SaveChangesAsync(ct);

            // Find player's first opponent
            var playerMatch = await _context.Matches.FirstAsync(m => m.TournamentId == tournament.Id && m.Round == 1 && m.IsPlayerMatch, ct);

            var opponentName = playerMatch.Player1Index == 0 ? playerMatch.Player2Name : playerMatch.Player1Name;
            var opponentIndex = playerMatch.Player1Index == 0 ? playerMatch.Player2Index : playerMatch.Player1Index;

            return new TournamentStartResponse
            {
                TournamentId = tournament.Id,
                PlayerName = humanPlayer.Name,
                TotalPlayers = totalPlayers,
                MaxRounds = _roundSchedulingService.GetMaxRounds(totalPlayers),
                FirstOpponent = opponentName,
                FirstOpponentIndex = opponentIndex
            };
        }

        public async Task<TournamentStatusResponse> GetStatusAsync(int tournamentId, CancellationToken ct = default)
        {
            var tournament = await _context.Tournaments
                .Include(t => t.Matches)
                .ThenInclude(m => m.GameRounds)
                .Include(t => t.Scoreboard)
                .FirstOrDefaultAsync(t => t.Id == tournamentId, ct);

            if (tournament == null)
                throw new InvalidOperationException($"Tournament {tournamentId} not found");
        
            // Get current player match
            var playerMatch = tournament.Matches
                .FirstOrDefault(m => m.Round == tournament.CurrentRound && m.IsPlayerMatch);

            MatchStatusDto? currentMatch = null;
            if (playerMatch != null && playerMatch.Status != MatchStatus.Completed)
            {
                var opponentName = playerMatch.Player1Index == 0 ? playerMatch.Player2Name : playerMatch.Player1Name;
                var opponentIndex = playerMatch.Player1Index == 0 ? playerMatch.Player2Index : playerMatch.Player1Index;
                var playerWins = playerMatch.Player1Index == 0 ? playerMatch.Player1Wins : playerMatch.Player2Wins;
                var opponentWins = playerMatch.Player1Index == 0 ? playerMatch.Player2Wins : playerMatch.Player1Wins;

                currentMatch = new MatchStatusDto
                {
                    OpponentName = opponentName,
                    OpponentIndex = opponentIndex,
                    GameRoundNumber = playerMatch.GameRounds.Count + 1,
                    PlayerWins = playerWins,
                    OpponentWins = opponentWins,
                    MatchStatus = playerMatch.Status.ToString()
                };
            }

            // Get scoreboard sorted by points
            var scoreboard = tournament.Scoreboard
                .OrderByDescending(s => s.Points)
                .ThenByDescending(s => s.Wins)
                .ThenBy(s => s.PlayerName)
                .Select((s, index) => new ScoreboardEntryDto
                {
                    PlayerIndex = s.PlayerIndex,
                    PlayerName = s.PlayerName,
                    Wins = s.Wins,
                    Losses = s.Losses,
                    Points = s.Points,
                    Rank = index + 1
                })
                .ToList();

            return new TournamentStatusResponse
            {
                TournamentId = tournament.Id,
                CurrentRound = tournament.CurrentRound,
                MaxRounds = _roundSchedulingService.GetMaxRounds(tournament.TotalPlayers),
                Status = tournament.Status.ToString(),
                CurrentMatch = currentMatch,
                Scoreboard = scoreboard
            };

        }


        public Task<AdvanceRoundResponse> AdvanceRoundAsync(int tournamentId, CancellationToken ct = default)
        {
            throw new NotImplementedException();
        }

        public Task<FinalResultResponse> GetFinalResultAsync(int tournamentId, CancellationToken ct = default)
        {
            throw new NotImplementedException();
        }


        public Task<PlayRoundResponse> PlayRoundAsync(int tournamentId, RpsChoice playerChoice, CancellationToken ct = default)
        {
            throw new NotImplementedException();
        }

        
    }
}
