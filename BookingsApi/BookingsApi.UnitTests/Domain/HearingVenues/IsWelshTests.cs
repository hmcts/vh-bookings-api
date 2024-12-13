using System.Drawing;
using BookingsApi.Domain;

namespace BookingsApi.UnitTests.Domain.HearingVenues;

public class IsWelshTests
{
    [TestCase(null, false)]
    [TestCase("Wales", true)]
    [TestCase("Midlands", false)]
    public void should_check_if_region_is_welsh(string region, bool expected)
    {
        var hearingVenue = new HearingVenue(1, "Venue", false, true, "123")
        {
            Region = region
        };

        hearingVenue.IsWelsh().Should().Be(expected);
    }
}