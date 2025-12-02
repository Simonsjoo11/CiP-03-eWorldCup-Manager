using EWorldCup.Application.Responses;
using EWorldCup.Domain.Entities;

namespace EWorldCup.Application.Interfaces
{
    /// <summary>
    /// Service for managing Rock-Paper-Scissors tournaments
    /// </summary>
    public interface ITournamentService
    {
        /// <summary>
        /// Starts a new tournament with specified player name and total players
        /// Creates AI players, initializes scoreboard, and sets up first round matches
        /// </summary>
        /// <param name="playerName">Name of the human player</param>
        /// <param name="totalPlayers">Total number of players (must be even, minimum 2)</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns>Tournament start response with initial state</returns>
        Task<TournamentStartResponse> StartTournamentAsync(string playerName, int totalPlayers, CancellationToken ct = default);

        /// <summary>
        /// Gets the current status of an active tournament
        /// Includes current round, player's next opponent, scoreboard, and match progress
        /// </summary>
        /// <param name="tournamentId">ID of the tournament</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns>Current tournament status</returns>
        Task<TournamentStatusResponse> GetStatusAsync(int tournamentId, CancellationToken ct = default);

        /// <summary>
        /// Processes the human player's move in the current match
        /// Generates AI opponent's choice, determines winner, and updates match state
        /// </summary>
        /// <param name="tournamentId">ID of the tournament</param>
        /// <param name="playerChoice">Player's RPS choice (Rock, Paper, or Scissors)</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns>Result of the round including choices and winner</returns>
        Task<PlayRoundResponse> PlayRoundAsync(int tournamentId, RpsChoice playerChoice, CancellationToken ct = default);

        /// <summary>
        /// Advances the tournament by simulating all AI vs AI matches in the current round
        /// Updates scoreboard with match results and sets up next round (if not complete)
        /// </summary>
        /// <param name="tournamentId">ID of the tournament</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns>Response indicating if tournament continues or is complete</returns>
        Task<AdvanceRoundResponse> AdvanceRoundAsync(int tournamentId, CancellationToken ct = default);

        /// <summary>
        /// Gets the final results of a completed tournament
        /// Includes final scoreboard sorted by points and the tournament winner
        /// </summary>
        /// <param name="tournamentId">ID of the tournament</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns>Final tournament results and winner</returns>
        Task<FinalResultResponse> GetFinalResultAsync(int tournamentId, CancellationToken ct = default);
    }
}
