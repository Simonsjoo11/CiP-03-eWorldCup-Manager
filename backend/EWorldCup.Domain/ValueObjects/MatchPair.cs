namespace EWorldCup.Domain.ValueObjects
{
    public record MatchPair
    {
        /// <summary>
        /// Name of the home participant
        /// </summary>
        public required string Home { get; init; }

        /// <summary>
        /// Name of the away participant
        /// </summary>
        public required string Away { get; init; }
    }
}
