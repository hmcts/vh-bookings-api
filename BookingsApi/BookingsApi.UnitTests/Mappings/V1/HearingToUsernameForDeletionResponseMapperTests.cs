using System.Linq;
using BookingsApi.Domain;
using BookingsApi.Domain.RefData;
using BookingsApi.Mappings.V1;

namespace BookingsApi.UnitTests.Mappings.V1
{
    public class HearingToUsernameForDeletionResponseMapperTests
    {
        [Test]
        public void should_map_hearing_to_response_model_with_lead_case()
        {
            var hearing = BuildHearing(true);
            var leadCase = hearing.GetCases().First(x => x.IsLeadCase);
            
            var result = HearingToUsernameForDeletionResponseMapper.MapToDeletionResponse(hearing);

            result.HearingId.Should().Be(hearing.Id);
            result.Venue.Should().Be(hearing.HearingVenueName);
            result.ScheduledDateTime.Should().Be(hearing.ScheduledDateTime);
            result.CaseName.Should().Be(leadCase.Name);
            result.CaseNumber.Should().Be(leadCase.Number);
        }
        
        [Test]
        public void should_map_hearing_to_response_model_with_first_case()
        {
            var hearing = BuildHearing(false);
            var firstCase = hearing.GetCases().First();
            
            var result = HearingToUsernameForDeletionResponseMapper.MapToDeletionResponse(hearing);

            result.Venue.Should().Be(hearing.HearingVenueName);
            result.ScheduledDateTime.Should().Be(hearing.ScheduledDateTime);
            result.CaseName.Should().Be(firstCase.Name);
            result.CaseNumber.Should().Be(firstCase.Number);
        }

        [Test]
        public void should_map_hearing_to_response_model_without_case()
        {
            var hearing = new VideoHearingBuilder().Build(); 

            var result = HearingToUsernameForDeletionResponseMapper.MapToDeletionResponse(hearing);

            result.Venue.Should().Be(hearing.HearingVenueName);
            result.ScheduledDateTime.Should().Be(hearing.ScheduledDateTime);
            result.CaseName.Should().BeNullOrEmpty();
            result.CaseNumber.Should().BeNullOrEmpty();
        }


        [Test]
        public void should_map_hearing_to_response_model_with_first_case_from_multiple_cases()
        {
            var hearing = BuildHearing(false);
            hearing.AddCase("Test 002 ", "Case name 2", false);
            var firstCase = hearing.GetCases().First();

            var result = HearingToUsernameForDeletionResponseMapper.MapToDeletionResponse(hearing);

            result.Venue.Should().Be(hearing.HearingVenueName);
            result.ScheduledDateTime.Should().Be(hearing.ScheduledDateTime);
            result.CaseName.Should().Be(firstCase.Name);
            result.CaseNumber.Should().Be(firstCase.Number);
        }

        private static VideoHearing BuildHearing(bool leadCase)
        {
            var hearing = new VideoHearingBuilder().Build();
            hearing.AddCase("Test 001 ", "Case name", leadCase);
            foreach (var participant in hearing.Participants)
            {
                participant.HearingRole = new HearingRole(1, "Name") { UserRole = new UserRole(1, "Judge"), };
                participant.CaseRole = new CaseRole(1, "Name");
            }
            return hearing;
        }
    }
}