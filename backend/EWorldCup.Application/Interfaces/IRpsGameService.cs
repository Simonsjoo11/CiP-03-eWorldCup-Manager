using EWorldCup.Domain.Entities;

namespace EWorldCup.Application.Interfaces
{
    /// <summary>
    /// Interface for Rock-Paper-Scissors game logic and simulation
    /// </summary>
    public interface IRpsGameService
    {
        /// <summary>
        /// Generates a random RPS choice for AI players
        /// </summary>
        /// <returns>Random choice (Rock, Paper, or Scissors)</returns>
        RpsChoice GenerateRandomChoice();

        /// <summary>
        /// Determines the winner of a single RPS round
        /// </summary>
        /// <param name="choice1">Player 1's choice</param>
        /// <param name="choice2">Player 2's choice</param>
        /// <returns>Result of the round (Player1Win, Player2Win, or Draw)</returns>
        RoundResult DetermineWinner(RpsChoice choice1, RpsChoice choice2);

        /// <summary>
        /// Simulates a complete best-of-3 match between two AI players
        /// </summary>
        /// <returns>List of GameRound entities (2-3 rounds)</returns>
        List<GameRound> SimulateMatch();

        /// <summary>
        /// Creates a single game round with specified choices (for human player moves)
        /// </summary>
        /// <param name="roundNumber">Which round this is (1, 2, or 3)</param>
        /// <param name="playerChoice">Human player's choice</param>
        /// <param name="opponentChoice">AI opponent's choice (random if not provided)</param>
        /// <returns>GameRound entity with result calculated</returns>
        GameRound CreateGameRound(int roundNumber, RpsChoice playerChoice, RpsChoice? opponentChoice = null);
    }
}
