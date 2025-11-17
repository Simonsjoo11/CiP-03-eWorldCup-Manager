namespace EWorldCup.Domain.Entities
{
    /// <summary>
    /// Represents a player's score and statistics within a tournament
    /// Used for scoreboard/standings display
    /// </summary>
    public class PlayerScore
    {
        /// <summary>
        /// Primary key
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Foreign key to tournament
        /// </summary>
        public int TournamentId { get; set; }

        /// <summary>
        /// Player's index in the round-robin schdule
        /// </summary>
        public int PlayerIndex { get; set; }

        /// <summary>
        /// Player's name
        /// </summary>
        public required string PlayerName { get; set; }

        /// <summary>
        /// Number of matches won (not game rounds, but entire matches)
        /// </summary>
        public int Wins { get; set; }

        /// <summary>
        /// Number of matches lost
        /// </summary>
        public int Losses { get; set; }

        /// <summary>
        /// Total points (3 per win, 0 per loss)
        /// </summary>
        public int Points { get; set; }

        /// <summary>
        /// Total matches played (Wins + Losses)
        /// </summary>
        public int MatchesPlayed => Wins + Losses;
    }
}
