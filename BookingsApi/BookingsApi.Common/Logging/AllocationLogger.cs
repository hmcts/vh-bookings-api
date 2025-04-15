namespace BookingsApi.Common.Logging
{
    using System;
    using Microsoft.Extensions.Logging;

    public static partial class AllocationsLogger
    {
        [LoggerMessage(
            EventId = 5000, 
            Level = LogLevel.Error,
            Message = "Unexpected error")]
        public static partial void LogErrorUnexpected(this ILogger logger, Exception ex);

    }
}