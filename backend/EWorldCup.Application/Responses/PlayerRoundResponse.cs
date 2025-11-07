namespace EWorldCup.Application.Responses
{
    public record PlayerRoundResponse
    {
        /// <summary>
        /// The round number
        /// </summary>
        public required int Round { get; init; }

        /// <summary>
        /// The players index
        /// </summary>
        public required int PlayerIndex { get; init; }

        /// <summary>
        /// The players name
        /// </summary>
        public required string Player { get; init; }

        /// <summary>
        /// The opponents index
        /// </summary>
        public required int OpponentIndex { get; init; }

        /// <summary>
        /// The opponents name
        /// </summary>
        public required string Opponent { get; init; }
    }
}
