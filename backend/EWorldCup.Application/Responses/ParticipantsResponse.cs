using EWorldCup.Application.DTOs;

namespace EWorldCup.Application.Responses
{
    public record ParticipantsResponse
    {
        /// <summary>
        /// Total number of participants
        /// </summary>
        public required int ParticipantCount { get; init; }

        /// <summary>
        /// Maximum numper of rounds
        /// </summary>
        public required int MaxRounds { get; init; }

        /// <summary>
        /// List of all participants
        /// </summary>
        public List<ParticipantDto> Participants { get; init; } = [];
    }
}
