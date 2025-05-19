namespace BookingsApi.Common.Logging
{
    using System;
    using Microsoft.Extensions.Logging;

    public static partial class AllocationLogger
    {
        [LoggerMessage(
            EventId = 5002, 
            Level = LogLevel.Information,
            Message = "Allocated hearing {HearingId} to cso {Cso}")]
        public static partial void LogInformationAllocatedHearingToCso(this ILogger logger, Guid hearingId, string cso);

         [LoggerMessage(
            EventId = 5003, 
            Level = LogLevel.Information,
            Message = "Deallocated hearing {HearingId}")]
        public static partial void LogInformationDeallocatedHearing(this ILogger logger, Guid hearingId);

    }
}