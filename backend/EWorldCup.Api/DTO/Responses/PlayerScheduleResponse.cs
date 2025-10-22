namespace EWorldCup.Api.DTO.Responses
{
    public sealed class PlayerScheduleItemDto
    {
        public int Round { get; init; }
        public int OpponentIndex { get; init; }
        public string Opponent { get; init; } = string.Empty;
    }

    public sealed class PlayerScheduleResponse
    {
        public int N { get; init; }
        public int PlayerIndex { get; init; }
        public string Player { get; init; } = string.Empty;
        public List<PlayerScheduleItemDto> Schedule { get; init; } = new();
    }
}