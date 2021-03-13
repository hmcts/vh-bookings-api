using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using BookingsApi.Contract.Responses;
using BookingsApi.DAL.Queries;
using BookingsApi.Domain;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;

namespace BookingsApi.UnitTests.Controllers.HearingsController
{
    public class GetHearingsByGroupIdTests : HearingsControllerTests
    {
        [Test]
        public async Task Should_Return_List_Of_Hearings_For_GroupId()
        {
            var groupId = Guid.NewGuid();
            var hearing1 = GetHearing("123");
            var hearing2 = GetHearing("123");
            var hearingList = new List<VideoHearing> {hearing1, hearing2};

            QueryHandlerMock
                .Setup(x =>
                    x.Handle<GetHearingsByGroupIdQuery, List<VideoHearing>>(It.IsAny<GetHearingsByGroupIdQuery>()))
                .ReturnsAsync(hearingList);

            var result = await Controller.GetHearingsByGroupId(groupId);

            result.Should().NotBeNull();
            var objectResult = (OkObjectResult) result;
            objectResult.StatusCode.Should().Be((int) HttpStatusCode.OK);
            var response = (List<HearingDetailsResponse>) objectResult.Value;
            response.Should().NotBeNull();
            response.Count.Should().Be(2);
            response.First(x => x.Id == hearing1.Id).Should().NotBeNull();
            response.First(x => x.Id == hearing2.Id).Should().NotBeNull();
            QueryHandlerMock.Verify(
                x => x.Handle<GetHearingsByGroupIdQuery, List<VideoHearing>>(It.IsAny<GetHearingsByGroupIdQuery>()),
                Times.Once);
        }

        [Test]
        public async Task Should_Return_NotFound_When_No_Hearing_Is_Returned()
        {
            var groupId = Guid.NewGuid();

            QueryHandlerMock
                .Setup(x =>
                    x.Handle<GetHearingsByGroupIdQuery, List<VideoHearing>>(It.IsAny<GetHearingsByGroupIdQuery>()))
                .ReturnsAsync((List<VideoHearing>) null);

            var result = await Controller.GetHearingsByGroupId(groupId);

            result.Should().NotBeNull();
            var objectResult = (NotFoundObjectResult) result;
            objectResult.StatusCode.Should().Be((int) HttpStatusCode.NotFound);
            QueryHandlerMock.Verify(
                x => x.Handle<GetHearingsByGroupIdQuery, List<VideoHearing>>(It.IsAny<GetHearingsByGroupIdQuery>()),
                Times.Once);
        }
    }
}