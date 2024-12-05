using BookingsApi.Domain;
using BookingsApi.Domain.Enumerations;
using BookingsApi.Domain.RefData;
using FizzWare.NBuilder;
using System;
using System.Linq;

namespace Testing.Common.Builders.Domain
{
    public class VideoHearingBuilder
    {
        private readonly string _caseTypeName = "Generic";
        private readonly string _hearingVenueName = "Birmingham Civil and Family Justice Centre";
        
        private readonly VideoHearing _videoHearing;

        public VideoHearingBuilder(DateTime? scheduledDateTime = null, bool addJudge = true,
            bool treatPersonAsNew = true)
        {
            var defaultDate = DateTime.Today.AddDays(1).AddHours(11).AddMinutes(45);
            var refDataBuilder = new RefDataBuilder();
            var venue = refDataBuilder.HearingVenues.First(x => x.Name == _hearingVenueName);
            var caseType = new CaseType(1, _caseTypeName);
            
            var duration = 80;
            var hearingRoomName = "Roome03";
            var otherInformation = "OtherInformation03";
            var createdBy = "User03";
            const bool audioRecordingRequired = true;

            _videoHearing = Builder<VideoHearing>.CreateNew().WithFactory(() =>
                    new VideoHearing(caseType,
                        scheduledDateTime.GetValueOrDefault(defaultDate),
                        duration,
                        venue,
                        hearingRoomName,
                        otherInformation,
                        createdBy,
                        audioRecordingRequired))
                .Build();

            var applicantLipHearingRole = new HearingRole((int)HearingRoleIds.LitigantInPerson, "Litigant in person")
                { UserRole = new UserRole(1, "Individual") };
            var respondentRepresentativeHearingRole = new HearingRole(5, "Representative")
                { UserRole = new UserRole(1, "Representative") };

            var respondentLipHearingRole = new HearingRole(4, "Litigant in person")
                { UserRole = new UserRole(1, "Individual") };

            var person1 = new PersonBuilder(true, treatPersonAsNew: treatPersonAsNew).Build();
            var person2 = new PersonBuilder(true, treatPersonAsNew: treatPersonAsNew).Build();
            var person3 = new PersonBuilder(true, treatPersonAsNew: treatPersonAsNew).Build();

            _videoHearing.AddIndividual(Guid.NewGuid().ToString(), person1, applicantLipHearingRole, $"{person1.FirstName} {person1.LastName}");
            var indApplicant = _videoHearing.Participants[^1];
            indApplicant.SetProtected(nameof(indApplicant.HearingRole), applicantLipHearingRole);

            _videoHearing!.AddIndividual(Guid.NewGuid().ToString(), person3, respondentLipHearingRole, $"{person3.FirstName} {person3.LastName}");
            var indRespondent = _videoHearing.Participants[^1];
            indRespondent.SetProtected(nameof(indRespondent.HearingRole), respondentLipHearingRole);

            _videoHearing!.AddRepresentative(Guid.NewGuid().ToString(), person2, respondentRepresentativeHearingRole, $"{person2.FirstName} {person2.LastName}", string.Empty);
            var repRespondent = _videoHearing.Participants[^1];
            repRespondent.SetProtected(nameof(repRespondent.HearingRole), respondentRepresentativeHearingRole);

            if (addJudge)
            {
                var judiciaryPersonJudge = new JudiciaryPersonBuilder("judge123").Build();
                _videoHearing.AddJudiciaryJudge(judiciaryPersonJudge, "Judge Test");
            }

            var judiciaryPersonJoh = new JudiciaryPersonBuilder("judiciaryPanel321").Build();
            _videoHearing.AddJudiciaryPanelMember(judiciaryPersonJoh, "PanelMember Test");

            // Set the navigation properties as well since these would've been set if we got the hearing from DB
            _videoHearing.SetProtected(nameof(_videoHearing.CaseType), caseType);
            _videoHearing.SetProtected(nameof(_videoHearing.HearingVenue), venue);
        }

        public VideoHearingBuilder WithCase()
        {
            _videoHearing.AddCase("AutoTest", "Test", true);
            return this;
        }
        
        public VideoHearingBuilder WithAllocatedJusticeUser()
        {
            var justiceUser = new JusticeUser
            {
                ContactEmail = "contact@email.com",
                Username = "contact@email.com",
                CreatedBy = "integration.GetVhoWorkHoursQueryHandler.UnitTest",
                CreatedDate = DateTime.Now,
                FirstName = "test",
                Lastname = "test"
            };
            justiceUser.AddRoles(new UserRole((int) UserRoleId.VhTeamLead, "Video Hearings Team Lead"));
            _videoHearing.Allocations.Add(new Allocation {JusticeUser = justiceUser, Hearing = _videoHearing});
            return this;
        }

        public VideoHearingBuilder WithScheduledDateTime(DateTime startTime)
        {
            _videoHearing.SetProtected(nameof(_videoHearing.ScheduledDateTime), startTime);
            return this;
        }
        
        public VideoHearingBuilder WithDuration(int duration)
        {
            _videoHearing.SetProtected(nameof(_videoHearing.ScheduledDuration), duration);
            return this;
        }
        
        public VideoHearingBuilder WithJudiciaryJudge()
        {
            var personalCode = Guid.NewGuid().ToString();
            var judiciaryPerson = new JudiciaryPersonBuilder(personalCode: personalCode).Build();
            _videoHearing.AddJudiciaryJudge(judiciaryPerson, "Judge Test");
            return this;
        }

        public VideoHearingBuilder WithJudiciaryPanelMember()
        {
            var personalCode = Guid.NewGuid().ToString();
            var judiciaryPerson = new JudiciaryPersonBuilder(personalCode: personalCode).Build();
            _videoHearing.AddJudiciaryPanelMember(judiciaryPerson, "PanelMember Test");
            return this;
        }
        
        public VideoHearing Build() => _videoHearing;
    }
}