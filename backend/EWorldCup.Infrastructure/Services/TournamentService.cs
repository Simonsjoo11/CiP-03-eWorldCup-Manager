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
        private const int HUMAN_PLAYER_INDEX = 0;
        private const int WINS_NEEDED_FOR_MATCH = 2;
        private const int MATCH_WIN_POINTS = 3;
        private const int MATCH_LOSS_POINTS = 0;

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
                    IsPlayerMatch = player1Index == HUMAN_PLAYER_INDEX || player2Index == HUMAN_PLAYER_INDEX,
                    Status = MatchStatus.NotStarted,
                    Player1Wins = 0,
                    Player2Wins = 0,
                };

                _context.Matches.Add(match);
            }

            await _context.SaveChangesAsync(ct);

            // Find player's first opponent
            var playerMatch = await _context.Matches.FirstAsync(m => m.TournamentId == tournament.Id && m.Round == 1 && m.IsPlayerMatch, ct);

            var opponentName = playerMatch.Player1Index == HUMAN_PLAYER_INDEX ? playerMatch.Player2Name : playerMatch.Player1Name;
            var opponentIndex = playerMatch.Player1Index == HUMAN_PLAYER_INDEX ? playerMatch.Player2Index : playerMatch.Player1Index;

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
                var opponentName = playerMatch.Player1Index == HUMAN_PLAYER_INDEX ? playerMatch.Player2Name : playerMatch.Player1Name;
                var opponentIndex = playerMatch.Player1Index == HUMAN_PLAYER_INDEX ? playerMatch.Player2Index : playerMatch.Player1Index;
                var playerWins = playerMatch.Player1Index == HUMAN_PLAYER_INDEX ? playerMatch.Player1Wins : playerMatch.Player2Wins;
                var opponentWins = playerMatch.Player1Index == HUMAN_PLAYER_INDEX ? playerMatch.Player2Wins : playerMatch.Player1Wins;

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

        public async Task<PlayRoundResponse> PlayRoundAsync(int tournamentId, RpsChoice playerChoice, CancellationToken ct = default)
        {
            _logger.LogInformation("Player making move in tournament {TournamentId}: {Choice}", tournamentId, playerChoice);

            // Load tournament with matches and game rounds
            var tournament = await _context.Tournaments
                .Include(t => t.Matches)
                .ThenInclude(m => m.GameRounds)
                .FirstOrDefaultAsync(t => t.Id == tournamentId, ct);

            if (tournament == null)
                throw new InvalidOperationException($"Tournament {tournamentId} not found");

            if (tournament.Status != TournamentStatus.InProgress)
                throw new InvalidOperationException("Tournament is not in progress");

            // Find players match in current round
            var playerMatch = tournament.Matches
                .FirstOrDefault(m => m.Round == tournament.CurrentRound && m.IsPlayerMatch);

            if (playerMatch == null)
                throw new InvalidOperationException("No player match found for current round");

            if (playerMatch.Status == MatchStatus.Completed)
                throw new InvalidOperationException("Match is already completed");

            // Determine if player is Player 1 or Player 2
            bool playerIsPlayer1 = playerMatch.Player1Index == HUMAN_PLAYER_INDEX;

            // Generate opponents choice and create game round
            var opponentChoice = _rpsGameService.GenerateRandomChoice();
            var gameRound = _rpsGameService.CreateGameRound(
                playerMatch.GameRounds.Count + 1,
                playerIsPlayer1 ? playerChoice : opponentChoice,
                playerIsPlayer1 ? opponentChoice : playerChoice
            );

            // Set the match ID and add to database
            gameRound.MatchId = playerMatch.Id;
            _context.GameRounds.Add(gameRound);
            playerMatch.GameRounds.Add(gameRound);

            // Update win counts based on result
            if (gameRound.Result == RoundResult.Player1Win)
                playerMatch.Player1Wins++;
            else if (gameRound.Result == RoundResult.Player2Win)
                playerMatch.Player2Wins++;

            // Determine current wins for response
            int playerWins = playerIsPlayer1 ? playerMatch.Player1Wins : playerMatch.Player2Wins;
            int opponentWins = playerIsPlayer1 ? playerMatch.Player2Wins : playerMatch.Player1Wins;

            // Check if match is complete
            bool matchComplete = playerMatch.Player1Wins >= WINS_NEEDED_FOR_MATCH || playerMatch.Player2Wins >= WINS_NEEDED_FOR_MATCH;

            string? matchWinner = null;
            if (matchComplete)
            {
                playerMatch.Status = MatchStatus.Completed;

                // Determine winner
                if (playerMatch.Player1Wins >= WINS_NEEDED_FOR_MATCH)
                {
                    playerMatch.WinnerIndex = playerMatch.Player1Index;
                    matchWinner = playerMatch.Player1Name;
                }
                else
                {
                    playerMatch.WinnerIndex = playerMatch.Player2Index;
                    matchWinner = playerMatch.Player2Name;
                }

                _logger.LogInformation("Match completed: Winner: {Winner}", matchWinner);

                // Update scoreboard with match results
                await UpdateScoreboardAfterMatchAsync(tournament.Id, playerMatch, ct);
            }

            else if (playerMatch.Status == MatchStatus.NotStarted)
            { 
                playerMatch.Status = MatchStatus.InProgress;
            }

            await _context.SaveChangesAsync(ct);

            // Build response message
            var result = DeterminePlayerResult(gameRound.Result, playerIsPlayer1);
            var message = BuildResultMessage(playerChoice, opponentChoice, result);

            return new PlayRoundResponse
            {
                GameRoundNumber = gameRound.RoundNumber,
                PlayerChoice = playerChoice,
                OpponentChoice = opponentChoice,
                Result = result,
                Message = message,
                PlayerWins = playerWins,
                OpponentWins = opponentWins,
                MatchComplete = matchComplete,
                MatchWinner = matchWinner
            };
        }


        public async Task<AdvanceRoundResponse> AdvanceRoundAsync(int tournamentId, CancellationToken ct = default)
        {
            _logger.LogInformation("Advancing round for tournament {TournamentId}", tournamentId);

            var tournament = await _context.Tournaments
                .Include(t => t.Matches)
                .ThenInclude(m => m.GameRounds)
                .Include(t => t.Scoreboard)
                .FirstOrDefaultAsync(t => t.Id == tournamentId, ct);

            if (tournament == null)
                throw new InvalidOperationException($"Tournament {tournamentId} not found");

            if (tournament.Status != TournamentStatus.InProgress)
                throw new InvalidOperationException("Tournament is not in progress");

            // Get all AI vs AI matches in current round
            var aiMatches = tournament.Matches
                .Where(m => m.Round == tournament.CurrentRound && !m.IsPlayerMatch && m.Status != MatchStatus.Completed)
                .ToList();

            int matchesSimulated = 0;

            // Simulate all remaining AI matches
            foreach (var match in aiMatches)
            {
                var gameRounds = _rpsGameService.SimulateMatch();

                foreach (var gameRound in gameRounds)
                {
                    gameRound.MatchId = match.Id;
                    _context.GameRounds.Add(gameRound);
                    match.GameRounds.Add(gameRound);

                    // Update wins
                    if (gameRound.Result == RoundResult.Player1Win)
                        match.Player1Wins++;
                    else if (gameRound.Result == RoundResult.Player2Win)
                        match.Player2Wins++;
                }

                // Mark match as completed and set winner
                match.Status = MatchStatus.Completed;
                if (match.Player1Wins >= WINS_NEEDED_FOR_MATCH)
                    match.WinnerIndex = match.Player1Index;
                else
                    match.WinnerIndex = match.Player2Index;

                // Update scoreboard for this match
                await UpdateScoreboardAfterMatchAsync(tournament.Id, match, ct);

                matchesSimulated++;
            }

            await _context.SaveChangesAsync(ct);

            // Check if all matches in current round are completed
            var allMatchesCompleted = tournament.Matches
                .Where(m => m.Round == tournament.CurrentRound)
                .All(m => m.Status == MatchStatus.Completed);

            if (!allMatchesCompleted)
                throw new InvalidOperationException("Not all matches in the current round are completed");

            int completedRound = tournament.CurrentRound;
            int maxRounds = _roundSchedulingService.GetMaxRounds(tournament.TotalPlayers);

            // Check if tournament is complete
            bool tournamentComplete = completedRound >= maxRounds;

            string? nextOpponent = null;
            int? nextRound = null;

            if (!tournamentComplete)
            {
                // Generate next round matches
                nextRound = completedRound + 1;
                tournament.CurrentRound = nextRound.Value;

                var playerNames = tournament.Scoreboard
                    .OrderBy(s => s.PlayerIndex)
                    .Select(s => s.PlayerName)
                    .ToList();

                var nextRoundPairs = _roundSchedulingService.GenerateRoundPairs(nextRound.Value, playerNames);

                foreach (var pair in nextRoundPairs)
                {
                    var player1Index = playerNames.IndexOf(pair.Home);
                    var player2Index = playerNames.IndexOf(pair.Away);

                    var match = new Match
                    {
                        TournamentId = tournament.Id,
                        Round = nextRound.Value,
                        Player1Index = player1Index,
                        Player1Name = pair.Home,
                        Player2Index = player2Index,
                        Player2Name = pair.Away,
                        IsPlayerMatch = player1Index == HUMAN_PLAYER_INDEX || player2Index == HUMAN_PLAYER_INDEX,
                        Status = MatchStatus.NotStarted,
                        Player1Wins = 0,
                        Player2Wins = 0,
                    };

                    _context.Matches.Add(match);
                    tournament.Matches.Add(match);

                    // Find player's next opponent
                    if (match.IsPlayerMatch)
                    {
                        nextOpponent = match.Player1Index == HUMAN_PLAYER_INDEX ? match.Player2Name : match.Player1Name;
                    }
                }

                await _context.SaveChangesAsync(ct);

                _logger.LogInformation("Advanced to round {NextRound}", nextRound);
            }
            else
            {
                tournament.Status = TournamentStatus.Completed;
                tournament.CompletedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync(ct);

                _logger.LogInformation("Tournament {TournamentId} completed", tournamentId);
            }

            // Get updated scoreboard
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

            var message = tournamentComplete
                ? $"Tournament complete! All {maxRounds} rounds finished."
                : $"Round {completedRound} complete. {matchesSimulated} AI matches simulated. Advancing to round {nextRound}.";

            return new AdvanceRoundResponse
            {
                TournamentId = tournament.Id,
                CompletedRound = completedRound,
                MatchesSimulated = matchesSimulated,
                TournamentComplete = tournamentComplete,
                NextRound = nextRound,
                NextOpponent = nextOpponent,
                Scoreboard = scoreboard,
                Message = message
            };
        }

        public async Task<FinalResultResponse> GetFinalResultAsync(int tournamentId, CancellationToken ct = default)
        {
            var tournament = await _context.Tournaments
                .Include(t => t.Scoreboard)
                .FirstOrDefaultAsync(t => t.Id == tournamentId, ct);

            if (tournament == null)
                throw new InvalidOperationException($"Tournament {tournamentId} not found");

            if (tournament.Status != TournamentStatus.Completed)
                throw new InvalidOperationException("Tournament is not yet completed");

            // Get final scoreboard sorted by points
            var finalScoreboard = tournament.Scoreboard
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
                    Rank = index + 1,
                })
                .ToList();

            var winner = finalScoreboard.First();
            var humanPlayer = finalScoreboard.First(s => s.PlayerIndex == HUMAN_PLAYER_INDEX);

            return new FinalResultResponse
            {
                TournamentId = tournament.Id,
                PlayerName = tournament.PlayerName,
                TotalRounds = _roundSchedulingService.GetMaxRounds(tournament.TotalPlayers),
                Winner = winner.PlayerName,
                WinnerIndex = winner.PlayerIndex,
                WinnerPoints = winner.Points,
                PlayerWon = humanPlayer.PlayerIndex == winner.PlayerIndex,
                PlayerRank = humanPlayer.Rank,
                FinalScoreboard = finalScoreboard
            };
        }

        private async Task UpdateScoreboardAfterMatchAsync(int tournamentId, Match match, CancellationToken ct)
        {
            if (match.Status != MatchStatus.Completed)
                throw new InvalidOperationException("Cannot update scoreboard for incomplete match");

            if (match.WinnerIndex == null)
                throw new InvalidOperationException("Match must have a winner to update scoreboard");

            _logger.LogInformation("Updating scoreboard for match: {Player1} vs {Player2}, Winner: {Winner}", 
                match.Player1Name, 
                match.Player2Name, 
                match.WinnerIndex == match.Player1Index ? match.Player1Name : match.Player2Name);

            // Fetch the player scores for both players in this match
            var playerScores = await _context.PlayerScores
                .Where(ps => ps.TournamentId == tournamentId && (ps.PlayerIndex == match.Player1Index || ps.PlayerIndex == match.Player2Index))
                .ToListAsync(ct);

            var player1Score = playerScores.First(ps => ps.PlayerIndex == match.Player1Index);
            var player2Score = playerScores.First(ps => ps.PlayerIndex == match.Player2Index);

            // Apply point system based on match outcome
            if (match.WinnerIndex == match.Player1Index)
            {
                // Player1 won the match
                player1Score.Wins++;
                player1Score.Points += MATCH_WIN_POINTS;
                
                // Loss = 0 points
                player2Score.Losses++;
            }

            else
            {
                // Player2 won the match
                player2Score.Wins++;
                player2Score.Points += MATCH_WIN_POINTS;

                // Loss = 0 points
                player1Score.Losses++;
            }

            _logger.LogInformation("Scoreboard updated - {Player1}: {P1Points}p ({P1Wins}W-{P1Losses}L), {Player2}: {P2Points}p ({P2Wins}W-{P2Losses}L)",
                player1Score.PlayerName, player1Score.Points, player1Score.Wins, player1Score.Losses,
                player2Score.PlayerName, player2Score.Points, player2Score.Wins, player2Score.Losses);
        }

        private static string DeterminePlayerResult(RoundResult result, bool playerIsPlayer1)
        {
            return result switch
            {
                RoundResult.Draw => "Draw",
                RoundResult.Player1Win => playerIsPlayer1 ? "Win" : "Loss",
                RoundResult.Player2Win => playerIsPlayer1 ? "Loss" : "Win",
                _ => "Unknown"
            };
        }

        private static string BuildResultMessage(RpsChoice playerChoice, RpsChoice opponentChoice, string result)
        {
            if (result == "Draw")
                return $"Draw! Both chose {playerChoice}.";

            if (result == "Win")
            {
                var reason = (playerChoice, opponentChoice) switch
                {
                    (RpsChoice.Rock, RpsChoice.Scissors) => "Rock crushes Scissors",
                    (RpsChoice.Paper, RpsChoice.Rock) => "Paper covers Rock",
                    (RpsChoice.Scissors, RpsChoice.Paper) => "Scissors cuts Paper",
                    _ => "Unknown"
                };
                return $"You won! {reason}.";
            }

            if (result == "Loss")
            {
                var reason = (opponentChoice, playerChoice) switch
                {
                    (RpsChoice.Rock, RpsChoice.Scissors) => "Rock crushes Scissors",
                    (RpsChoice.Paper, RpsChoice.Rock) => "Paper covers Rock",
                    (RpsChoice.Scissors, RpsChoice.Paper) => "Scissors cuts Paper",
                    _ => "Unknown"
                };
                return $"You lost! {reason}.";
            }

            return "Unknown result.";
        }
    }
}
