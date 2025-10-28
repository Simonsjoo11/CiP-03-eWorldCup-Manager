
namespace EWorldCup.Api.DTO.Responses
{
    public sealed class ParticipantDto
    {
        public int Id { get; init; }
        public Guid Uid { get; init; }

        public string Name { get; init; } = string.Empty;
    }

    public sealed class ParticipantsResponse
    {
        public IReadOnlyList<ParticipantDto> Participants { get; init; } = Array.Empty<ParticipantDto>();
    }
}
