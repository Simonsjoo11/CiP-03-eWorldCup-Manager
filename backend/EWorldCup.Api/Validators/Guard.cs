namespace EWorldCup.Api.Validators
{
    public static class Guard
    {
        public static void NEvenAndMin2(int n)
        {
            if (n < 2) throw new ArgumentException("n must be ≥ 2.");
            if ((n & 1) == 1) throw new ArgumentException("n must be even.");
        }
        public static void IndexWithin(int n, int i)
        {
            if (i < 0 || i >= n) throw new ArgumentException($"i must be 0..{n - 1}.");
        }
        public static void RoundWithin(int n, int d)
        {
            if (d < 1 || d > n - 1) throw new ArgumentException($"d must be 1..{n - 1}.");
        }
    }
}
