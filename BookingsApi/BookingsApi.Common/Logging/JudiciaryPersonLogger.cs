namespace BookingsApi.Common.Logging
{
    using System;
    using Microsoft.Extensions.Logging;

    public static partial class JudiciaryPersonLogger
    {
        // Information log messages

        [LoggerMessage(
            EventId = 5000, 
            Level = LogLevel.Information,
            Message = "Starting BulkJudiciaryPersons operation, processing {JudiciaryPersonRequestsCount} items")]
        public static partial void LogInformationStartingBulkJudiciaryPersonsOperationProcessing(this ILogger logger, int judiciaryPersonRequestsCount);

        // Error log messages
        
        [LoggerMessage(
            EventId = 6000, 
            Level = LogLevel.Error,
            Message = "Could not add or update external Judiciary user with Personal Code: {PersonalCode}")]
        public static partial void LogErrorCouldNotAddOrUpdate(this ILogger logger, Exception ex, string personalCode);
    }
}