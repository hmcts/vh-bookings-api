using BookingsApi.Common;
using BookingsApi.Domain;
using BookingsApi.Domain.Enumerations;
using BookingsApi.Domain.Participants;
using BookingsApi.Domain.RefData;
using BookingsApi.Infrastructure.Services;

namespace BookingsApi.UnitTests.Infrastructure.Services;

public class EventDtoMappersTests
{
    
    [TestCase(JudiciaryParticipantHearingRoleCode.Judge)]
    [TestCase(JudiciaryParticipantHearingRoleCode.PanelMember)]
    public void should_map_hearing_confirmation_dto_with_judiciary_participant(JudiciaryParticipantHearingRoleCode hearingRoleCode)
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
        var role = (judiciaryParticipant.HearingRoleCode == JudiciaryParticipantHearingRoleCode.Judge)
            ? "Judge"
            : "Judicial Office Holder";

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
        mappedParticipant.ParticipnatId.Should().Be(judiciaryParticipant.Id);
        mappedParticipant.UserRole.Should().Be(role);
    }
    
    [TestCase(CaseRoleGroup.Judge)]
    [TestCase(CaseRoleGroup.StaffMember)]
    public void should_map_hearing_confirmation_dto_with_participant(CaseRoleGroup hearingRoleCode)
    {
        var @case = new Case("case number", "case name")
        {
            IsLeadCase = true
        };

        
        var hearingId = Guid.NewGuid();
        var scheduledTime = DateTime.Now.AddDays(2);

        var caseRole = new CaseRole(1,"CaseRoleName");
        var hearingRole = new HearingRole(1, "RoleName");
        var person = new PersonBuilder(true).Build();

        Participant participant = null;
        switch (hearingRoleCode)
        {
            case CaseRoleGroup.Judge:
                participant = new Judge(person, hearingRole, caseRole)
                {
                    DisplayName = "Display Name",
                    CreatedBy = "Created By",
                    HearingRole = new HearingRole(1, "Judge") {UserRole = new UserRole(1, "Judge")}
                };
                break;
            case CaseRoleGroup.StaffMember:
                participant = new StaffMember(person, hearingRole, caseRole)
                {
                    DisplayName = "Display Name",
                    CreatedBy = "Created By",
                    HearingRole = new HearingRole(1, "StaffMember") {UserRole = new UserRole(1, "StaffMember")}
                };
                break;
        }

        var participantDto = ParticipantDtoMapper.MapToDto(participant, "");
        var mappedParticipant = EventDtoMappers.MapToHearingConfirmationDto(hearingId, scheduledTime, participantDto, @case);

        mappedParticipant.HearingId.Should().Be(hearingId);

        mappedParticipant.ScheduledDateTime.Should().Be(scheduledTime);
        mappedParticipant.CaseName.Should().Be(@case.Name);
        mappedParticipant.CaseNumber.Should().Be(@case.Number);
        mappedParticipant.DisplayName.Should().Be(participant.DisplayName);
        mappedParticipant.Representee.Should().Be("");
        mappedParticipant.Username.Should().Be(participant.Person.Username);
                    
        mappedParticipant.ContactEmail.Should().Be(participant.Person.ContactEmail);
        mappedParticipant.ContactTelephone.Should().Be(participant.Person.TelephoneNumber);
        mappedParticipant.FirstName.Should().Be(participant.Person.FirstName);
        mappedParticipant.LastName.Should().Be(participant.Person.LastName);
        mappedParticipant.ParticipnatId.Should().Be(participant.Id);
        mappedParticipant.UserRole.Should().Be(participant.HearingRole.UserRole.Name);
    }

    [Test]
    public void should_map_hearing_confirmation_dto_with_other_judge_email_and_telephone()
    {
        // Arrange
        var @case = new Case("case number", "case name")
        {
            IsLeadCase = true
        };
        
        var hearingId = Guid.NewGuid();
        var scheduledTime = DateTime.Now.AddDays(2);

        var caseRole = new CaseRole(1,"CaseRoleName");
        var hearingRole = new HearingRole(1, "RoleName");
        var person = new PersonBuilder(true).Build();

        var participant = new Judge(person, hearingRole, caseRole)
        {
            DisplayName = "Display Name",
            CreatedBy = "Created By",
            HearingRole = new HearingRole(1, "Judge") {UserRole = new UserRole(1, "Judge")}
        };

        const string otherInformation = "|JudgeEmail|otherjudgeemail@email.com|JudgePhone|0123456";
        
        var participantDto = ParticipantDtoMapper.MapToDto(participant, otherInformation);
        
        // Act
        var mappedParticipant = EventDtoMappers.MapToHearingConfirmationDto(hearingId, scheduledTime, participantDto, @case);
        
        // Assert
        mappedParticipant.HearingId.Should().Be(hearingId);
        mappedParticipant.ScheduledDateTime.Should().Be(scheduledTime);
        mappedParticipant.CaseName.Should().Be(@case.Name);
        mappedParticipant.CaseNumber.Should().Be(@case.Number);
        mappedParticipant.DisplayName.Should().Be(participant.DisplayName);
        mappedParticipant.Representee.Should().Be("");
        mappedParticipant.Username.Should().Be(participant.Person.Username);
        mappedParticipant.ContactEmail.Should().Be("otherjudgeemail@email.com");
        mappedParticipant.ContactTelephone.Should().Be("0123456");
        mappedParticipant.FirstName.Should().Be(participant.Person.FirstName);
        mappedParticipant.LastName.Should().Be(participant.Person.LastName);
        mappedParticipant.ParticipnatId.Should().Be(participant.Id);
        mappedParticipant.UserRole.Should().Be(participant.HearingRole.UserRole.Name);
    }

    [Test]
    public void should_map_welcome_email_dto_with_other_judge_email_and_telephone()
    {
        // Arrange
        var @case = new Case("case number", "case name")
        {
            IsLeadCase = true
        };
        
        var hearingId = Guid.NewGuid();

        var caseRole = new CaseRole(1,"CaseRoleName");
        var hearingRole = new HearingRole(1, "RoleName");
        var person = new PersonBuilder(true).Build();

        var participant = new Judge(person, hearingRole, caseRole)
        {
            DisplayName = "Display Name",
            CreatedBy = "Created By",
            HearingRole = new HearingRole(1, "Judge") {UserRole = new UserRole(1, "Judge")}
        };

        const string otherInformation = "|JudgeEmail|otherjudgeemail@email.com|JudgePhone|0123456";
        
        var participantDto = ParticipantDtoMapper.MapToDto(participant, otherInformation);
        
        // Act
        var mappedParticipant = EventDtoMappers.MapToWelcomeEmailDto(hearingId, participantDto, @case);
        
        // Assert
        mappedParticipant.HearingId.Should().Be(hearingId);
        mappedParticipant.CaseName.Should().Be(@case.Name);
        mappedParticipant.CaseNumber.Should().Be(@case.Number);
        mappedParticipant.ContactEmail.Should().Be("otherjudgeemail@email.com");
        mappedParticipant.ContactTelephone.Should().Be("0123456");
        mappedParticipant.FirstName.Should().Be(participant.Person.FirstName);
        mappedParticipant.LastName.Should().Be(participant.Person.LastName);
        mappedParticipant.ParticipnatId.Should().Be(participant.Id);
        mappedParticipant.UserRole.Should().Be(participant.HearingRole.UserRole.Name);
    }
}