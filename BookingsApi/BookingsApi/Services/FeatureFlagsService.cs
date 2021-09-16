using BookingsApi.Contract.Configuration;
using Microsoft.Extensions.Options;

namespace BookingsApi.Services
{
    public interface IFeatureFlagsService
    {
        FeatureToggleConfiguration GetFeatureFlags(); 
    }

    public class FeatureFlagsService : IFeatureFlagsService
    {
        private readonly FeatureToggleConfiguration _featureToggleConfiguration;
        public FeatureFlagsService(IOptions<FeatureToggleConfiguration> options)
        {
            _featureToggleConfiguration = options.Value;
        }

        public FeatureToggleConfiguration GetFeatureFlags()
        {
            return _featureToggleConfiguration;
        }
    }
}
