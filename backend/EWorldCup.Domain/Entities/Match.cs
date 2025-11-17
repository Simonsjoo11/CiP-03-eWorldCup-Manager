namespace EWorldCup.Domain.Entities
{
    /// <summary>
    /// Represents a single match between two players (best-of-3 game rounds)
    /// </summary>
    public class Match
    {
        /// <summary>
        /// Primary key
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Foreign key to Tournament
        /// </summary>
        public int TournamentId { get; set; }

        /// <summary>
        /// Which round-robin round this match belongs to (1 to n-1)
        /// </summary>
        public int Round { get; set; }

        /// <summary>
        /// First player's index in the round-robin schedule (0-based)
        /// </summary>
        public int Player1Index { get; set; }

        /// <summary>
        /// First player's name (denormalized for easier querying)
        /// </summary>
        public required string Player1Name { get; set; }

        /// <summary>
        /// Second player's index in the round-robin schedule (0-based)
        /// </summary>
        public int Player2Index { get; set; }

        /// <summary>
        /// Second player's name (denormalized for easier querying)
        /// </summary>
        public required string Player2Name { get; set; }

        /// <summary>
        /// True if this match involves the human player (requires manual play)
        /// False if this is an AI vs AI match (auto-simulated)
        /// </summary>
        public bool IsPlayerMatch { get; set; }

        /// <summary>
        /// Current status of this match
        /// </summary>
        public MatchStatus Status { get; set; }

        /// <summary>
        /// The individual game rounds within this match (up to 3)
        /// </summary>
        public List<GameRound> GameRounds { get; set; } = [];

        /// <summary>
        /// How many game rounds Player1 has won (0-2)
        /// </summary>
        public int Player1Wins { get; set; }

        /// <summary>
        /// How many game rounds Player2 has won (0-2)
        /// </summary>
        public int Player2Wins { get; set; }

        /// <summary>
        /// Index of the winning player (null if match not completed or draw)
        /// </summary>
        public int? WinnerIndex { get; set; }
    }

    /// <summary>
    /// Match lifecycle states
    /// </summary>
    public enum MatchStatus
    {
        /// <summary>
        /// Match scheduled but not started
        /// </summary>
        NotStarted = 0,

        /// <summary>
        /// Match in progress (some game rounds played, but not finished)
        /// </summary>
        InProgress = 1,

        /// <summary>
        /// Match finished (someone won 2 rounds or all 3 played)
        /// </summary>
        Completed = 2
    }
}
