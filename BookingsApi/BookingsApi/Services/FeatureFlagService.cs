using BookingsApi.Contract.Configuration;
using Microsoft.Extensions.Options;

namespace BookingsApi.Services
{
    public class FeatureFlagService : IFeatureFlagService
    {
        private readonly FeatureFlagConfiguration _featureFlagConfigurationOptions;
        public FeatureFlagService(IOptions<FeatureFlagConfiguration> featureFlagConfigurationOptions)
        {
            _featureFlagConfigurationOptions = featureFlagConfigurationOptions.Value;
        }

        public bool GetFeatureFlag(string featureName)
        {
            return featureName switch
            {
                nameof(FeatureFlags.StaffMemberFeature) => _featureFlagConfigurationOptions.StaffMemberFeature,
                nameof(FeatureFlags.EJudFeature) => _featureFlagConfigurationOptions.EJudFeature
            };
        }
    }
}
