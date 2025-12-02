using EWorldCup.Application.DTOs;

namespace EWorldCup.Application.Responses
{
    /// <summary>
    /// Response containing final tournament results and winner
    /// </summary>
    public record FinalResultResponse
    {
        /// <summary>
        /// Tournament ID
        /// </summary>
        public required int TournamentId { get; init; }

        /// <summary>
        /// Human player's name
        /// </summary>
        public required string PlayerName { get; init; }

        /// <summary>
        /// Total number of rounds played
        /// </summary>
        public required int TotalRounds { get; init; }

        /// <summary>
        /// Winner's name
        /// </summary>
        public required string Winner { get; init; }

        /// <summary>
        /// Winner's index
        /// </summary>
        public required int WinnerIndex { get; init; }

        /// <summary>
        /// Winner's total points
        /// </summary>
        public required int WinnerPoints { get; init; }

        /// <summary>
        /// True if the human player won the tournament
        /// </summary>
        public required bool PlayerWon { get; init; }

        /// <summary>
        /// Human player's final rank
        /// </summary>
        public required int PlayerRank { get; init; }

        /// <summary>
        /// Final scoreboard sorted by rank
        /// </summary>
        public List<ScoreboardEntryDto> FinalScoreboard { get; init; } = [];
    }
}
