namespace EWorldCup.Domain.ValueObjects
{
    public record MatchPair
    {
        /// <summary>
        /// Name of the home player
        /// </summary>
        public required string Home { get; init; }

        /// <summary>
        /// Name of the away player
        /// </summary>
        public required string Away { get; init; }
    }
}
