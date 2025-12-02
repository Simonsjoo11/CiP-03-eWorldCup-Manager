using EWorldCup.Application.DTOs;

namespace EWorldCup.Application.Responses
{
    /// <summary>
    /// Response after advancing to the next round
    /// </summary>
    public record AdvanceRoundResponse
    {
        /// <summary>
        /// Tournament ID
        /// </summary>
        public required int TournamentId { get; init; }

        /// <summary>
        /// The round that was just completed
        /// </summary>
        public required int CompletedRound { get; init; }

        /// <summary>
        /// Number of matches simulated in this round
        /// </summary>
        public required int MatchesSimulated { get; init; }

        /// <summary>
        /// True if the tournament is now complete
        /// </summary>
        public required bool TournamentComplete { get; init; }

        /// <summary>
        /// Next round number
        /// </summary>
        public int? NextRound { get; init; }

        /// <summary>
        /// Player's next opponent
        /// </summary>
        public string? NextOpponent { get; init; }

        /// <summary>
        /// Updated scoreboard after this round
        /// </summary>
        public List<ScoreboardEntryDto> Scoreboard { get; init; } = [];

        /// <summary>
        /// Summary message
        /// </summary>
        public required string Message { get; init; }
    }
}
