using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using BookingsApi.Domain;
using BookingsApi.DAL.Queries;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;

namespace BookingsApi.UnitTests.Controllers.HearingsController
{
    public class GetHearingsForTodayTests : HearingsControllerTests
    {
        [Test]
        public async Task Should_return_hearing_details_for_given_venue_names()
        {
            var hearing = new List<VideoHearing>{ GetHearing("123")};

            QueryHandlerMock
             .Setup(x => x.Handle<GetHearingsForTodayQuery, List<VideoHearing>>(It.IsAny<GetHearingsForTodayQuery>()))
             .ReturnsAsync(hearing);

            var result = await Controller.GetHearingsForTodayByVenue(hearing.Select(h => h.HearingVenueName).ToArray());

            result.Should().NotBeNull();
            var objectResult = (OkObjectResult)result;
            objectResult.StatusCode.Should().Be((int)HttpStatusCode.OK);
            QueryHandlerMock.Verify(x => x.Handle<GetHearingsForTodayQuery, List<VideoHearing>>(It.IsAny<GetHearingsForTodayQuery>()), Times.Once);

        }

        [Test]
        public async Task Should_return_notfound_with_no_video_hearing()
        {
            QueryHandlerMock
             .Setup(x => x.Handle<GetHearingsForTodayQuery, List<VideoHearing>>(It.IsAny<GetHearingsForTodayQuery>()))
             .ReturnsAsync(new List<VideoHearing>());

            var result = await Controller.GetHearingsForTodayByVenue(new[]{"random venue name"});

            result.Should().NotBeNull();
            var objectResult = (NotFoundResult)result;
            objectResult.StatusCode.Should().Be((int)HttpStatusCode.NotFound);
            QueryHandlerMock.Verify(x => x.Handle<GetHearingsForTodayQuery, List<VideoHearing>>(It.IsAny<GetHearingsForTodayQuery>()), Times.Once);
        }
    }
}
