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
                code = new string(Enumerable.Range(0, 6)
                    .Select(_ => Chars[Random.Next(Chars.Length)])
                    .ToArray());
            }
            while (ConnectedUsers.Values.Contains(code));

            return code;
        }
    }
}
