namespace BookingsApi.Services
{
    public interface IFeatureFlagService
    {
        bool GetFeatureFlag(string featureName);
    }
}
