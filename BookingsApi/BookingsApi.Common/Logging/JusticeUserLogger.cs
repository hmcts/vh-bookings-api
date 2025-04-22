namespace BookingsApi.Common.Logging
{
    using System;
    using Microsoft.Extensions.Logging;

    public static partial class JusticeUserLogger
    {

        // Error log messages
        
        [LoggerMessage(
            EventId = 6001, 
            Level = LogLevel.Error,
            Message = "Detected an existing user for the username {Username}")]
        public static partial void LogErrorDetectedAnExisting(this ILogger logger, Exception ex, string username);
    }
}