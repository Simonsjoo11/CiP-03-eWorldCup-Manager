using EWorldCup.Api.DTO.Responses;

namespace EWorldCup.Api.Services
{
    public interface ITournamentService
    {
        Task<ParticipantsResponse> GetParticipantsAsync(CancellationToken ct);

        // Rounds
        int GetMaxRounds(int? n);
        Task<RoundResponse> GetRoundAsync(int round, int? n, CancellationToken ct);

        // Player
        Task<PlayerScheduleResponse> GetPlayerScheduleAsync(int i, CancellationToken ct);
        Task<PlayerRoundResponse> GetPlayerInRoundAsync(int i, int d,CancellationToken ct);

        // Matches meta
        Task<RemainingPairsResponse> GetRemainingPairsAsync(int? n, int? D, CancellationToken ct);
    }
}
