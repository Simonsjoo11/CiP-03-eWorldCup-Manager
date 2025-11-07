using EWorldCup.Domain.ValueObjects;

namespace EWorldCup.Application.Interfaces
{
    public interface IRoundSchedulingService
    {
        /// <summary>
        /// Calculates the maximum number of rounds for n participants
        /// </summary>
        int GetMaxRounds(int participantCount);

        /// <summary>
        /// Generates all match pairings for a specific round using round-robin algorithm
        /// </summary>
        List<MatchPair> GenerateRoundPairs(int round, IReadOnlyList<string> participantNames);
    }
}
