using System;
using System.Collections.Generic;
using Microsoft.ApplicationInsights.DataContracts;

namespace BookingsApi.Common
{
    public interface ILogger
    {
        void TrackError(Exception ex, Dictionary<string, string> properties = null);
        void TrackPageView(string viewName);
        void TrackEvent(string eventName, Dictionary<string, string> properties = null);
        void TrackDependency(string dependencyType, string dependencyName, string data, DateTimeOffset startTime, TimeSpan duration, bool success);
        void TrackTrace(string message, SeverityLevel severityLevel, Dictionary<string, string> properties = null);
    }
}