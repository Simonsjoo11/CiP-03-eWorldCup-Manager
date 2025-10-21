using EWorldCup.Api.Models;

namespace EWorldCup.Api.Repositories
{
    public interface IRoundRepository
    {
        int GetMaxRounds(int? n = null);
        List<MatchPair> GetRoundPairs(int round, int? n = null);
    }
}
