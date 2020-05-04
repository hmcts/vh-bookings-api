using Bookings.API.Mappings;
using Bookings.Domain;
using Bookings.Domain.RefData;
using Bookings.UnitTests.Utilities;
using FluentAssertions;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using Testing.Common.Builders.Domain;

namespace Bookings.UnitTests.Mappings
{
    public class HearingByCaseNumberResponseMapperTests : TestBase
    {
        [Test]
        public void Should_map_all_properties()
        {
            var caseNumber = "123";
            var hearingsByCaseNumber = new List<VideoHearing>() { GetHearing() };
            var hearingMapper = new HearingByCaseNumberResponseMapper();
            var result = hearingMapper.MapHearingToDetailedResponse(hearingsByCaseNumber, caseNumber);

            var @case = hearingsByCaseNumber[0].GetCases().FirstOrDefault(c => c.Number == caseNumber);
            var judgeParticipant = hearingsByCaseNumber[0].GetParticipants().FirstOrDefault(s => s.HearingRole?.UserRole != null && s.HearingRole.UserRole.Name == "Judge");
            var courtroomAccountName = judgeParticipant != null ? judgeParticipant.DisplayName : string.Empty;
            var courtroomAccount = (judgeParticipant != null && judgeParticipant.Person != null) ? judgeParticipant.Person.Username : string.Empty;
            result[0].CaseName.Should().Be(@case.Name);
            result[0].CaseNumber.Should().Be(@case.Number);
            result[0].Id.Should().Be(hearingsByCaseNumber[0].Id);
            result[0].ScheduledDateTime.Should().Be(hearingsByCaseNumber[0].ScheduledDateTime);
            result[0].HearingVenueName.Should().Be(hearingsByCaseNumber[0].HearingVenueName);
            result[0].CourtroomAccount.Should().Be(courtroomAccount);
            result[0].CourtroomAccountName.Should().Be(courtroomAccountName);
            result[0].HearingRoomName.Should().Be(hearingsByCaseNumber[0].HearingRoomName);
        }

        private static VideoHearing GetHearing()
        {
            var hearing = new VideoHearingBuilder().Build();
            hearing.AddCase("123", "Case name", true);
            foreach (var participant in hearing.Participants)
            {
                participant.HearingRole = new HearingRole(1, "Name") { UserRole = new UserRole(1, "Judge"), };
                participant.CaseRole = new CaseRole(1, "Name");
            }
            return hearing;
        }
    }
}
