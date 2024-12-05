using BookingsApi.Common.Helpers;
using BookingsApi.Domain;
using BookingsApi.Domain.Enumerations;
using BookingsApi.Infrastructure.Services;

namespace BookingsApi.UnitTests.Infrastructure.Services;

public class EventDtoMappersTests
{
    
    [TestCase(JudiciaryParticipantHearingRoleCode.Judge, HearingRoles.Judge)]
    [TestCase(JudiciaryParticipantHearingRoleCode.PanelMember, HearingRoles.JudicialOfficeHolder)]
    public void should_map_hearing_confirmation_dto_with_judiciary_participant(JudiciaryParticipantHearingRoleCode hearingRoleCode, string expectedRoleName)
    {
        var @case = new Case("case number", "case name")
        {
            IsLeadCase = true
        };

        var judiciaryPerson = new JudiciaryPersonBuilder("Personal Code").Build();
        var hearingId = Guid.NewGuid();
        var scheduledTime = DateTime.Now.AddDays(2);
        
        var judiciaryParticipant = new JudiciaryParticipant("DisplayName", judiciaryPerson, hearingRoleCode);
        var mappedParticipant = EventDtoMappers.MapToHearingConfirmationDto(hearingId, scheduledTime, judiciaryParticipant, @case);
        
        mappedParticipant.HearingId.Should().Be(hearingId);

        mappedParticipant.ScheduledDateTime.Should().Be(scheduledTime);
        mappedParticipant.CaseName.Should().Be(@case.Name);
        mappedParticipant.CaseNumber.Should().Be(@case.Number);
        mappedParticipant.DisplayName.Should().Be(judiciaryParticipant.DisplayName);
        mappedParticipant.Representee.Should().Be("");
        mappedParticipant.Username.Should().Be(judiciaryParticipant.JudiciaryPerson.Email);
                    
        mappedParticipant.ContactEmail.Should().Be(judiciaryParticipant.JudiciaryPerson.Email);
        mappedParticipant.ContactTelephone.Should().Be(judiciaryParticipant.JudiciaryPerson.WorkPhone);
        mappedParticipant.FirstName.Should().Be(judiciaryParticipant.JudiciaryPerson.KnownAs);
        mappedParticipant.LastName.Should().Be(judiciaryParticipant.JudiciaryPerson.Surname);
        mappedParticipant.ParticipantId.Should().Be(judiciaryParticipant.Id);
        mappedParticipant.UserRole.Should().Be(expectedRoleName);
    }
}