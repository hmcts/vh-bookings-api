using Bookings.API.Mappings;
using Bookings.Domain;
using Bookings.Domain.RefData;
using Bookings.UnitTests.Utilities;
using FluentAssertions;
using NUnit.Framework;
using System.Linq;
using Testing.Common.Builders.Domain;

namespace Bookings.UnitTests.Mappings
{
    public class HearingByCaseNumberResponseMapperTests : TestBase
    {
        [Test]
        public void Should_map_all_properties()
        {
            var hearing = GetHearing();
            var caseNumber = "123";
            var hearingMapper = new HearingByCaseNumberResponseMapper();
            var result = hearingMapper.MapHearingToDetailedResponse(hearing, caseNumber);

            var @case = hearing.GetCases().FirstOrDefault(c => c.Number == caseNumber);
            var judgeParticipant = hearing.GetParticipants().FirstOrDefault(s => s.HearingRole?.UserRole != null && s.HearingRole.UserRole.Name == "Judge");
            var judgeName = judgeParticipant != null ? judgeParticipant.DisplayName : "";
            result.CaseName.Should().Be(@case.Name);
            result.CaseNumber.Should().Be(@case.Number);
            result.Id.Should().Be(hearing.Id);
            result.ScheduledDateTime.Should().Be(hearing.ScheduledDateTime);
            result.HearingVenueName.Should().Be(hearing.HearingVenueName);
            result.CourtroomAccount.Should().Be(judgeName);
        }

        private static VideoHearing GetHearing()
        {
            var hearing = new VideoHearingBuilder().Build();
            hearing.AddCase("123", "Case name", true);
            foreach (var participant in hearing.Participants)
            {
                participant.HearingRole = new HearingRole(1, "Name") { UserRole = new UserRole(1, "User"), };
                participant.CaseRole = new CaseRole(1, "Name");
            }
            return hearing;
        }
    }
}
