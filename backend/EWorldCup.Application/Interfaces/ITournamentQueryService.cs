using EWorldCup.Application.Responses;

namespace EWorldCup.Application.Interfaces
{
    public interface ITournamentQueryService
    {
        Task<ParticipantsResponse> GetParticipantAsync(CancellationToken ct = default);
        Task<RoundResponse> GetRoundAsync(int round, CancellationToken ct = default);
        Task<PlayerScheduleResponse> GetPlayerScheduleAsync(int playerIndex, CancellationToken ct = default);
        Task<PlayerRoundResponse> GetPlayerInRoundAsync(int playerIndex, int round, CancellationToken ct = default);

        Task<RemainingPairsResponse> GetRemainingPairsAsync(int? participantcount, int? roundsPlayed, CancellationToken ct = default);
    }
}
