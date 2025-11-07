namespace EWorldCup.Application.Responses
{
    public record RemainingPairsResponse
    {
        /// <summary>
        /// Total number of participants in the tournament
        /// </summary>
        public required int ParticipantCount { get; init; }

        /// <summary>
        /// Number of rounds already played
        /// </summary>
        public required int RoundsPlayed { get; init; }

        /// <summary>
        /// Number of unique pairings that havent been played yet
        /// </summary>
        public required int Remaining { get; init; }

        /// <summary>
        /// Total numbers of unique pairings possible
        /// </summary>
        public required int TotalPairs { get; init; }
    }
}
