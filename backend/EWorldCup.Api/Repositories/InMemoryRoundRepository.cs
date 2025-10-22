using EWorldCup.Api.Models;
using System.Text.Json;

namespace EWorldCup.Api.Repositories
{
    public class InMemoryRoundRepository : IRoundRepository
    {
        private readonly IParticipantRepository _participantsRepo;

        public InMemoryRoundRepository(IParticipantRepository participantsRepo)
        {
            _participantsRepo = participantsRepo;
        }

        public int GetMaxRounds(int? n = null)
        {
            // Pull actual list once (sync-bridged since this interface is sync)
            var participants = _participantsRepo.GetAllAsync().GetAwaiter().GetResult();
            var count = n ?? participants.Count;

            ValidateN(count, participants.Count);
            return count - 1;
        }

        public List<MatchPair> GetRoundPairs(int round, int? n = null)
        {
            var participants = _participantsRepo.GetAllAsync().GetAwaiter().GetResult();
            var actualCount = participants.Count;
            var count = n ?? actualCount;

            ValidateN(count, actualCount);
            ValidateRound(round, count);

            // Circle method with one fixed player (index 0 fixed, 1..n-1 rotate)
            var players = Enumerable.Range(0, count).ToList();
            var fixedPlayer = players[0];
            var rotating = players.Skip(1).ToList();

            // Rotate (round - 1) steps
            var shift = (round - 1) % rotating.Count;
            var rotated = rotating.Skip(shift).Concat(rotating.Take(shift)).ToList();

            var pairs = new List<MatchPair>(count / 2);

            // First pair: Fixed vs last of rotated
            var firstAway = rotated[^1];
            pairs.Add(new MatchPair(
                NameOrDefault(participants, fixedPlayer),
                NameOrDefault(participants, firstAway)
            ));

            // Remaining pairs: mirrored from the ends inward (excluding last, already used)
            var halfMinusOne = (count / 2) - 1;
            for (int i = 0; i < halfMinusOne; i++)
            {
                var home = rotated[i];
                var away = rotated[rotated.Count - 2 - i];

                pairs.Add(new MatchPair(
                    NameOrDefault(participants, home),
                    NameOrDefault(participants, away)
                ));
            }

            return pairs;
        }

        private static void ValidateN(int requestedN, int available)
        {
            if (requestedN < 2) throw new ArgumentException("Number of participants must be at least 2.");
            if ((requestedN & 1) == 1) throw new ArgumentException("Number of participants must be even.");

            // If someone passes n > available participants, we can't map names for those indices.
            if (requestedN > available)
                throw new ArgumentException($"Requested n ({requestedN}) exceeds available participants ({available}).");
        }

        private static void ValidateRound(int round, int n)
        {
            if (round < 1 || round > n - 1)
                throw new ArgumentException($"Round must be between 1 and {n - 1}.");
        }

        private static string NameOrDefault(IReadOnlyList<Participant> list, int index)
        {
            // list is based on actual participants; index is guaranteed < n <= list.Count by ValidateN
            return list[index].Name ?? $"Player {index + 1}";
        }
    }
}
