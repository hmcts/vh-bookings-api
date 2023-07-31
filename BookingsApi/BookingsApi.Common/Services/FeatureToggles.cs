using System;
using LaunchDarkly.Logging;
using LaunchDarkly.Sdk;
using LaunchDarkly.Sdk.Server;
using LaunchDarkly.Sdk.Server.Interfaces;

namespace BookingsApi.Common.Services
{
    public interface IFeatureToggles
    {
        public bool AdminSearchToggle();
        public bool ReferenceDataToggle();
        public bool EJudFeature();
    }

    public class FeatureToggles : IFeatureToggles
    {
        private readonly ILdClient _ldClient;
        private readonly Context _context;
        private const string LdUser = "vh-booking-api";
        private const string AdminSearchToggleKey = "admin_search";
        private const string ReferenceDataToggleKey = "reference-data";
        private const string EJudFeatureKey = "ejud-feature";
        public FeatureToggles(string sdkKey, string environmentName)
        {
            var config = LaunchDarkly.Sdk.Server.Configuration.Builder(sdkKey)
                .Logging(Components.Logging(Logs.ToWriter(Console.Out)).Level(LogLevel.Warn)).Build();
            _ldClient = new LdClient(config);
            _context = Context.Builder(LdUser).Name(environmentName).Build();
            _ldClient = new LdClient(config);
        }

        public bool AdminSearchToggle() => GetBoolToggle(AdminSearchToggleKey);
        public bool ReferenceDataToggle() => GetBoolToggle(ReferenceDataToggleKey);
        public bool EJudFeature() => GetBoolToggle(EJudFeatureKey);
        
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
 