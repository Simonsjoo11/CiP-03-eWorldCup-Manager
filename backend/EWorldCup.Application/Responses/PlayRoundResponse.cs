using EWorldCup.Domain.Entities;

namespace EWorldCup.Application.Responses
{
    /// <summary>
    /// Response after the player makes a move in their current match
    /// </summary>
    public record PlayRoundResponse
    {
        /// <summary>
        /// The game round number that was just played (1, 2, or 3)
        /// </summary>
        public required int GameRoundNumber { get; init; }

        /// <summary>
        /// The player's choice
        /// </summary>
        public required RpsChoice PlayerChoice { get; init; }

        /// <summary>
        /// The opponent's choice
        /// </summary>
        public required RpsChoice OpponentChoice { get; init; }

        /// <summary>
        /// Result of this game round
        /// </summary>
        public required string Result { get; init; }

        /// <summary>
        /// Human-readable message (e.g., "You won! Rock beats Scissors")
        /// </summary>
        public required string Message { get; init; }

        /// <summary>
        /// Number of rounds the player has won in this match
        /// </summary>
        public required int PlayerWins { get; init; }

        /// <summary>
        /// Number of rounds the opponent has won in this match
        /// </summary>
        public required int OpponentWins { get; init; }

        /// <summary>
        /// True if the match is complete (someone won 2 rounds)
        /// </summary>
        public required bool MatchComplete { get; init; }

        /// <summary>
        /// Winner of the match (null if match not complete)
        /// </summary>
        public string? MatchWinner { get; init; }
    }
}
