using System;
using System.Net;

namespace Heimdall.Server
{
    /// <summary>
    /// Wrapper class around an underlying HTTP request, exposing features relevant to Heimdall requests.
    /// </summary>
    class HeimdallRequest
    {
        /// <summary>
        /// Gets the underlying HTTP request.
        /// </summary>
        public HttpListenerRequest HttpRequest { get; private set; }

        /// <summary>
        /// Gets this requests's unique identifier.
        /// </summary>
        public Guid RequestId { get; private set; }

        /// <summary>
        /// Constructs a HeimdallRequest object based on the given HTTP request.
        /// </summary>
        /// <param name="httpRequest">The underlying HTTP request.</param>
        public HeimdallRequest(HttpListenerRequest httpRequest)
        {
            HttpRequest = httpRequest ?? throw new ArgumentNullException(nameof(httpRequest));
            RequestId = Guid.NewGuid();
        }
    }
}
