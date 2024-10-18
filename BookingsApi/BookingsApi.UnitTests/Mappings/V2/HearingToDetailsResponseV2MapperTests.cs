using BookingsApi.Contract.V2.Enums;
using BookingsApi.Domain;
using BookingsApi.Domain.Helper;
using BookingsApi.Mappings.Common;
using BookingsApi.Mappings.V2.Extensions;
using BookingsApi.Mappings.V2;

namespace BookingsApi.UnitTests.Mappings.V2;

public class HearingToDetailsResponseV2MapperTests
{
    [Test]
    public void Should_map_all_properties()
    {
        // Arrange
        var hearing = new VideoHearingBuilder()
            .WithCase()
            .WithAllocatedJusticeUser()
            .Build();

        // Act
        var result = HearingToDetailsResponseV2Mapper.Map(hearing);

        // Assert
        result.Id.Should().Be(hearing.Id);
        result.ScheduledDuration.Should().Be(hearing.ScheduledDuration);
        result.ScheduledDateTime.Should().Be(hearing.ScheduledDateTime);
        result.ServiceId.Should().Be(hearing.CaseType.ServiceId);
        result.ServiceName.Should().Be(hearing.CaseType.Name);
        result.HearingVenueCode.Should().Be(hearing.HearingVenue.VenueCode);
        result.HearingVenueName.Should().Be(hearing.HearingVenue.Name);
        result.IsHearingVenueScottish.Should().Be(hearing.HearingVenue.IsScottish);
        result.HearingRoomName.Should().Be(hearing.HearingRoomName);
        result.OtherInformation.Should().Be(hearing.OtherInformation);
        result.CreatedBy.Should().Be(hearing.CreatedBy);
        result.CreatedDate.Should().Be(hearing.CreatedDate);
        result.UpdatedBy.Should().Be(hearing.UpdatedBy);
        result.UpdatedDate.Should().Be(hearing.UpdatedDate);
        result.ConfirmedBy.Should().Be(hearing.ConfirmedBy);
        result.ConfirmedDate.Should().Be(hearing.ConfirmedDate);
        result.Status.Should().Be(hearing.Status.MapToContractEnum());
        result.AudioRecordingRequired.Should().Be(hearing.AudioRecordingRequired);
        result.CancelReason.Should().Be(hearing.CancelReason);
        result.GroupId.Should().Be(hearing.SourceId);
        result.BookingSupplier.Should().Be((BookingSupplier)hearing.ConferenceSupplier);
        result.SupportsWorkAllocation.Should().Be(hearing.HearingVenue.IsWorkAllocationEnabled);
        result.AllocatedToId.Should().Be(hearing.AllocatedTo.Id);
        result.AllocatedToUsername.Should().Be(hearing.AllocatedTo.Username);
        result.AllocatedToName.Should().Be($"{hearing.AllocatedTo.FirstName} {hearing.AllocatedTo.Lastname}");

        var cases = hearing.GetCases()
            .Select(CaseToResponseV2Mapper.MapCaseToResponse)
            .ToList();
        result.Cases.Should().BeEquivalentTo(cases);

        var participantMapper = new ParticipantToResponseV2Mapper();
        var participants = hearing.GetParticipants()
            .Select(x => participantMapper.MapParticipantToResponse(x))
            .ToList();
        result.Participants.Should().BeEquivalentTo(participants);

        var endpoints = hearing.GetEndpoints()
            .Select(EndpointToResponseV2Mapper.MapEndpointToResponse)
            .ToList();
        result.Endpoints.Should().BeEquivalentTo(endpoints);

        var judiciaryParticipantMapper = new JudiciaryParticipantToResponseMapper();
        var judiciaryParticipants = hearing.GetJudiciaryParticipants()
            .Select(x => judiciaryParticipantMapper.MapJudiciaryParticipantToResponse(x))
            .ToList();
        result.JudicialOfficeHolders.Should().BeEquivalentTo(judiciaryParticipants);
    }

    [Test]
    public void Should_map_without_allocated_to()
    {
        // Arrange
        var hearing = new VideoHearingBuilder()
            .WithCase()
            .Build();
        
        // Act
        var result = HearingToDetailsResponseV2Mapper.Map(hearing);

        // Assert
        result.AllocatedToId.Should().BeNull();
        result.AllocatedToUsername.Should().Be(VideoHearingHelper.NotAllocated);
        result.AllocatedToName.Should().Be(VideoHearingHelper.NotAllocated);
    }

    [Test]
    public void Should_map_for_venue_with_work_allocation_disabled()
    {
        // Arrange
        var hearing = new VideoHearingBuilder()
            .WithCase()
            .Build();
        
        var venue = new HearingVenue(1, "Venue", isWorkAllocationEnabled: false);
        hearing.SetProtected(nameof(hearing.HearingVenue), venue);
        
        // Act
        var result = HearingToDetailsResponseV2Mapper.Map(hearing);

        // Assert
        result.AllocatedToId.Should().BeNull();
        result.AllocatedToUsername.Should().Be(VideoHearingHelper.NotRequired);
        result.AllocatedToName.Should().Be(VideoHearingHelper.NotRequired);
    }

    [Test]
    public void Should_map_for_generic_case_type()
    {
        // Arrange
        var hearing = new VideoHearingBuilder()
            .WithCase()
            .Build();

        const int genericCaseTypeId = 3;
        hearing.CaseTypeId = genericCaseTypeId;
        
        // Act
        var result = HearingToDetailsResponseV2Mapper.Map(hearing);

        // Assert
        result.AllocatedToId.Should().BeNull();
        result.AllocatedToUsername.Should().Be(VideoHearingHelper.NotRequired);
        result.AllocatedToName.Should().Be(VideoHearingHelper.NotRequired);
    }
}