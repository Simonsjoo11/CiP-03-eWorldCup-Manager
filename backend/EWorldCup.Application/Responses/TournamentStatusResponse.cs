using EWorldCup.Application.DTOs;

namespace EWorldCup.Application.Responses
{
    /// <summary>
    /// Response containing current tournament status and game state
    /// </summary>
    public record TournamentStatusResponse
    {
        /// <summary>
        /// Tournament ID
        /// </summary>
        public required int TournamentId { get; init; }

        /// <summary>
        /// Current round number
        /// </summary>
        public required int CurrentRound { get; init; }

        /// <summary>
        /// Maximum number of rounds
        /// </summary>
        public required int MaxRounds { get; init; }

        /// <summary>
        /// Tournament status (InProgress, Completed, etc.)
        /// </summary>
        public required string Status { get; init; }

        /// <summary>
        /// Current match information (null if no active match)
        /// </summary>
        public MatchStatusDto? CurrentMatch { get; init; }

        /// <summary>
        /// Current scoreboard sorted by points
        /// </summary>
        public List<ScoreboardEntryDto> Scoreboard { get; init; } = [];
    }
}
