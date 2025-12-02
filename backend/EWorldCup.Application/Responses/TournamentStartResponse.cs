namespace EWorldCup.Application.Responses
{
    /// <summary>
    /// Response returned when a new tournament is started
    /// </summary>
    public record TournamentStartResponse
    {
        /// <summary>
        /// The unique ID of the created tournament
        /// </summary>
        public required int TournamentId { get; init; }

        /// <summary>
        /// The human player's name
        /// </summary>
        public required string PlayerName { get; init; }

       /// <summary>
       /// Total number of players in the tournament
       /// </summary>
        public int TotalPlayers { get; init; }

        /// <summary>
        /// Maximum number of rounds
        /// </summary>
        public required int MaxRounds { get; init; }

        /// <summary>
        /// Name of the player's first opponent
        /// </summary>
        public required string FirstOpponent { get; init; }

        /// <summary>
        /// Index of the first opponent
        /// </summary>
        public required int FirstOpponentIndex { get; init; }
    }
}
