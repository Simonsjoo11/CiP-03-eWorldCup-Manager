namespace EWorldCup.Api.DTO.Responses
{
    public sealed class RemainingPairsResponse
    {
        public bool Ok { get; init; } = true;
        public int N { get; init; }
        public int RoundsPlayed { get; init; }
        public int Remaining { get; init; }
        public int TotalPairs { get; init; }
    }
}
