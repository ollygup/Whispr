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
            return false;
        }

        public UserInfo? GetPeer(string connectionId)
        {
            if (UserA?.ConnectionId == connectionId) return UserB;
            if (UserB?.ConnectionId == connectionId) return UserA;
            return null;
        }
    }
}