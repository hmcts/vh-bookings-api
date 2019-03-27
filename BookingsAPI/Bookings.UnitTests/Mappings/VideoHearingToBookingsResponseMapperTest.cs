using Bookings.API.Mappings;
using Bookings.Domain;
using Bookings.Domain.RefData;
using FluentAssertions;
using NUnit.Framework;
using System.Collections.Generic;
using Testing.Common.Builders.Domain;

namespace Bookings.UnitTests.Mappings
{
    public class VideoHearingToBookingsResponseMapperTest
    {
        [Test]
        public void should_return_mapped_hearings_grouped_by_date()
        {
            var videoHearings = GetData();
            videoHearings[0].GetParticipants()[2].HearingRole = new HearingRole(5, "Judge") { UserRole = new UserRole(5, "Judge") };
            videoHearings[0].AddCase("234", "X vs Y", true);
            videoHearings[0].CaseType = new CaseType(1, "Civil Money Claims");

            var target = new VideoHearingsToBookingsResponseMapper();
            var responses = target.MapHearingResponses(videoHearings);
            responses.Should().NotBeNull();
            responses[0].Hearings.Count.Should().Be(1);
            responses[0].Hearings[0].ScheduledDuration.Should().Be(80);
            responses[0].Hearings[0].JudgeName.Should().NotBeNullOrEmpty();
            responses[0].Hearings[0].HearingNumber.Should().Be("234");
            responses[0].Hearings[0].HearingName.Should().Be("X vs Y");
            responses[0].Hearings[0].CaseTypeName.Should().Be("Civil Money Claims");
            responses[0].Hearings[0].CourtAddress.Should().Be("Birmingham Civil and Family Justice Centre");
            responses[0].Hearings[0].CourtRoom.Should().Be("Roome03");
        }

        private List<VideoHearing> GetData()
        {
            var videoHearing = new VideoHearingBuilder().Build();
            return new List<VideoHearing> { videoHearing };
        }
    }
}