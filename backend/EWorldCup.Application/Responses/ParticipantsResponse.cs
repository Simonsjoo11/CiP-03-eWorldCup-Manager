using EWorldCup.Application.DTOs;

namespace EWorldCup.Application.Responses
{
    public record ParticipantsResponse
    {
        /// <summary>
        /// List of all participants
        /// </summary>
        public List<ParticipantDto> Participants { get; init; } = [];
    }
}
