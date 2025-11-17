namespace EWorldCup.Application.DTOs
{
    /// <summary>
    /// Single entry in the tournament scoreboard
    /// </summary>
    public record ScoreboardEntryDto
    {
        /// <summary>
        /// Player's index
        /// </summary>
        public required int PlayerIndex { get; init; }

        /// <summary>
        /// Player's name
        /// </summary>
        public required string PlayerName { get; init; }

        /// <summary>
        /// Number of matches won
        /// </summary>
        public required int Wins { get; init; }

        /// <summary>
        /// Number of matches lost
        /// </summary>
        public required int Losses { get; init; }

        /// <summary>
        /// Total points (3 per win, 0 per loss)
        /// </summary>
        public required int Points { get; init; }

        /// <summary>
        /// Current rank position (1 = first place)
        /// </summary>
        public int Rank { get; init; }
    }
}
