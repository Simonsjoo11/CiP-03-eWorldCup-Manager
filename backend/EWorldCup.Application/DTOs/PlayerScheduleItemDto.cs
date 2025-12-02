namespace EWorldCup.Application.DTOs
{
    public record PlayerScheduleItemDto
    {
        /// <summary>
        /// The round number
        /// </summary>
        public required int Round { get; init; }

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
