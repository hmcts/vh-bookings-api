namespace BookingsApi.Common.Logging
{
    using System;
    using Microsoft.Extensions.Logging;

    public static partial class HearingLogger
    {
        // Trace log messages

        [LoggerMessage(
            EventId = 7000, 
            Level = LogLevel.Trace,
            Message = "HearingVenueCode {HearingVenueCode} does not exist")]
        public static partial void LogTraceHearingVenueCodeDoesNotExist(this ILogger logger, string hearingVenueCode);
  
    }
}