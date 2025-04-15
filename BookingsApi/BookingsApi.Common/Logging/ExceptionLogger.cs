namespace BookingsApi.Common.Logging
{
    using System;
    using Microsoft.Extensions.Logging;

    public static partial class ExceptionLogger
    {
        [LoggerMessage(
            EventId = 8000, 
            Level = LogLevel.Error,
            Message = "CreateUser validation failed: {errors}")]
        public static partial void LogErrorCreateUserValidation(this ILogger logger, string errors);

    }
}