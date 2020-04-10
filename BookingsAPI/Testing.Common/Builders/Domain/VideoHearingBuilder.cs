using System;
using System.Linq;
using Bookings.Domain;
using Bookings.Domain.Enumerations;
using Bookings.Domain.RefData;
using FizzWare.NBuilder;

namespace Testing.Common.Builders.Domain
{
    public class VideoHearingBuilder
    {
        private readonly string _caseTypeName = "Civil Money Claims";
        private readonly string _hearingTypeName = "Application to Set Judgment Aside";
        private readonly string _hearingVenueName = "Birmingham Civil and Family Justice Centre";
        
        private readonly VideoHearing _videoHearing;
        private readonly Person _judgePerson;

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

            _videoHearing =  Builder<VideoHearing>.CreateNew().WithFactory(() =>
                new VideoHearing(caseType, hearingType, scheduledDateTime, duration, venue, hearingRoomName, 
                    otherInformation, createdBy, questionnaireNotRequired, audioRecordingRequired, cancelReason)).Build();

            var claimantCaseRole = new CaseRole(1, "Claimant") { Group = CaseRoleGroup.Claimant };
            var defendantCaseRole = new CaseRole(2, "Defendant") { Group = CaseRoleGroup.Defendant };
            var claimantLipHearingRole = new HearingRole(1, "Claimant LIP") { UserRole = new UserRole(1, "Individual")};
            var defendantRepresentativeHearingRole =  new HearingRole(5, "Representative") { UserRole = new UserRole(1, "Representative") };

            var defendantLipHearingRole =  new HearingRole(4, "Defendant LIP") { UserRole = new UserRole(1, "Individual") };
            var judgeCaseRole = new CaseRole(5, "Judge") { Group = CaseRoleGroup.Judge };
            var judgeHearingRole = new HearingRole(13, "Judge") { UserRole = new UserRole(1, "Judge") }; 

            var person1 = new PersonBuilder(true).Build();
            var person2 = new PersonBuilder(true).Build();
            var person3 = new PersonBuilder(true).Build();
            _judgePerson = new PersonBuilder(true).Build();

            _videoHearing.AddIndividual(person1, claimantLipHearingRole, claimantCaseRole,
                $"{person1.FirstName} {person1.LastName}");

            _videoHearing.AddIndividual(person3, defendantLipHearingRole, defendantCaseRole,
                $"{person3.FirstName} {person3.LastName}");
            
            _videoHearing.AddRepresentative(person2, defendantRepresentativeHearingRole, defendantCaseRole,
                $"{person2.FirstName} {person2.LastName}", string.Empty, string.Empty);

            _videoHearing.AddJudge(_judgePerson, judgeHearingRole, judgeCaseRole,
                $"{_judgePerson.FirstName} {_judgePerson.LastName}");

            // Set the navigation properties as well since these would've been set if we got the hearing from DB
            _videoHearing.SetProtected(nameof(_videoHearing.HearingType), hearingType);
            _videoHearing.SetProtected(nameof(_videoHearing.CaseType), caseType);
            _videoHearing.SetProtected(nameof(_videoHearing.HearingVenue), venue);

            
        }

        public Person Judge => _judgePerson;
        
        public VideoHearing Build() => _videoHearing;
    }
}