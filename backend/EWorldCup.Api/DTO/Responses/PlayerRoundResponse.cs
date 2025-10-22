namespace EWorldCup.Api.DTO.Responses
{
    public sealed class PlayerRoundResponse
    {
        public int Round { get; init; }
        public int PlayerIndex { get; init; }
        public string Player { get; init; } = string.Empty;
        public int OpponentIndex { get; init; }
        public string Opponent { get; init; } = string.Empty;
    }
}
