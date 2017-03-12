using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Heimdall.Server
{
    /// <summary>
    /// The main class implementing Heimdall server logic.
    /// </summary>
    public class HeimdallServer
    {
        /// <summary>
        /// Gets the server's configuration.
        /// </summary>
        public HeimdallServerConfig Config { get; private set; }

        /// <summary>
        /// Gets the helper used by the server to log information.
        /// </summary>
        public IHeimdallLogger Logger { get; private set; }

        private HttpListener _httpListener;

        /// <summary>
        /// Constructs a new HeimdallServer.
        /// </summary>
        /// <param name="config">The server's configuration.</param>
        /// <param name="logger">A logger object to use for logging.</param>
        public HeimdallServer(
            HeimdallServerConfig config,
            IHeimdallLogger logger)
        {
            Config = config ?? throw new ArgumentNullException(nameof(config));
            Logger = logger ?? throw new ArgumentNullException(nameof(logger));

            // Set up the HTTP Listener.
            //
            string listenUrl = $"http://+:{Config.Port}/";
            _httpListener = new HttpListener();
            _httpListener.Prefixes.Add(listenUrl);
        }

        /// <summary>
        /// Starts the Heimdall server. The server will begin listening for, and handling requests.
        /// </summary>
        /// <remarks>
        /// Before running the server, ensure that the server is allowed to listen for HTTP requests on the configured port,
        /// otherwise you will get an access violation error. To enable this server to listen for HTTP requests, run this
        /// command from an elevated command-line:
        /// 
        /// netsh http add urlacl url=http://+:{PORT}/ user={USER} listen=yes
        /// 
        /// Where:
        ///     {PORT} is the configured port number
        ///     {USER} is the user account that the server will be running as
        /// </remarks>
        public void Run()
        {
            Task.Run(() =>
            {
                Logger.LogInformation("Starting up Heimdall server.");
                _httpListener.Start();

                var semaphore = new Semaphore(Config.MaxConnections, Config.MaxConnections);

                while (true)
                {
                    semaphore.WaitOne();

                    _httpListener.GetContextAsync().ContinueWith(async (contextTask) =>
                    {
                        semaphore.Release();

                        try
                        {
                            var context = await contextTask;
                            await HandleIncomingRequest(context);
                        }
                        catch (Exception e)
                        {
                            Logger.LogError(e.ToString());
                        }
                    });
                }
            }).Wait();
        }

        private volatile int _requestCounter = 0;

        private async Task HandleIncomingRequest(HttpListenerContext context)
        {
            int requestNumber = Interlocked.Increment(ref _requestCounter);

            HeimdallRequest request = new HeimdallRequest(context.Request);

            Logger.LogPerRequestInformation($"Received {context.Request.HttpMethod} request. This is request {requestNumber} ({request.RequestId}).");
            string message = $"Hello! You are request #{requestNumber}. Your requestId is {request.RequestId}";
            byte[] messageBytes = Encoding.UTF8.GetBytes(message);

            Stream outputStream = context.Response.OutputStream;
            await outputStream.WriteAsync(messageBytes, 0, messageBytes.Length);
            outputStream.Close();
        }
    }
}
