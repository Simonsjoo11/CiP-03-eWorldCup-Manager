namespace EWorldCup.Application.DTOs
{
    public record PlayerDto
    {
        /// <summary>
        /// The ID of the player
        /// </summary>
        public required int Id { get; init; }

        /// <summary>
        /// The name of the player
        /// </summary>
        public required string Name { get; init; }
    }
}
