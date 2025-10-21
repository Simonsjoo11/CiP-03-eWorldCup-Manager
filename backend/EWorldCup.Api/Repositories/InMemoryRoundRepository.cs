using EWorldCup.Api.Models;
using System.Text.Json;

namespace EWorldCup.Api.Repositories
{
    public class InMemoryRoundRepository : IRoundRepository
    {
        private readonly List<Participant> _participants;

        public InMemoryRoundRepository()
        {
            _participants = LoadParticipants();
        }

        private static List<Participant> LoadParticipants()
        {
            var filePath = Path.Combine("Data", "participants.json");
            if (!File.Exists(filePath))
                return new List<Participant>();
            
            var json = File.ReadAllText(filePath);
            return JsonSerializer.Deserialize<List<Participant>>(
                json,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
                ) ?? new List<Participant>();
        }

        public int GetMaxRounds(int? n = null)
        {
            var count = n ?? _participants.Count;
            if (count < 2)
                throw new ArgumentException("Number of participants must be at least 2.");
            if (count % 2 != 0)
                throw new ArgumentException("Number of participants must be even.");
            
            return count - 1;
        }

        public List<MatchPair> GetRoundPairs(int round, int? n = null)
        {
            var count = n ?? _participants.Count;

            if (count < 2)
                throw new ArgumentException("Number of participants must be at least 2.");
            if (count % 2 != 0)
                throw new ArgumentException("Number of participants must be even.");
            if (round < 1 || round > count - 1)
                throw new ArgumentException($"Round must be between 1 and {count - 1}.");

            // Circle method with one fixed player
            var players = Enumerable.Range(0, count).ToList();
            var fixedPlayer = players[0];
            var rotating = players.Skip(1).ToList();

            // Rotate (round - 1) steps
            var shift = (round - 1) % rotating.Count;
            var rotated = rotating.Skip(shift).Concat(rotating.Take(shift)).ToList();

            var pairs = new List<MatchPair>();

            // First pair: Fixed vs last of rotated
            var firstAway = rotated[^1];
            pairs.Add(new MatchPair(
                _participants.ElementAtOrDefault(fixedPlayer)?.Name ?? $"Player {fixedPlayer + 1}",
                _participants.ElementAtOrDefault(firstAway)?.Name ?? $"Player {firstAway + 1}"
            ));

            // Remaining pairs: mirrored from the ends inward (expluding last, already used)
            var halfMinusOne = (count / 2) - 1;
            for (int i = 0; i < halfMinusOne; i++)
            {
                var home = rotated[i];
                var away = rotated[rotated.Count - 2 - i];

                var homeName = _participants.ElementAtOrDefault(home)?.Name ?? $"Player {home + 1}";
                var awayName = _participants.ElementAtOrDefault(away)?.Name ?? $"Player {away + 1}";
                pairs.Add(new MatchPair(homeName, awayName));
            }

            return pairs;
        }
    }
}
