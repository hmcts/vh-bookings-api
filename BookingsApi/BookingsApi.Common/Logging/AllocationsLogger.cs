namespace BookingsApi.Common.Logging
{
    using System;
    using Microsoft.Extensions.Logging;

    public static partial class AllocationsLogger
    {
        [LoggerMessage(
            EventId = 5000, 
            Level = LogLevel.Error,
            Message = "CreateUser validation failed: {errors}")]
        public static partial void LogErrorCreateUserValidation(this ILogger logger, string errors);

    }
}