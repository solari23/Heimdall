using System;

namespace Heimdall.Server
{
    /// <summary>
    /// Encapsulates configuration settings for a Heimdall server.
    /// </summary>
    public class HeimdallServerConfig
    {
        /// <summary>
        /// The network port number to listen on for requests.
        /// </summary>
        public UInt16 Port { get; set; }

        /// <summary>
        /// The maximum number of concurrect connections to accept.
        /// </summary>
        public int MaxConnections { get; set; }
    }
}
