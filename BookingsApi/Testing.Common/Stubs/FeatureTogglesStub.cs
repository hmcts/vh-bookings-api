using BookingsApi.Common.Services;

namespace Testing.Common.Stubs;

public class FeatureTogglesStub : IFeatureToggles
{
    public bool AdminSearch { get; set; } = true;
    public bool RefData { get; set; } = false;
    public bool NewTemplates { get; set; } = true;
    public bool UseVodafone { get; set; } = false;

    public bool AdminSearchToggle()
    {
        return AdminSearch;
    }

    public bool ReferenceDataToggle()
    {
        return RefData;
    }

    public bool UsePostMay2023Template()
    {
        return NewTemplates;
    }

    public bool UseVodafoneToggle()
    {
        return UseVodafone;
    }
}