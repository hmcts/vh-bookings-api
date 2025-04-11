namespace BookingsApi.Common.Logging
{
    using System;
    using Microsoft.Extensions.Logging;

    public static partial class JusticeUserLogger
    {
        // Debug log messages
        [LoggerMessage(
            EventId = 8000, 
            Level = LogLevel.Debug,
            Message = "Updating {Count} non-anonymised participants")]
        public static partial void LogDebugUpdateNonAnonymised(this ILogger logger, int count);

        // Error log messages
        
        [LoggerMessage(
            EventId = 6001, 
            Level = LogLevel.Error,
            Message = "Detected an existing user for the username {Username}")]
        public static partial void LogErrorDetectedAnExisting(this ILogger logger, Exception ex, string username);
    }
}