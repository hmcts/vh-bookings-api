using System;
using System.Diagnostics.CodeAnalysis;
using LaunchDarkly.Logging;
using LaunchDarkly.Sdk;
using LaunchDarkly.Sdk.Server;

namespace BookingsApi.Common.Services
{
    public interface IFeatureToggles
    { 
    }

    public class FeatureToggles : IFeatureToggles
    {
        private readonly LdClient _ldClient;
        private readonly Context _context;
        private const string LdUser = "vh-booking-api";

        public FeatureToggles(string sdkKey, string environmentName)
        {
            var config = LaunchDarkly.Sdk.Server.Configuration.Builder(sdkKey)
                .Logging(Components.Logging(Logs.ToWriter(Console.Out)).Level(LogLevel.Warn)).Build();
            _context = Context.Builder(LdUser).Name(environmentName).Build();
            _ldClient = new LdClient(config);
        }
        
        [SuppressMessage("Major Code Smell", "S1144:Unused private methods should be removed", Justification = "Kept in for future implementations")]
        private bool GetBoolToggle(string key)
        {
            if (!_ldClient.Initialized)
            {
                throw new InvalidOperationException("LaunchDarkly client not initialized");
            }
            return _ldClient.BoolVariation(key, _context);
        }
    }
}
 