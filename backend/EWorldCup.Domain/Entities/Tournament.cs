namespace EWorldCup.Domain.Entities
{
    /// <summary>
    /// Represents a Rock-Paper-Scissors tournament
    /// </summary>
    public class Tournament
    {
        /// <summary>
        /// Primary key - Database ID
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// The player's name
        /// </summary>
        public required string PlayerName { get; set; }

        /// <summary>
        /// Total number of players in the tournament (must be even)
        /// </summary>
        public int TotalPlayers { get; set; }

        /// <summary>
        /// Current round number
        /// </summary>
        public int CurrentRound { get; set; }

        /// <summary>
        /// Tournament lifecycly status
        /// </summary>
        public TournamentStatus Status { get; set; }

        /// <summary>
        /// When the tournament was created
        /// </summary>
        public DateTime StartedAt { get; set; }

        /// <summary>
        /// When the tournament was finished (null if still in progress)
        /// </summary>
        public DateTime? CompletedAt { get; set; }

        /// <summary>
        /// All matches in this tournmanent
        /// </summary>
        public List<Match> Matches { get; set; } = [];

        /// <summary>
        /// Current scoreboard/standings
        /// </summary>
        public List<PlayerScore> Scoreboard { get; set; } = [];
    }

    /// <summary>
    /// Tournament lifecycle
    /// </summary>
    public enum TournamentStatus
    {
        /// <summary>
        /// Tournament created but not started
        /// </summary>
        NotStarted = 0,

        /// <summary>
        /// Tournament is currently being played
        /// </summary>
        InProgress = 1,

        /// <summary>
        /// All rounds completed
        /// </summary>
        Completed = 2
    }
}
