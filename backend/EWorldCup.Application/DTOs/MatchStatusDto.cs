namespace EWorldCup.Application.DTOs
{
    /// <summary>
    /// Information about the player's current match
    /// </summary>
    public record MatchStatusDto
    {
        /// <summary>
        /// Opponent's name
        /// </summary>
        public required string OpponentName { get; init; }

        /// <summary>
        /// Opponent's index (0-based)
        /// </summary>
        public required int OpponentIndex { get; init; }

        /// <summary>
        /// Current game round number within the match (1, 2, or 3)
        /// </summary>
        public required int GameRoundNumber { get; init; }

        /// <summary>
        /// Number of rounds the player has won in this match
        /// </summary>
        public required int PlayerWins { get; init; }

        /// <summary>
        /// Number of rounds the opponent has won in this match
        /// </summary>
        public required int OpponentWins { get; init; }

        /// <summary>
        /// Match status (InProgress, Completed, etc.)
        /// </summary>
        public required string MatchStatus { get; init; }
    }
}
