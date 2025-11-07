namespace EWorldCup.Domain.Entities
{
    public class Participant
    {
        /// <summary>
        /// The participants ID
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Unique identifier for the participant
        /// </summary>
        public Guid Uid { get; set; } = Guid.NewGuid();

        /// <summary>
        /// The participants name
        /// </summary>
        public required string Name { get; set; } = string.Empty;
    }
}
