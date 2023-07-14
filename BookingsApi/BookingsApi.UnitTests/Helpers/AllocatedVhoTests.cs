using BookingsApi.Domain;
using BookingsApi.Domain.Helper;
using FluentAssertions;
using NUnit.Framework;
using Testing.Common.Builders.Domain;

namespace BookingsApi.UnitTests.Helpers;

public class AllocatedVhoTests
{
    [Test]
    public void should_return_not_required_if_venue_does_not_support_work_allocation()
    {
        // arrange
        var hearing = new VideoHearingBuilder().Build();
        var venue = new HearingVenue(1, "test", isScottish:false, isWorkAllocationEnabled:false);
        hearing.SetProtected(nameof(Hearing.HearingVenue), venue);

        // act
        var result = VideoHearingHelper.AllocatedVho(hearing);

        // assert
        result.Should().Be(VideoHearingHelper.NotRequired);
    }
    
    [Test]
    public void should_return_not_required_if_hearing_case_type_is_generic()
    {
        // arrange
        var hearing = new VideoHearingBuilder().Build();
        hearing.UpdateCase(new Case("3", "Generic"));
        var venue = new HearingVenue(1, "test", isScottish: false, isWorkAllocationEnabled: false);
        hearing.SetProtected(nameof(Hearing.HearingVenue), venue);

        // act
        var result = VideoHearingHelper.AllocatedVho(hearing);

        // assert
        result.Should().Be(VideoHearingHelper.NotRequired);
    }

    [Test]
    public void should_return_not_allocated_if_venue_supports_work_allocation_and_is_not_yet_allocation()
    {
        // arrange
        var hearing = new VideoHearingBuilder().Build();
        var venue = new HearingVenue(1, "test", isScottish: false, isWorkAllocationEnabled: true);
        hearing.SetProtected(nameof(Hearing.HearingVenue), venue);

        // act
        var result = VideoHearingHelper.AllocatedVho(hearing);

        // assert
        result.Should().Be(VideoHearingHelper.NotAllocated);
    }
    
    [Test]
    public void should_return_allocated_vho_if_venue_supports_work_allocation_and_is_allocated()
    {
        // arrange
        var hearing = new VideoHearingBuilder().WithAllocatedJusticeUser().Build();
        var venue = new HearingVenue(1, "test", isScottish: false, isWorkAllocationEnabled: true);
        hearing.SetProtected(nameof(Hearing.HearingVenue), venue);

        // act
        var result = VideoHearingHelper.AllocatedVho(hearing);

        // assert
        result.Should().Be(hearing.AllocatedTo.Username);
    }
}