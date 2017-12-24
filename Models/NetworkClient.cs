using System;

namespace UnifiControllerCommunicator
{
    public class NetworkClient
    {
        public string Name { get; set; }
        public decimal? Signal { get; set; }
        public decimal? IdleTime { get; set; }
        public DateTime? LastSeen { get; set; }
    }
}
