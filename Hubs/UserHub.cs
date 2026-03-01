using Microsoft.AspNetCore.SignalR;
using System.Collections.Concurrent;
using Whispr.Models;

namespace Whispr.Hubs
{
    public class UserHub : Hub
    {
        private ConcurrentDictionary<string, UserInfo> ConnectedUsers = new ConcurrentDictionary<string, UserInfo>(); // group, users connection id
        private ConcurrentDictionary<string, PeerSession> UserGroups = new ConcurrentDictionary<string, PeerSession>(); // groupOwnerConnectionId, bothPeerInformation


        private readonly IServiceProvider _serviceProvider;
        private readonly IWebHostEnvironment _env;

        public UserHub(IServiceProvider serviceProvider, IWebHostEnvironment env)
        {
            _serviceProvider = serviceProvider;
            _env = env;
        }


        public async Task Register(string username)
        {
            var userInfo = new UserInfo
            {
                ConnectionId = Context.ConnectionId,
                Username = username,
            };

            ConnectedUsers.TryAdd(Context.ConnectionId, userInfo);
        }

        public async Task CreateUserGroup()
        {
            var user = ConnectedUsers[Context.ConnectionId];
            var session = new PeerSession { UserA = user };

            UserGroups.TryAdd(Context.ConnectionId, session);

            await Clients.Caller.SendAsync("SessionCreated", Context.ConnectionId); // send back to generate QR
        }

        public async Task JoinUserGroup(string creatorConnectionId)
        {
            if (!UserGroups.TryGetValue(creatorConnectionId, out var session))
            {
                await Clients.Caller.SendAsync("Error", "Session not found");
                return;
            }

            if (session.IsFull)
            {
                await Clients.Caller.SendAsync("Error", "Session is full");
                return;
            }

            var user = ConnectedUsers[Context.ConnectionId];

            if (!session.TryAdd(user))
            {
                await Clients.Caller.SendAsync("Error", "Failed, Please Try Again");
                return;
            }
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

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            // Remove user from registry
            if (ConnectedUsers.TryRemove(Context.ConnectionId, out var userInfo))
            {
                // Notify all remaining users
                await Clients.Others.SendAsync("UserLeft", new
                {
                    connectionId = Context.ConnectionId,
                    username = userInfo.Username,
                });
            }

            await base.OnDisconnectedAsync(exception);
        }
    }
}
