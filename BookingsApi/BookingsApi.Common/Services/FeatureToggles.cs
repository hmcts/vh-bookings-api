using System;
using LaunchDarkly.Logging;
using LaunchDarkly.Sdk;
using LaunchDarkly.Sdk.Server;
using LaunchDarkly.Sdk.Server.Interfaces;

namespace BookingsApi.Common.Services
{
    public interface IFeatureToggles
    {
        bool UsePostMay2023Template();
        bool MultiDayBookingEnhancementsEnabled();
    }

    public class FeatureToggles : IFeatureToggles
    {
        private readonly ILdClient _ldClient;
        private readonly Context _context;
        private const string LdUser = "vh-booking-api";
        private const string NewNotifyTemplatesToggleKey = "notify-post-may-2023-templates";
        private const string MultiDayBookingEnhancementsToggleKey = "multi-day-booking-enhancements";

        public FeatureToggles(string sdkKey, string environmentName)
        {
            var config = LaunchDarkly.Sdk.Server.Configuration.Builder(sdkKey)
                .Logging(Components.Logging(Logs.ToWriter(Console.Out)).Level(LogLevel.Warn)).Build();
            _context = Context.Builder(LdUser).Name(environmentName).Build();
            _ldClient = new LdClient(config);
        }
        
        public bool UsePostMay2023Template() => GetBoolToggle(NewNotifyTemplatesToggleKey);
        public bool MultiDayBookingEnhancementsEnabled() => GetBoolToggle(MultiDayBookingEnhancementsToggleKey);
        
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
 