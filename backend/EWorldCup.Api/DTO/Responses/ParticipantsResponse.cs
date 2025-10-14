namespace EWorldCup.Api.DTO.Responses
{   
    public record ParticipantDto(int Id, string Name);
    public record ParticipantsResponse(IReadOnlyList<ParticipantDto> Participants);
}
