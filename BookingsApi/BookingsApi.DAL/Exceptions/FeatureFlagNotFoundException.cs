using System;

namespace BookingsApi.DAL.Exceptions
{
#pragma warning disable S3925 // "ISerializable" should be implemented correctly
    public class FeatureFlagNotFoundException : Exception
    {
        
        
        public FeatureFlagNotFoundException(string featureflagName) : base($"FeatureFlag '{featureflagName}' does not exist")
        {
        }
        
    }
}