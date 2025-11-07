using EWorldCup.Application.DTOs;

namespace EWorldCup.Application.Responses
{
    public record PlayerScheduleResponse
    {
        /// <summary>
        /// Total number of participants in the tournament
        /// </summary>
        public required int N { get; init; }

        /// <summary>
        /// The players index
        /// </summary>
        public required int PlayerIndex { get; init; }

        /// <summary>
        /// The players name
        /// </summary>
        public required string Player { get; init; }

        /// <summary>
        /// List of all rounds and opponents for this player
        /// </summary>
        public List<PlayerScheduleItemDto> Schedule { get; init; } = [];
    }
}
