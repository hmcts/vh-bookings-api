using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using BookingsApi.Contract.V1.Responses;
using BookingsApi.DAL.Queries;
using BookingsApi.Domain;
using Microsoft.AspNetCore.Mvc;

namespace BookingsApi.UnitTests.Controllers.WorkAllocationsController;

public class GetAllocationsForHearingsByVenueTests : WorkAllocationsControllerTest
{
    [Test]
    public async Task Should_return_allocation_found_for_the_provided_hearings_and_null_where_unallocated()
    {
        //ARRANGE
        var hearing = GetHearing(null);
        QueryHandlerMock
            .Setup(x =>
                x.Handle<GetHearingsForTodayQuery, List<VideoHearing>>(
                    It.IsAny<GetHearingsForTodayQuery>()))
            .ReturnsAsync(new List<VideoHearing> { hearing });

        //ACT
        var result = await Controller.GetAllocationsForHearingsByVenue(new [] {hearing.HearingVenueName}) as OkObjectResult;

        //ASSERT
        result.StatusCode.Should().NotBeNull().And.Be((int)HttpStatusCode.OK);
        var allocatedCsoResponse = result.Value as IEnumerable<AllocatedCsoResponse>;
        allocatedCsoResponse.First().HearingId.Should().Be(hearing.Id);
    }
}