using System;
using System.Linq;
using Bookings.Domain;
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

        public VideoHearingBuilder()
        {
            var refDataBuilder = new RefDataBuilder();
            var venue = refDataBuilder.HearingVenues.First( x=> x.Name == _hearingVenueName);
            var caseType = new CaseType(1, _caseTypeName);
            var hearingType = Builder<HearingType>.CreateNew().WithFactory(() => new HearingType(_hearingTypeName)).Build();
            var scheduledDateTime = DateTime.Today.AddDays(1).AddHours(11).AddMinutes(45);
            var duration = 80;

            _videoHearing =  Builder<VideoHearing>.CreateNew().WithFactory(() =>
                new VideoHearing(caseType, hearingType, scheduledDateTime, duration, venue)).Build();

            var claimantCaseRole = new CaseRole(1, "Claimant");
            var defendantCaseRole = new CaseRole(2, "Defendant");
            var claimantLipHearingRole = new HearingRole(1, "Claimant LIP");
            var defendantSolicitorHearingRole =  new HearingRole(5, "Solicitor");

            var defendantLipHearingRole =  new HearingRole(4, "Defendant LIP");
            
            var person1 = new PersonBuilder(true).Build();
            var person2 = new PersonBuilder(true).Build();
            var person3 = new PersonBuilder(true).Build();
            
            _videoHearing.AddIndividual(person1, claimantLipHearingRole, claimantCaseRole,
                $"{person1.FirstName} {person1.LastName}");

            _videoHearing.AddIndividual(person3, defendantLipHearingRole, defendantCaseRole,
                $"{person3.FirstName} {person3.LastName}");
            
            _videoHearing.AddSolicitor(person2, defendantSolicitorHearingRole, defendantCaseRole,
                $"{person2.FirstName} {person2.LastName}", string.Empty, string.Empty);
            
        }
        
        public VideoHearing Build() => _videoHearing;
    }
}