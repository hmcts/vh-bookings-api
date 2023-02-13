using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using BookingsApi.Contract.Responses;
using BookingsApi.DAL.Queries;
using BookingsApi.Domain;
using BookingsApi.Domain.RefData;
using BookingsApi.Mappings;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;

namespace BookingsApi.UnitTests.Controllers.WorkAllocationsController;

public class GetAllocationsForHearingsTests : WorkAllocationsControllerTest
{
    [Test]
    public async Task Should_return_allocation_found_for_the_provided_hearings_and_null_where_unallocated()
    {
        //ARRANGE
        //first hearing
        var justiceUser = new JusticeUser { UserRole = new UserRole(1,"test") };
        var hearingId1 = Guid.NewGuid();
        var hearing1 = new Mock<VideoHearing>();
        hearing1.SetupGet(e => e.Id).Returns(hearingId1);
        hearing1.Setup(e => e.AllocatedTo).Returns(justiceUser);
        //second hearing
        var hearingId2 = Guid.NewGuid();
        var hearing2 = new Mock<VideoHearing>();
        hearing2.SetupGet(e => e.Id).Returns(hearingId2);
        var expectedResponse = new List<AllocatedCsoResponse>
        {
            new () {HearingId = hearingId1, Cso = JusticeUserToResponseMapper.Map(justiceUser) },
            new () {HearingId = hearingId2,  Cso = null }
        };

        QueryHandlerMock
            .Setup(x =>
                x.Handle<GetAllocationHearingsQuery, List<VideoHearing>>(It.IsAny<GetAllocationHearingsQuery>()))
            .ReturnsAsync(new List<VideoHearing> { hearing1.Object, hearing2.Object });

        //ACT
        var result = await Controller.GetAllocationsForHearings(new [] {hearingId1, hearingId2}) as OkObjectResult;

        //ASSERT
        result.StatusCode.Should().NotBeNull().And.Be((int)HttpStatusCode.OK);
        var allocatedCsoResponse = result.Value as IEnumerable<AllocatedCsoResponse>;
        allocatedCsoResponse.Should().NotBeNull().And.HaveCount(2);
        allocatedCsoResponse.Should().BeEquivalentTo(expectedResponse);
    }
}