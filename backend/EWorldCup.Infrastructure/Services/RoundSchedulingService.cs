using EWorldCup.Application.Interfaces;
using EWorldCup.Domain.ValueObjects;

namespace EWorldCup.Infrastructure.Services
{
    public class RoundSchedulingService : IRoundSchedulingService
    {

        /// <summary>
        /// Calculates the maximum number of rounds for n participants.
        /// In round-robin, each participant must play every other participant once.
        /// </summary>
        public int GetMaxRounds(int participantCount)
        {
            if (participantCount < 2)
            {
                throw new ArgumentException("Participant count must be at least 2", nameof(participantCount));
            }

            if (participantCount % 2 != 0)
            {
                throw new ArgumentException("Participant count must be even", nameof(participantCount));
            }

            return participantCount - 1;
        }

        /// <summary>
        /// Generates all match pairings for a specific round using the polygon method.
        /// Position 0 is fixed, other positions rotate clockwise.
        /// </summary>
        public List<MatchPair> GenerateRoundPairs(int round, IReadOnlyList<string> participantNames)
        {
            int n = participantNames.Count;

            // Validate inputs
            if (n < 2 || n % 2 != 0)
            {
                throw new ArgumentException("Number of participants must be even and at least 2");
            }

            if (round < 1 || round > n - 1)
            {
                throw new ArgumentException($"Round must be between 1 and {n - 1}", nameof(round));
            }

            // Create array of indices (0 to n-1)
            var indices = Enumerable.Range(0, n).ToArray();

            // Rotate indices for the current round (round 1 = no rotation)
            for (int r = 1; r < round; r++)
            {
                RotateIndices(indices);
            }

            // Create pairs: first half vs second half (mirrored)
            var pairs = new List<MatchPair>();
            int halfSize = n / 2;

            for (int i = 0; i < halfSize; i++)
            {
                int homeIndex = indices[i];
                int awayIndex = indices[n - 1 - i];

                pairs.Add(new MatchPair
                {
                    Home = participantNames[homeIndex],
                    Away = participantNames[awayIndex]
                });
            }

            return pairs;
        }

        /// <summary>
        /// Rotates participant indices using the polygon method.
        /// Position 0 stays fixed, positions 1 to n-1 rotate clockwise.
        /// </summary>
        private void RotateIndices(int[] indices)
        {
            if (indices.Length < 2) return;

            // Keep position 0 fixed
            // Rotate positions 1 through n-1 clockwise
            int last = indices[^1];

            for (int i = indices.Length - 1; i > 1; i--)
            {
                indices[i] = indices[i - 1];
            }

            indices[1] = last;
        }
    }
}
