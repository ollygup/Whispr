namespace Whispr.Hubs
{
    public partial class UserHub
    {
        private static readonly char[] Chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789".ToCharArray();
        private static readonly Random Random = new Random();

        private string GenerateUniqueCode()
        {
            string code;
            do
            {
                var part1 = new string(Enumerable.Range(0, 4)
                    .Select(_ => Chars[Random.Next(Chars.Length)])
                    .ToArray());
                var part2 = new string(Enumerable.Range(0, 4)
                    .Select(_ => Chars[Random.Next(Chars.Length)])
                    .ToArray());
                code = $"{part1}-{part2}";
            }
            while (ConnectedUsers.Values.Contains(code));

            return code;
        }
    }
}
