using Microsoft.AspNetCore.SignalR;
using Whispr.Models.WebRTC;

namespace Whispr.Hubs
{
    public partial class UserHub
    {
        public async Task SendOffer(string targetConnectionId, RtcSessionDescription sdp)
        {
            await Clients.Client(targetConnectionId).SendAsync("ReceiveOffer", Context.ConnectionId, sdp);
        }

        public async Task SendAnswer(string targetConnectionId, RtcSessionDescription sdp)
        {
            await Clients.Client(targetConnectionId).SendAsync("ReceiveAnswer", Context.ConnectionId, sdp);
        }

        public async Task SendIceCandidate(string targetConnectionId, RtcIceCandidate candidate)
        {
            await Clients.Client(targetConnectionId).SendAsync("ReceiveIceCandidate", Context.ConnectionId, candidate);
        }

        
        public async Task SendFileOffer(string targetConnectionId, FileOfferDto offer)
        {
            await Clients.Client(targetConnectionId).SendAsync("ReceiveFileOffer", offer);
        }
        public async Task SendFileOfferResponse(string targetConnectionId, bool isOfferAccepted)
        {
            await Clients.Client(targetConnectionId).SendAsync("ReceiveFileOfferResponse", isOfferAccepted);
        }
    }
}
