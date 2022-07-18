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
    }

    public class FeatureToggles : IFeatureToggles
    {
        private readonly ILdClient _ldClient;
        private readonly User _user;
        private const string LdUser = "vh-booking-api";
        private const string AdminSearchToggleKey = "admin_search";
        private const string ReferenceDataToggleKey = "reference-data";
        public FeatureToggles(string sdkKey)
        {
            var config = LaunchDarkly.Sdk.Server.Configuration.Builder(sdkKey)
                .Logging(
                    Components.Logging(Logs.ToWriter(Console.Out)).Level(LogLevel.Warn)
                )
                .Build();
            _ldClient = new LdClient(config);
            _user = User.WithKey(LdUser);
        }

        public bool AdminSearchToggle() => _ldClient.BoolVariation(AdminSearchToggleKey, _user);
        public bool ReferenceDataToggle() => _ldClient.BoolVariation(ReferenceDataToggleKey, _user);
    }
}
 