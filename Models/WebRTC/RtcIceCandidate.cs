namespace Whispr.Models.WebRTC
{
    public class RtcIceCandidate
    {
        public string? Candidate { get; set; }
        public string? SdpMid { get; set; }
        public int? SdpMLineIndex { get; set; }
    }
}
