using BookingsApi.Common.Services;

namespace Testing.Common.Stubs;

public class FeatureTogglesStub : IFeatureToggles
{
    public bool UseVodafone { get; set; } = false;
    
    public bool UseVodafoneToggle()
    {
        return UseVodafone;
    }
}