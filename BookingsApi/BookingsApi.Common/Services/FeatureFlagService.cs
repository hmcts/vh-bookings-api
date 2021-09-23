using BookingsApi.Common.Exceptions;
using BookingsApi.Contract.Configuration;
using Microsoft.Extensions.Options;

namespace BookingsApi.Common.Services
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
            switch (featureName)
            {
                case nameof(FeatureFlags.StaffMemberFeature):
                    return _featureFlagConfigurationOptions.StaffMemberFeature;
                case nameof(FeatureFlags.EJudFeature):
                    return _featureFlagConfigurationOptions.EJudFeature;
                default:
                    throw new FeatureFlagNotFoundException(featureName);
            }
        }
    }
}
