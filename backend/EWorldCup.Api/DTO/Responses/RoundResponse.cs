using EWorldCup.Api.Models;

namespace EWorldCup.Api.DTO.Responses
{
    public sealed class RoundResponse
    {
        public int Round {  get; init; }
        public List<MatchPair> Pairs { get; init; } = new();
    }
}
