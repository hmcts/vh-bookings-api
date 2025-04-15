namespace BookingsApi.Common.Logging
{
    using System;
    using Microsoft.Extensions.Logging;

    public static partial class WorkAllocationsControllerV2Logger
    {
       
        [LoggerMessage(
            EventId = 5001, 
            Level = LogLevel.Information,
            Message = "[GetUnallocatedHearings] Could not find any unallocated hearings")]
        public static partial void LogInformationCouldNotFindAnyUnallocatedHearings(this ILogger logger);

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