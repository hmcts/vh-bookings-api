using System;
using System.Linq;
using BookingsApi.Domain;
using BookingsApi.Domain.Enumerations;
using BookingsApi.Domain.RefData;
using FizzWare.NBuilder;

namespace Testing.Common.Builders.Domain
{
    public class VideoHearingBuilder
    {
        private readonly string _caseTypeName = "Generic";
        private readonly string _hearingTypeName = "Automated Test";
        private readonly string _hearingVenueName = "Birmingham Civil and Family Justice Centre";
        
        private readonly VideoHearing _videoHearing;
        private readonly Person _judgePerson;
        private readonly Person _johPerson;
        private readonly Person _staffMemberPerson;

        public VideoHearingBuilder()
        {
            var refDataBuilder = new RefDataBuilder();
            var venue = refDataBuilder.HearingVenues.First( x=> x.Name == _hearingVenueName);
            var caseType = new CaseType(1, _caseTypeName);
            var hearingType = Builder<HearingType>.CreateNew().WithFactory(() => new HearingType(_hearingTypeName)).Build();
            var scheduledDateTime = DateTime.Today.AddDays(1).AddHours(11).AddMinutes(45);
            var duration = 80;
            var hearingRoomName = "Roome03";
            var otherInformation = "OtherInformation03";
            var createdBy = "User03";
            const bool questionnaireNotRequired = false;
            const bool audioRecordingRequired = true;
            var cancelReason = "Online abandonment (incomplete registration)";

            _videoHearing = Builder<VideoHearing>.CreateNew().WithFactory(() =>
                    new VideoHearing(caseType, hearingType, scheduledDateTime, duration, venue, hearingRoomName,
                        otherInformation, createdBy, questionnaireNotRequired, audioRecordingRequired, cancelReason))
                .Build();

            var applicantCaseRole = new CaseRole(1, "Applicant") { Group = CaseRoleGroup.Applicant };
            var respondentCaseRole = new CaseRole(2, "Respondent") { Group = CaseRoleGroup.Respondent };
            var applicantLipHearingRole = new HearingRole(1, "Litigant in person") { UserRole = new UserRole(1, "Individual")};
            var respondentRepresentativeHearingRole =  new HearingRole(5, "Representative") { UserRole = new UserRole(1, "Representative") };

            var respondentLipHearingRole =  new HearingRole(4, "Litigant in person") { UserRole = new UserRole(1, "Individual") };
            var judgeCaseRole = new CaseRole(5, "Judge") { Group = CaseRoleGroup.Judge };
            var judgeHearingRole = new HearingRole(13, "Judge") { UserRole = new UserRole(1, "Judge") }; 
            var staffMemberHearingRole = new HearingRole(727, nameof(UserRoleId.StaffMember)) { UserRole = new UserRole((int) UserRoleId.StaffMember, nameof(UserRoleId.StaffMember)) };
            var staffMemberCaseRole = new CaseRole(213, nameof(UserRoleId.StaffMember)) { Group = CaseRoleGroup.StaffMember };
            var johHearingRole = new HearingRole(14, "Judicial Office Holder") { UserRole = new UserRole( 7, "Judicial Office Holder")};
            var johCaseRole = new CaseRole(11, "Winger") { Group = CaseRoleGroup.Winger };

            var person1 = new PersonBuilder(true).Build();
            var person2 = new PersonBuilder(true).Build();
            var person3 = new PersonBuilder(true).Build();
            _judgePerson = new PersonBuilder(true).Build();
            _johPerson = new PersonBuilder(true).Build();
            _staffMemberPerson = new PersonBuilder(true).Build();

            _videoHearing.AddIndividual(person1, applicantLipHearingRole, applicantCaseRole,
                $"{person1.FirstName} {person1.LastName}");
            var indApplicant =_videoHearing?.Participants.Last();
            indApplicant.SetProtected(nameof(indApplicant.CaseRole), applicantCaseRole);
            indApplicant.SetProtected(nameof(indApplicant.HearingRole), applicantLipHearingRole);

            _videoHearing.AddIndividual(person3, respondentLipHearingRole, respondentCaseRole,
                $"{person3.FirstName} {person3.LastName}");
            var indRespondent =_videoHearing?.Participants.Last();
            indRespondent.SetProtected(nameof(indApplicant.CaseRole), respondentCaseRole);
            indRespondent.SetProtected(nameof(indRespondent.HearingRole), respondentLipHearingRole);
            
            _videoHearing.AddRepresentative(person2, respondentRepresentativeHearingRole, respondentCaseRole,
                $"{person2.FirstName} {person2.LastName}", string.Empty);
            var repRespondent =_videoHearing?.Participants.Last();
            repRespondent.SetProtected(nameof(indApplicant.CaseRole), respondentCaseRole);
            repRespondent.SetProtected(nameof(repRespondent.HearingRole), respondentRepresentativeHearingRole);
            
            _videoHearing.AddJudge(_judgePerson, judgeHearingRole, judgeCaseRole,
                $"{_judgePerson.FirstName} {_judgePerson.LastName}");
            var judge =_videoHearing?.Participants.Last();
            judge.SetProtected(nameof(indApplicant.CaseRole), judgeCaseRole);
            judge.SetProtected(nameof(judge.HearingRole), judgeHearingRole);

            _videoHearing.AddJudicialOfficeHolder(_johPerson, johHearingRole, johCaseRole,
                $"{_johPerson.FirstName} {_johPerson.LastName}");
            var joh = _videoHearing?.Participants.Last();
            joh.SetProtected(nameof(indApplicant.CaseRole), johCaseRole);
            joh.SetProtected(nameof(joh.HearingRole), johHearingRole);

            _videoHearing.AddStaffMember(_staffMemberPerson, staffMemberHearingRole, staffMemberCaseRole,
                $"{_staffMemberPerson.FirstName} {_staffMemberPerson.LastName}");
            var staffMember = _videoHearing?.Participants.Last();
            staffMember.SetProtected(nameof(indApplicant.CaseRole), staffMemberCaseRole);
            staffMember.SetProtected(nameof(staffMember.HearingRole), staffMemberHearingRole);

            // Set the navigation properties as well since these would've been set if we got the hearing from DB
            _videoHearing.SetProtected(nameof(_videoHearing.HearingType), hearingType);
            _videoHearing.SetProtected(nameof(_videoHearing.CaseType), caseType);
            _videoHearing.SetProtected(nameof(_videoHearing.HearingVenue), venue);
        }

        public VideoHearingBuilder WithCase()
        {
            _videoHearing.AddCase("AutoTest", "Test", true);
            return this;
        }
        public Person Judge => _judgePerson;

        public Person JudicialOfficeHolder => _johPerson;
        
        public VideoHearing Build() => _videoHearing;
    }
}