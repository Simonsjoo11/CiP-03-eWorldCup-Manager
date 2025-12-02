namespace EWorldCup.Domain.Entities
{
    /// <summary>
    /// Represents one round within a match (one rock-paper-scissors throw)
    /// A match consists of up to 3 GameRounds (best-of-3)
    /// </summary>
    public class GameRound
    {
        /// <summary>
        /// Primary key
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Foreign key to the parent Match
        /// </summary>
        public int MatchId { get; set; }

        /// <summary>
        /// Which round within the match (1, 2, or 3)
        /// </summary>
        public int RoundNumber { get; set; }

        /// <summary>
        /// Player 1's choice (Rock, Paper, or Scissors)
        /// </summary>
        public RpsChoice Player1Choice { get; set; }

        /// <summary>
        /// Player 2's choice (Rock, Paper, or Scissors)
        /// </summary>
        public RpsChoice Player2Choice { get; set; }

        /// <summary>
        /// Result of this round
        /// </summary>
        public RoundResult Result { get; set; }

        /// <summary>
        /// When this round was played (for audit trail)
        /// </summary>
        public DateTime PlayedAt { get; set; }
    }

    /// <summary>
    /// Rock-Paper-Scissors choice options
    /// </summary>
    public enum RpsChoice
    {
        /// <summary>
        /// Rock beats Scissors
        /// </summary>
        Rock = 0,

        /// <summary>
        /// Paper beats Rock
        /// </summary>
        Paper = 1,

        /// <summary>
        /// Scissors beats Paper
        /// </summary>
        Scissors = 2
    }

    /// <summary>
    /// Result of a single game round
    /// </summary>
    public enum RoundResult
    {
        /// <summary>
        /// Player 1 won this round
        /// </summary>
        Player1Win = 0,

        /// <summary>
        /// Player 2 won this round
        /// </summary>
        Player2Win = 1,

        /// <summary>
        /// Both players chose the same (round is a draw)
        /// </summary>
        Draw = 2
    }
}
