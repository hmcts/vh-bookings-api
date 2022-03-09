﻿using LaunchDarkly.Sdk;
using LaunchDarkly.Sdk.Server;
using LaunchDarkly.Sdk.Server.Interfaces;

namespace BookingsApi.Common.Services
{
    public interface IFeatureToggles
    {
        public bool AdminSearchToggle();
    }

    public class FeatureToggles : IFeatureToggles
    {
        private readonly ILdClient _ldClient;
        private readonly User _user;
        private const string LdUser = "vh-booking-api";
        private const string AdminSearchToggleKey = "admin_search";
        public FeatureToggles(string sdkKey)
        {
            _ldClient = new LdClient(sdkKey);
            _user = User.WithKey(LdUser);
        }

        public bool AdminSearchToggle() => _ldClient.BoolVariation(AdminSearchToggleKey, _user);
    }
}
 