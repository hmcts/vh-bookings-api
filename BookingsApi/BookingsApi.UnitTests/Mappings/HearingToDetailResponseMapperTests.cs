using BookingsApi.Mappings;
using Bookings.Domain;
using Bookings.Domain.Participants;
using Bookings.Domain.RefData;
using FizzWare.NBuilder;
using FluentAssertions;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using BookingsApi.UnitTests.Utilities;
using Testing.Common.Builders.Domain;

namespace BookingsApi.UnitTests.Mappings
{
    public class HearingToDetailResponseMapperTests : TestBase
    {
        private readonly HearingToDetailResponseMapper _mapper = new HearingToDetailResponseMapper();
        private readonly string _caseTypeName = "Civil Money Claims";
        private readonly string _hearingTypeName = "Application to Set Judgment Aside";
        private readonly string _hearingVenueName = "Birmingham Civil and Family Justice Centre";

        private VideoHearing _videoHearing;
        protected virtual IList<Participant> Participants { get; set; }
        public DateTime UpdatedDate { get; protected set; }

        [Test]
        public void Should_map_all_properties()
        {
            var refDataBuilder = new RefDataBuilder();
            var venue = refDataBuilder.HearingVenues.First(x => x.Name == _hearingVenueName);
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

            var hearing = new VideoHearing(caseType, hearingType, scheduledDateTime, duration, venue, hearingRoomName,
                   otherInformation, createdBy, questionnaireNotRequired, audioRecordingRequired, cancelReason);
           
            _videoHearing = Builder<VideoHearing>.CreateNew().WithFactory(() =>
               hearing).Build();

            var claimantCaseRole = new CaseRole(1, "Claimant");
            var claimantLipHearingRole = new HearingRole(1, "Litigant in person") { UserRole = new UserRole(1, "Individual") };
            _videoHearing.AddCase("0875", "Test Case Add", false);

            var person1 = new PersonBuilder(true).Build();
            _videoHearing.AddIndividual(person1, claimantLipHearingRole, claimantCaseRole,
                $"{person1.FirstName} {person1.LastName}");

            var party = _videoHearing.GetParticipants().FirstOrDefault();
            party.SetProtected(nameof(party.CaseRole), claimantCaseRole);
            party.SetProtected(nameof(party.HearingRole), claimantLipHearingRole);

            var endpoints = new Endpoint("displayName", "333", "200", null);
            _videoHearing.AddEndpoint(endpoints);

            // Set the navigation properties as well since these would've been set if we got the hearing from DB
            _videoHearing.SetProtected(nameof(_videoHearing.HearingType), hearingType);
            _videoHearing.SetProtected(nameof(_videoHearing.CaseType), caseType);
            _videoHearing.SetProtected(nameof(_videoHearing.HearingVenue), venue);

            var response = _mapper.MapHearingToDetailedResponse(_videoHearing);
            response.Should().BeEquivalentTo(response, options => options
                .Excluding(v => v.Id)
            );
        }
    }
}
