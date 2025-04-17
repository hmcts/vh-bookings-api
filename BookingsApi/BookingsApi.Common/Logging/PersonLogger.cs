namespace BookingsApi.Common.Logging
{
    using System;
    using Microsoft.Extensions.Logging;

    public static partial class PersonLogger
    {
        // Information log messages

        [LoggerMessage(
            EventId = 8000, 
            Level = LogLevel.Debug,
            Message = "Updating {Count} non-anonymised participants")]
        public static partial void LogDebugUpdateNonAnonymised(this ILogger logger, int count);

    }
}