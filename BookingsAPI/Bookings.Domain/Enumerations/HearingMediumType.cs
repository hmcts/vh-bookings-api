using System.ComponentModel;

namespace Bookings.Domain.Enumerations
{
    public enum HearingMediumType
    {
        [Description("Fully Video")]
        FullyVideo = 1,
        Telephone = 2,
        Physical = 3,
        Mixed = 4,
        VideoEnabled = 5
    }
}