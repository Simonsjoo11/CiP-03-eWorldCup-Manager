using EWorldCup.Application.Interfaces;
using EWorldCup.Domain.Entities;

namespace EWorldCup.Infrastructure.Services
{
    /// <summary>
    /// Service for Rock-Paper-Scissors game logic and simulation
    /// </summary>
    public class RpsGameService : IRpsGameService
    {
        private readonly Random _random;

        public RpsGameService()
        {
            _random = Random.Shared;
        }

        /// <summary>
        /// Generates a random RPS choice for AI players
        /// </summary>
        /// <returns>Random choice (Rock, Paper, or Scissors)</returns>
        public RpsChoice GenerateRandomChoice()
        {
            return (RpsChoice)_random.Next(0, 3);
        }

        /// <summary>
        /// Determines the winner of a single RPS round
        /// Uses circular logic: Rock beats Scissors, Scissors beats Paper, Paper beats Rock
        /// </summary>
        /// <param name="choice1">Player 1's choice</param>
        /// <param name="choice2">Player 2's choice</param>
        /// <returns>Result of the round</returns>
        public RoundResult DetermineWinner(RpsChoice choice1, RpsChoice choice2)
        {
            if (choice1 == choice2)
            {
                return RoundResult.Draw;
            }

            // Rock (0) beats Scissors (2)
            // Paper (1) beats Rock (0)
            // Scissors (2) beats Paper (1)

            bool choice1Wins = (choice1 == RpsChoice.Rock && choice2 == RpsChoice.Scissors) ||
                               (choice1 == RpsChoice.Paper && choice2 == RpsChoice.Rock) ||
                               (choice1 == RpsChoice.Scissors && choice2 == RpsChoice.Paper);

            return choice1Wins ? RoundResult.Player1Win : RoundResult.Player2Win;

        }

        /// <summary>
        /// Simulates a complete best-of-3 match between two AI players
        /// Plays rounds until one player wins 2 rounds
        /// </summary>
        /// <returns>List of GameRound entities (2-3 rounds)</returns>
        public List<GameRound> SimulateMatch()
        {
            var gameRounds = new List<GameRound>();
            int player1Wins = 0;
            int player2Wins = 0;
            int roundNumber = 1;

            // Play until someone wins 2 rounds (best-of-3)
            while (player1Wins < 2 && player2Wins < 2)
            {
                var choice1 = GenerateRandomChoice();
                var choice2 = GenerateRandomChoice();
                var result = DetermineWinner(choice1, choice2);

                var gameRound = new GameRound
                {
                    RoundNumber = roundNumber,
                    Player1Choice = choice1,
                    Player2Choice = choice2,
                    Result = result,
                    PlayedAt = DateTime.UtcNow
                };

                gameRounds.Add(gameRound);

                // Update win counters (draws dont count toward the 2 wins)
                if (result == RoundResult.Player1Win)
                    player1Wins++;
                else if (result == RoundResult.Player2Win)
                    player2Wins++;

                roundNumber++;

                // Safety check to prevent infinite loop
                if (roundNumber > 100)
                    break;
            }
            return gameRounds;
        }

        /// <summary>
        /// Creates a single game round with specified choices (for human player moves)
        /// </summary>
        /// <param name="roundNumber">Which round this is (1, 2, or 3)</param>
        /// <param name="playerChoice">Human player's choice</param>
        /// <param name="opponentChoice">AI opponent's choice (random if not provided)</param>
        /// <returns>GameRound entity with result calculated</returns>
        public GameRound CreateGameRound(int roundNumber, RpsChoice playerChoice, RpsChoice? opponentChoice = null)
        {
            var aiChoice = opponentChoice ?? GenerateRandomChoice();
            var result = DetermineWinner(playerChoice, aiChoice);

            return new GameRound
            {
                RoundNumber = roundNumber,
                Player1Choice = playerChoice,
                Player2Choice = aiChoice,
                Result = result,
                PlayedAt = DateTime.UtcNow
            };
        }
    }
}
