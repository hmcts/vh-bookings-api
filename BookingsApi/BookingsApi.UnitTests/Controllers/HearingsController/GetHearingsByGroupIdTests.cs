using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using BookingsApi.Contract.V1.Responses;
using BookingsApi.DAL.Queries;
using BookingsApi.Domain;
using BookingsApi.Domain.RefData;
using Castle.Core.Internal;
using Microsoft.AspNetCore.Mvc;

namespace BookingsApi.UnitTests.Controllers.HearingsController
{
    public class GetHearingsByGroupIdTests : HearingsControllerTests
    {
        [Test]
        public async Task Should_Return_List_Of_Hearings_For_GroupId()
        {
            var groupId = Guid.NewGuid();
            var caseNames = new List<string>
            {
                "Case name Day 1 of 2",
                "Case name Day 2 of 2"
            };

            var hearing1 = GetMultiDayHearing("123", caseNames);
            var hearing2 = GetMultiDayHearing("123", caseNames);
            var hearingList = new List<VideoHearing> { hearing1, hearing2 };

            QueryHandlerMock
                .Setup(x =>
                    x.Handle<GetHearingsByGroupIdQuery, List<VideoHearing>>(It.IsAny<GetHearingsByGroupIdQuery>()))
                .ReturnsAsync(hearingList);

            var result = await Controller.GetHearingsByGroupId(groupId);

            result.Should().NotBeNull();
            var objectResult = (OkObjectResult)result;
            objectResult.StatusCode.Should().Be((int)HttpStatusCode.OK);
            var response = (List<HearingDetailsResponse>)objectResult.Value;
            response.Should().NotBeNull();
            response.Count.Should().Be(2);
            response.First(x => x.Id == hearing1.Id).Should().NotBeNull();
            response.First(x => x.Id == hearing2.Id).Should().NotBeNull();
            QueryHandlerMock.Verify(
                x => x.Handle<GetHearingsByGroupIdQuery, List<VideoHearing>>(It.IsAny<GetHearingsByGroupIdQuery>()),
                Times.Once);
        }

        [Test]
        public async Task Should_Return_An_Empty_List_When_No_Hearing_Is_Returned()
        {
            var groupId = Guid.NewGuid();

            QueryHandlerMock
                .Setup(x =>
                    x.Handle<GetHearingsByGroupIdQuery, List<VideoHearing>>(It.IsAny<GetHearingsByGroupIdQuery>()))
                .ReturnsAsync(new List<VideoHearing>());

            var result = await Controller.GetHearingsByGroupId(groupId);

            result.Should().NotBeNull();
            var objectResult = (OkObjectResult)result;
            objectResult.StatusCode.Should().Be((int)HttpStatusCode.OK);

            var value = (List<HearingDetailsResponse>)objectResult.Value;
            value.Count.Should().Be(0);
            QueryHandlerMock.Verify(
                x => x.Handle<GetHearingsByGroupIdQuery, List<VideoHearing>>(It.IsAny<GetHearingsByGroupIdQuery>()),
                Times.Once);
        }

        private static VideoHearing GetMultiDayHearing(string caseNumber, List<string> caseNames)
        {
            var hearing = new VideoHearingBuilder().Build();

            if (!caseNumber.IsNullOrEmpty())
            {
                foreach (var caseName in caseNames)
                {
                    hearing.AddCase(caseNumber, caseName, true);
                }
            }

            foreach (var participant in hearing.Participants)
            {
                participant.HearingRole = new HearingRole(1, "Name") { UserRole = new UserRole(1, "User"), };
                participant.CaseRole = new CaseRole(1, "Name");
            }

            hearing.AddEndpoints(new List<Endpoint>
                {new Endpoint("new endpoint", Guid.NewGuid().ToString(), "pin", null)});

            return hearing;
        }
    }
}