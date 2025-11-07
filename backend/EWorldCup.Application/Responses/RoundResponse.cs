using EWorldCup.Application.DTOs;

namespace EWorldCup.Application.Responses
{
    public record RoundResponse
    {
        /// <summary>
        /// The round number
        /// </summary>
        public required int Round { get; init; }

        /// <summary>
        /// List of all match pairings in this round
        /// </summary>
        public List<MatchPairDto> Pairs { get; init; } = [];
    }
}
