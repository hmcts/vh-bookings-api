using System;
using System.Collections.Generic;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.DataContracts;

namespace BookingsApi.Common
{
    public class Logger : ILogger
    {
        private readonly TelemetryClient _telemetryClient;

        public Logger(TelemetryClient telemetryClient)
        {
            _telemetryClient = telemetryClient;
        }
        
        public void TrackError(Exception ex, Dictionary<string, string> properties = null)
        {
            _telemetryClient.TrackException(ex, properties);
        }

        public void TrackPageView(string viewName)
        {
            _telemetryClient.TrackPageView(viewName);
        }

        public void TrackEvent(string eventName, Dictionary<string, string> properties = null)
        {
            _telemetryClient.TrackEvent(eventName, properties);
        }

        public void TrackDependency(string dependencyType, string dependencyName, string data, DateTimeOffset startTime, TimeSpan duration, bool success)
        {
            _telemetryClient.TrackDependency(dependencyType, dependencyName, data, startTime, duration, success);
        }

        public void TrackTrace(string message, SeverityLevel severityLevel, Dictionary<string, string> properties = null)
        {
            _telemetryClient.TrackTrace(message, severityLevel, properties);
        }
    }
}