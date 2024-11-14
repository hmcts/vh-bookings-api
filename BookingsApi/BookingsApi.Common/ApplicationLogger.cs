using System;
using System.Collections.Generic;
using System.Security.Principal;
using System.Text.Json;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.ApplicationInsights.Extensibility;

namespace BookingsApi.Common;

/// <summary>
/// The application logger class send telemetry to Application Insights.
/// </summary>
public static class ApplicationLogger
{
    private static readonly TelemetryClient TelemetryClient = InitTelemetryClient();
    private const string Event = nameof(Event);
    private static TelemetryClient InitTelemetryClient() {
        var config = TelemetryConfiguration.CreateDefault();
        var client = new TelemetryClient(config);
        return client;
    }

    public static void Trace(string traceCategory, string eventTitle, string information)
    {
        var traceTelemetry = new TraceTelemetry(traceCategory, severityLevel: SeverityLevel.Information);
        traceTelemetry.Properties.Add("Information", information);
        traceTelemetry.Properties.Add(Event, eventTitle);
        TelemetryClient.TrackTrace(traceTelemetry);
    }

    public static void TraceWithProperties(string traceCategory, string eventTitle, string user, IDictionary<string, string> properties)
    {
        var traceTelemetry = new TraceTelemetry(traceCategory, SeverityLevel.Information);

        traceTelemetry.Properties.Add(Event, eventTitle);

        traceTelemetry.Properties.Add("User", user);

        if (properties != null)
        {
            foreach (KeyValuePair<string, string> entry in properties)
            {
                traceTelemetry.Properties.Add(entry.Key, entry.Value);
            }
        }

        TelemetryClient.TrackTrace(traceTelemetry);
          
    }
   
    public static void TraceWithProperties(string traceCategory, string eventTitle, string user)
    {
        TraceWithProperties(traceCategory, eventTitle, user, null);
    }

    public static void TraceWithObject(string traceCategory, string eventTitle, string user, object valueToSerialized)
    {
        var traceTelemetry = new TraceTelemetry(traceCategory, SeverityLevel.Information);

        traceTelemetry.Properties.Add(Event, eventTitle);

        traceTelemetry.Properties.Add("User", user);

        if (valueToSerialized != null)
        {
            traceTelemetry.Properties.Add(valueToSerialized.GetType().Name, JsonSerializer.Serialize(valueToSerialized));
        }

        TelemetryClient.TrackTrace(traceTelemetry);
    }

    public static void TraceWithObject(string traceCategory, string eventTitle, string user)
    {
        TraceWithObject(traceCategory, eventTitle, user, null);
    }

    public static void TraceException(string traceCategory, string eventTitle, Exception exception, IPrincipal user, IDictionary<string, string> properties)
    {
        ArgumentNullException.ThrowIfNull(exception);

        var telemetryException = new ExceptionTelemetry(exception);

        telemetryException.Properties.Add(Event, traceCategory + " " + eventTitle);

        if (user is { Identity: not null })
        {
            telemetryException.Properties.Add("User", user.Identity.Name);
        }

        if (properties != null)
        {
            foreach (var entry in properties)
            {
                telemetryException.Properties.Add(entry.Key, entry.Value);
            }
        }

        TelemetryClient.TrackException(telemetryException);
    }

    public static void TraceException(string traceCategory, string eventTitle, Exception exception, IPrincipal user)
    {
        TraceException(traceCategory, eventTitle, exception, user, null);
    }

    public static void TraceEvent(string eventTitle, IDictionary<string, string> properties)
    {
        var telemetryEvent = new EventTelemetry(eventTitle);

        if (properties != null)
        {
            foreach (KeyValuePair<string, string> entry in properties)
            {
                telemetryEvent.Properties.Add(entry.Key, entry.Value);
            }
        }

        TelemetryClient.TrackEvent(telemetryEvent);
    }

    public static void TraceRequest(string operationName, DateTimeOffset startTime, TimeSpan duration, string responseCode, bool success)
    {
        var telemetryOperation = new RequestTelemetry(operationName, startTime, duration, responseCode, success);
        TelemetryClient.TrackRequest(telemetryOperation);
    }
}