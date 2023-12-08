using System.Collections.Generic;
using BookingsApi.Domain;
using BookingsApi.Domain.Participants;
using BookingsApi.Domain.RefData;
using BookingsApi.Mappings.Common;
using BookingsApi.Mappings.V1;
using BookingsApi.Mappings.V1.Extensions;
using BookingsApi.UnitTests.Utilities;
using FizzWare.NBuilder;

namespace BookingsApi.UnitTests.Mappings.V1
{
    public class HearingToDetailResponseMapperTests : TestBase
    {
        private readonly string _caseTypeName = "Generic";
        private readonly string _hearingTypeName = "Automated Test";
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
            const bool audioRecordingRequired = true;
            var cancelReason = "Online abandonment (incomplete registration)";

            var hearing = new VideoHearing(caseType,
                hearingType,
                scheduledDateTime,
                duration, 
                venue, 
                hearingRoomName,
                otherInformation,
                createdBy, 
                audioRecordingRequired, 
                cancelReason);
           
            _videoHearing = Builder<VideoHearing>.CreateNew().WithFactory(() =>
               hearing).Build();

            var applicantCaseRole = new CaseRole(1, "Applicant");
            var applicantLipHearingRole = new HearingRole(1, "Litigant in person") { UserRole = new UserRole(1, "Individual") };
            _videoHearing.AddCase("0875", "Test Case Add", false);

            var person1 = new PersonBuilder(true).Build();
            _videoHearing.AddIndividual(person1, applicantLipHearingRole, applicantCaseRole,
                $"{person1.FirstName} {person1.LastName}");

            var party = _videoHearing.GetParticipants().FirstOrDefault();
            party.SetProtected(nameof(party.CaseRole), applicantCaseRole);
            party.SetProtected(nameof(party.HearingRole), applicantLipHearingRole);

            var endpoints = new Endpoint("displayName", "333", "200", null);
            _videoHearing.AddEndpoint(endpoints);

            var judiciaryJudgePerson = new JudiciaryPersonBuilder(Guid.NewGuid().ToString()).Build();
            _videoHearing.AddJudiciaryJudge(judiciaryJudgePerson, "Judiciary Judge");
            
            var judiciaryPanelMemberPerson = new JudiciaryPersonBuilder(Guid.NewGuid().ToString()).Build();
            _videoHearing.AddJudiciaryPanelMember(judiciaryPanelMemberPerson, "Judiciary Panel Member");
            
            // Set the navigation properties as well since these would've been set if we got the hearing from DB
            _videoHearing.SetProtected(nameof(_videoHearing.HearingType), hearingType);
            _videoHearing.SetProtected(nameof(_videoHearing.CaseType), caseType);
            _videoHearing.SetProtected(nameof(_videoHearing.HearingVenue), venue);

            var response = HearingToDetailsResponseMapper.Map(_videoHearing);
            
            var caseMapper = new CaseToResponseMapper();
            var participantMapper = new ParticipantToResponseMapper();
            var judiciaryParticipantMapper = new JudiciaryParticipantToResponseMapper();
            
            response.Id.Should().Be(_videoHearing.Id);
            response.ScheduledDuration.Should().Be(_videoHearing.ScheduledDuration);
            response.ScheduledDateTime.Should().Be(_videoHearing.ScheduledDateTime);
            response.HearingTypeName.Should().Be(_videoHearing.HearingType.Name);
            response.CaseTypeName.Should().Be(_videoHearing.CaseType.Name);
            response.HearingVenueName.Should().Be(_videoHearing.HearingVenue.Name);
            response.Cases.Should().BeEquivalentTo(_videoHearing
                .GetCases()
                .Select(caseMapper.MapCaseToResponse)
                .ToList());
            response.Participants.Should().BeEquivalentTo(_videoHearing
                .GetParticipants()
                .Select(participantMapper.MapParticipantToResponse)
                .ToList());
            response.HearingRoomName.Should().Be(_videoHearing.HearingRoomName);
            response.OtherInformation.Should().Be(_videoHearing.OtherInformation);
            response.CreatedBy.Should().Be(_videoHearing.CreatedBy);
            response.CreatedDate.Should().Be(_videoHearing.CreatedDate);
            response.UpdatedBy.Should().Be(_videoHearing.UpdatedBy);
            response.UpdatedDate.Should().Be(_videoHearing.UpdatedDate);
            response.ConfirmedBy.Should().Be(_videoHearing.ConfirmedBy);
            response.ConfirmedDate.Should().Be(_videoHearing.ConfirmedDate);
            response.Status.Should().Be(_videoHearing.Status.MapToContractEnum());
            response.AudioRecordingRequired.Should().Be(_videoHearing.AudioRecordingRequired);
            response.CancelReason.Should().Be(_videoHearing.CancelReason);
            response.GroupId.Should().Be(_videoHearing.SourceId);
            response.Endpoints.Should().BeEquivalentTo(_videoHearing
                .GetEndpoints()
                .Select(EndpointToResponseMapper.MapEndpointToResponse)
                .ToList());
            response.JudiciaryParticipants.Should().BeEquivalentTo(_videoHearing
                .GetJudiciaryParticipants()
                .Select(judiciaryParticipantMapper.MapJudiciaryParticipantToResponse)
                .ToList());
        }
    }
}
