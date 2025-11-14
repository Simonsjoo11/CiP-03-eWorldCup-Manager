namespace EWorldCup.Domain.Entities
{
    public class Player
    {
        /// <summary>
        /// The players ID
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Unique identifier for the player
        /// </summary>
        public Guid Uid { get; set; } = Guid.NewGuid();

        /// <summary>
        /// The players name
        /// </summary>
        public required string Name { get; set; } = string.Empty;
    }
}
