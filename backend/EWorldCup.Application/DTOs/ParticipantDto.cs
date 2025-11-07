namespace EWorldCup.Application.DTOs
{
    public record ParticipantDto
    {
        /// <summary>
        /// The ID of the participant
        /// </summary>
        public required int Id { get; init; }

        /// <summary>
        /// The name of the participant
        /// </summary>
        public required string Name { get; init; }
    }
}
