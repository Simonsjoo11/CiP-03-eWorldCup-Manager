using EWorldCup.Application.DTOs;

namespace EWorldCup.Application.Responses
{
    public record PlayersResponse
    {
        /// <summary>
        /// Total number of players
        /// </summary>
        public required int PlayerCount { get; init; }

        /// <summary>
        /// Maximum numper of rounds
        /// </summary>
        public required int MaxRounds { get; init; }

        /// <summary>
        /// List of all players
        /// </summary>
        public List<PlayerDto> Players{ get; init; } = [];
    }
}
