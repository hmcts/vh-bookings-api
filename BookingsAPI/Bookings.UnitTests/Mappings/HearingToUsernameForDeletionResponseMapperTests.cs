using System.Linq;
using Bookings.API.Mappings;
using Bookings.Domain;
using Bookings.Domain.RefData;
using FluentAssertions;
using NUnit.Framework;
using Testing.Common.Builders.Domain;

namespace Bookings.UnitTests.Mappings
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