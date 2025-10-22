using EWorldCup.Api.DTO.Responses;
using EWorldCup.Api.Repositories;
using EWorldCup.Api.Validators;
using System.Diagnostics.CodeAnalysis;

namespace EWorldCup.Api.Services
{
    public class TournamentService : ITournamentService
    {
        private readonly IParticipantRepository _participants;
        private readonly IRoundRepository _rounds;

        public TournamentService(IParticipantRepository participants, IRoundRepository rounds)
        {
            _participants = participants;
            _rounds = rounds;
        }
        public async Task<ParticipantsResponse> GetParticipantsAsync(CancellationToken ct)
        {
            var list = await _participants.GetAllAsync();
            var dtos = list.Select(p => new ParticipantDto(p.Id, p.Name)).ToList();
            return new ParticipantsResponse(dtos);
        }

        public int GetMaxRounds(int? n)
        {
            return _rounds.GetMaxRounds(n);
        }
        public Task<RoundResponse> GetRoundAsync(int round, CancellationToken ct)
        {
            var pairs = _rounds.GetRoundPairs(round);
            var resp = new RoundResponse { Round = round, Pairs = pairs };
            return Task.FromResult(resp);
        }

        public async Task<PlayerScheduleResponse> GetPlayerScheduleAsync(int i, CancellationToken ct)
        {
            var participants = await _participants.GetAllAsync(ct);
            var n = participants.Count;

            Guard.NEvenAndMin2(n);
            Guard.IndexWithin(n, i);

            var nameToIndex = new Dictionary<string, int>(StringComparer.Ordinal);
            for (int idx = 0; idx < n; idx++) nameToIndex[participants[idx].Name] = idx;

            var maxRounds = _rounds.GetMaxRounds(n);
            var items = new List<PlayerScheduleItemDto>(maxRounds);
            var myName = participants[i].Name;

            for (int d = 1; d <= maxRounds; d++)
            {
                var pairs = _rounds.GetRoundPairs(d, n);
                var pair = pairs.FirstOrDefault(p => p.Home == myName || p.Away == myName)
                           ?? throw new InvalidOperationException($"No pairing for player {i} in round {d}.");
                var oppName = pair.Home == myName ? pair.Away : pair.Home;
                var oppIndex = nameToIndex.TryGetValue(oppName, out var k) ? k : -1;

                items.Add(new PlayerScheduleItemDto { Round = d, OpponentIndex = oppIndex, Opponent = oppName });
            }

            return new PlayerScheduleResponse
            {
                N = n,
                PlayerIndex = i,
                Player = myName,
                Schedule = items
            };
        }

        public async Task<PlayerRoundResponse> GetPlayerInRoundAsync(int i, int d, CancellationToken ct)
        {
            var participants = await _participants.GetAllAsync(ct);
            var n = participants.Count;

            Guard.NEvenAndMin2(n);
            Guard.IndexWithin(n, i);
            Guard.RoundWithin(n, d);

            var myName = participants[i].Name;
            var nameToIndex = participants.Select((p, idx) => (p.Name, idx))
                                          .ToDictionary(t => t.Name, t => t.idx, StringComparer.Ordinal);

            var pairs = _rounds.GetRoundPairs(d, n);
            var pair = pairs.FirstOrDefault(p => p.Home == myName || p.Away == myName)
                       ?? throw new InvalidOperationException("Pair not found.");

            var oppName = pair.Home == myName ? pair.Away : pair.Home;
            var oppIndex = nameToIndex.TryGetValue(oppName, out var k) ? k : -1;

            return new PlayerRoundResponse
            {
                Round = d,
                PlayerIndex = i,
                Player = myName,
                OpponentIndex = oppIndex,
                Opponent = oppName
            };
        }


        public async Task<RemainingPairsResponse> GetRemainingPairsAsync(int? n, int? D, CancellationToken ct)
        {
            var count = n ?? await _participants.CountAsync(ct);
            Guard.NEvenAndMin2(count);

            var maxRounds = _rounds.GetMaxRounds(count);
            var d = D ?? 0;
            if (d < 0 || d > maxRounds) throw new ArgumentException($"D must be 0..{maxRounds}");

            var totalPairs = count * (count - 1) / 2;
            var played = d * (count / 2);
            var remaining = Math.Max(0, totalPairs - played);

            return new RemainingPairsResponse
            {
                N = count,
                RoundsPlayed = d,
                Remaining = remaining,
                TotalPairs = totalPairs
            };
        }

    }
}
