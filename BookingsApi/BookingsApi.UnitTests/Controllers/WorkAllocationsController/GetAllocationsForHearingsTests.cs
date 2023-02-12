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
        var justiceUser = new JusticeUser { UserRole = new UserRole(1,"test") };
        var hearingId1 = Guid.NewGuid();
        var hearingId2 = Guid.NewGuid();
        var expectedResponse = new List<AllocatedCsoResponse>
        {
            new (hearingId1) { Cso = JusticeUserToResponseMapper.Map(justiceUser) },
            new (hearingId2) { Cso = null }
        };
        
        QueryHandlerMock
            .SetupSequence(x =>
                x.Handle<GetCsoAllocationByHearingIdQuery, JusticeUser>(It.IsAny<GetCsoAllocationByHearingIdQuery>()))
            .ReturnsAsync(justiceUser)
            .ReturnsAsync(null as JusticeUser);

        //ACT
        var result = await Controller.GetAllocationsForHearings(new [] {hearingId1, hearingId2}) as OkObjectResult;

        //ASSERT
        QueryHandlerMock
            .Verify(x => x.Handle<GetCsoAllocationByHearingIdQuery, JusticeUser>(It.IsAny<GetCsoAllocationByHearingIdQuery>()), Times.Exactly(2));
        result.StatusCode.Should().NotBeNull().And.Be((int)HttpStatusCode.OK);
        var allocatedCsoResponse = result.Value as IEnumerable<AllocatedCsoResponse>;
        allocatedCsoResponse.Should().NotBeNull().And.HaveCount(2);
        allocatedCsoResponse.Should().BeEquivalentTo(expectedResponse);
    }
}