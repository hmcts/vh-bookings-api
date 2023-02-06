﻿using BookingsApi.Common.Exceptions;
using BookingsApi.Common.Services;
using BookingsApi.Contract.Configuration;
using Microsoft.Extensions.Options;

namespace BookingsApi.Services
{
    public class FeatureFlagService : IFeatureFlagService
    {
        private readonly FeatureFlagConfiguration _featureFlagConfigurationOptions;
        private readonly IFeatureToggles _featureToggles;
        public FeatureFlagService(IOptions<FeatureFlagConfiguration> featureFlagConfigurationOptions, IFeatureToggles featureToggles)
        {
            _featureFlagConfigurationOptions = featureFlagConfigurationOptions.Value;
            _featureToggles = featureToggles;
        }
        
        public bool GetFeatureFlag(string featureName)
        {
            return featureName switch
            {
                nameof(FeatureFlags.StaffMemberFeature) => _featureFlagConfigurationOptions.StaffMemberFeature,
                nameof(FeatureFlags.EJudFeature) => _featureToggles.EJudFeature(),
                nameof(FeatureFlags.WorkAllocation) => _featureToggles.WorkAllocation(),
                _ => throw new FeatureFlagNotFoundException(featureName)
            };
        }
    }
}
