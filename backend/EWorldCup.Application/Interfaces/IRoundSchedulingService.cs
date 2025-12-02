using EWorldCup.Domain.ValueObjects;

namespace EWorldCup.Application.Interfaces
{
    public interface IRoundSchedulingService
    {
        /// <summary>
        /// Calculates the maximum number of rounds for n players
        /// </summary>
        int GetMaxRounds(int playerCount);

        /// <summary>
        /// Generates all match pairings for a specific round using round-robin algorithm
        /// </summary>
        List<MatchPair> GenerateRoundPairs(int round, IReadOnlyList<string> playerNames);
    }
}
