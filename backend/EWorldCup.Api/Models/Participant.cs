namespace EWorldCup.Api.Models
{
    public class Participant
    {
        public int Id { get; set; }
        public Guid Uid { get; set; } = Guid.NewGuid();
        public string Name { get; set; } = string.Empty;
    }
}
