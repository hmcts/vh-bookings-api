namespace BookingsApi.Common.Services
{
    public interface IFeatureFlagService
    {
        bool GetFeatureFlag(string featureName);
    }
}