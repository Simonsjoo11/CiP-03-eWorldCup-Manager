namespace EWorldCup.Api.Models
{
    public class MatchPair
    {
        public string Home { get; init; } = string.Empty;
        public string Away {  get; init; } = string.Empty;

        public MatchPair(string home, string away) 
        {
            Home = home;
            Away = away;
        }
    }
}
