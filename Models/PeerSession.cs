namespace Whispr.Models
{
    public class PeerSession
    {
        public UserInfo? UserA { get; set; }
        public UserInfo? UserB { get; set; }
        public bool IsFull => UserA != null && UserB != null;

        public bool TryAdd(UserInfo user)
        {
            if (UserA == null) { UserA = user; return true; }
            if (UserB == null) { UserB = user; return true; }
            return false; // full
        }

        public UserInfo? GetPeer(string connectionId)
        {
            return UserA?.ConnectionId == connectionId ? UserB : UserA;
        }
    }
}
