using BookingsApi.Domain.Enumerations;
using BookingsApi.Infrastructure.Services;

namespace BookingsApi.UnitTests.Infrastructure.Services;

public class EventDtoMappersTests
{
    
    [Test]
    public void should_map_judiciary_participant_to_dto()
    {
        var hearing = new VideoHearingBuilder(addJudge:false).WithCase().WithJudiciaryPanelMember().WithJudiciaryJudge().Build();
        var @case = hearing.GetCases()[0];
        foreach (var participant in hearing.JudiciaryParticipants)
        {
            var mappedParticipant =
                EventDtoMappers.MapToHearingConfirmationDto(hearing.Id, hearing.ScheduledDateTime, participant,
                    @case);
            var role = (participant.HearingRoleCode == JudiciaryParticipantHearingRoleCode.Judge)
                ? "Judge"
                : "Judicial Office Holder";
            mappedParticipant.UserRole.Should().Be(role);
            mappedParticipant.Username.Should().Be(participant.JudiciaryPerson.Email);
        }

        foreach (var participant in hearing.Participants)
        {
            var mappedParticipant =
                EventDtoMappers.MapToHearingConfirmationDto(hearing.Id, hearing.ScheduledDateTime, participant,
                    @case);
            mappedParticipant.UserRole.Should().Be(participant.HearingRole.UserRole.Name);
            mappedParticipant.Username.Should().Be(participant.Person.Username);
        }
    }
}