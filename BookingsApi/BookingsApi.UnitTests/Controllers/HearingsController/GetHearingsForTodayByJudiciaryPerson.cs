using System.Collections.Generic;
using System.Net;
using BookingsApi.Domain;
using BookingsApi.DAL.Queries;
using Microsoft.AspNetCore.Mvc;

namespace BookingsApi.UnitTests.Controllers.HearingsController
{
    public class GetHearingsForTodayByJudiciaryPersonTests : HearingsControllerTests
    {
        [Test]
        public async Task Should_return_hearing_details_for_today_by_judiciary_person()
        {
            var hearing = new List<VideoHearing>{ GetHearing("123")};
            QueryHandlerMock
             .Setup(x => x.Handle<GetHearingsByJudiciaryPersonQuery, List<VideoHearing>>(It.IsAny<GetHearingsByJudiciaryPersonQuery>()))
             .ReturnsAsync(hearing);

            var result = await Controller.GetHearingsForTodayByJudiciaryPerson("email@edjudiciary.net");

            result.Should().NotBeNull();
            var objectResult = (OkObjectResult)result;
            objectResult.StatusCode.Should().Be((int)HttpStatusCode.OK);
            QueryHandlerMock.Verify(x => x.Handle<GetHearingsByJudiciaryPersonQuery, List<VideoHearing>>(It.IsAny<GetHearingsByJudiciaryPersonQuery>()), Times.Once);

        }

        [Test]
        public async Task Should_return_notfound_with_no_video_hearing()
        {
            var queryParam = "email@edjudiciary.net";
            QueryHandlerMock
             .Setup(x => x.Handle<GetHearingsByJudiciaryPersonQuery, List<VideoHearing>>(It.IsAny<GetHearingsByJudiciaryPersonQuery>()))
             .ReturnsAsync(new List<VideoHearing>());

            var result = await Controller.GetHearingsForTodayByJudiciaryPerson(queryParam);

            result.Should().NotBeNull();
            var objectResult = (NotFoundObjectResult)result;
            objectResult.StatusCode.Should().Be((int)HttpStatusCode.NotFound);
            objectResult.Value.Should().Be($"No hearings found for {queryParam}");
            QueryHandlerMock.Verify(x => x.Handle<GetHearingsByJudiciaryPersonQuery, List<VideoHearing>>(It.IsAny<GetHearingsByJudiciaryPersonQuery>()), Times.Once);
        }
    }
}
