namespace Heimdall.Server
{
    /// <summary>
    /// The intarface of a utility which can be used by a Heimdall server to
    /// log information during execution.
    /// </summary>
    /// <remarks>
    /// The implementation must ensure that all methods are thread-safe!
    /// </remarks>
    public interface IHeimdallLogger
    {
        /// <summary>
        /// Logs details about incoming requests.
        /// </summary>
        /// <param name="requestDetails">The details of the request.</param>
        void LogPerRequestInformation(string requestDetails);

        /// <summary>
        /// Logs an informational message.
        /// </summary>
        /// <param name="message">The message to log.</param>
        void LogInformation(string message);

        /// <summary>
        /// Logs details about an error.
        /// </summary>
        /// <param name="message">The message to log.</param>
        void LogError(string message);

        /// <summary>
        /// Logs warning messages about unusual occurrences during operation.
        /// </summary>
        /// <param name="message">The message to log.</param>
        void LogWarning(string message);
    }
}
