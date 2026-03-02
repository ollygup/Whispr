using Microsoft.AspNetCore.SignalR;
using System.Collections.Concurrent;
using Whispr.Models;

namespace Whispr.Hubs
{
    public partial class UserHub : Hub
    {
        private static ConcurrentDictionary<string, string> ConnectedUsers = new ConcurrentDictionary<string, string>(); // userConnectionId, userUniqueCode
        private static ConcurrentDictionary<string, PeerSession> UserGroups = new ConcurrentDictionary<string, PeerSession>(); // uniqueCode, bothPeerInformation

        private readonly IServiceProvider _serviceProvider;
        private readonly IWebHostEnvironment _env;

        public UserHub(IServiceProvider serviceProvider, IWebHostEnvironment env)
        {
            _serviceProvider = serviceProvider;
            _env = env;
        }


        public async Task Register()
        {
            var uniqueCode = GenerateUniqueCode();
            ConnectedUsers.TryAdd(Context.ConnectionId, uniqueCode);

            await CreateUserGroup();

            await Clients.Caller.SendAsync("ReceiveCode", uniqueCode);
            //await CreateUserGroup();
        }

        private async Task CreateUserGroup()
        {
            var userCode = ConnectedUsers[Context.ConnectionId];
            var userInfo = new UserInfo { ConnectionId = Context.ConnectionId, UserCode = userCode };
            var session = new PeerSession { UserA = userInfo };

            UserGroups.TryAdd(userCode, session);
        }

        public async Task JoinUserGroup(string uniqueCode)
        {
            if (!UserGroups.TryGetValue(uniqueCode, out var session))
            {
                await Clients.Caller.SendAsync("Error", "Session not found");
                return;
            }

            if (session.IsFull)
            {
                await Clients.Caller.SendAsync("Error", "Session is full");
                return;
            }

            var userCode = ConnectedUsers[Context.ConnectionId];
            var userInfo = new UserInfo { ConnectionId = Context.ConnectionId, UserCode = userCode };
            if (!session.TryAdd(userInfo))
            {
                await Clients.Caller.SendAsync("Error", "Failed, Please Try Again");
                return;
            }

            await Clients.Clients([session.UserA?.ConnectionId, session.UserB?.ConnectionId]).SendAsync("UserJoined", session);
        }
        public async Task LeaveGroup()
        {
            var myConnectionId = Context.ConnectionId;

            // Only the joiner (UserB) can explicitly leave — find the group they joined
            var entry = UserGroups.FirstOrDefault(kvp =>
                kvp.Value.UserB?.ConnectionId == myConnectionId);

            if (entry.Value == null) return;

            var session = entry.Value;
            var ownerConnectionId = session.UserA?.ConnectionId;

            // Remove joiner from session but keep the group alive for the owner
            session.UserB = null;

            // Notify owner
            if (ownerConnectionId != null)
                await Clients.Client(ownerConnectionId).SendAsync("PeerLeft");
        }

        public async Task SendUserReadyNotification(string toConnectionId)
        {
            // Verify sender exists
            if (!ConnectedUsers.ContainsKey(Context.ConnectionId))
            {
                await Clients.Caller.SendAsync("Error", "You must register first");
                return;
            }

            // Verify recipient exists
            if (!ConnectedUsers.ContainsKey(toConnectionId))
            {
                await Clients.Caller.SendAsync("Error", "User not found");
                return;
            }

            await Clients.Client(toConnectionId).SendAsync("ReceiveUserReadyNotification", true);
        }





        public override async Task OnConnectedAsync()
        {
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            var myConnectionId = Context.ConnectionId;

            if (ConnectedUsers.TryRemove(myConnectionId, out var userCode))
            {
                // Check if this user is a owner
                if (UserGroups.TryGetValue(userCode, out var session))
                {
                    var peerConnectionId = session.UserB?.ConnectionId;
                    // Owner left — remove group entirely
                    UserGroups.TryRemove(userCode, out _);
                    // Notify joiner
                    if (peerConnectionId != null)
                        await Clients.Client(peerConnectionId).SendAsync("PeerLeft");
                }

                // Check if this user is a joiner (UserB) in someone else's group
                var entry = UserGroups.FirstOrDefault(kvp =>
                    kvp.Value.UserB?.ConnectionId == myConnectionId);

                if (entry.Value != null)
                {
                    var ownerConnectionId = entry.Value.UserA?.ConnectionId;
                    // Remove joiner from group, keep group alive
                    entry.Value.UserB = null;
                    // Notify owner
                    if (ownerConnectionId != null)
                        await Clients.Client(ownerConnectionId).SendAsync("PeerLeft");
                }
            }

            await base.OnDisconnectedAsync(exception);
        }
    }
}
