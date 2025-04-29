using System.Collections.Generic;
using System.Net;
using BookingsApi.Contract.V1.Requests;
using BookingsApi.Contract.V1.Responses;
using BookingsApi.DAL.Queries;
using BookingsApi.Domain;
using Microsoft.AspNetCore.Mvc;

namespace BookingsApi.UnitTests.Controllers.WorkAllocationsController
{
    public class SearchForAllocationHearingsTests : WorkAllocationsControllerTest
    {
        [Test]
        public async Task Should_return_an_empty_list_if_no_records_found_for_the_given_parameters()
        {
            var query = new SearchForAllocationHearingsRequest()
            {
                CaseNumber = "caseNumber",
                Cso = [Guid.NewGuid()],
                CaseType = ["caseType"],
                FromDate = DateTime.MinValue,
                ToDate = DateTime.MinValue
            };

            QueryHandlerMock
                .Setup(x => x.Handle<GetAllocationHearingsBySearchQuery, List<VideoHearing>>(It.IsAny<GetAllocationHearingsBySearchQuery>()))
                .ReturnsAsync(new List<VideoHearing>());

            var result = await Controller.SearchForAllocationHearings(query);

            result.Should().NotBeNull();

            var objectResult = (OkObjectResult)result;

            objectResult.StatusCode.Should().Be((int)HttpStatusCode.OK);

            var response = (List<HearingAllocationsResponse>)objectResult.Value;

            response.Should().NotBeNull();

            response.Should().BeEmpty();
            
            QueryHandlerMock
                .Verify(x => x.Handle<GetAllocationHearingsBySearchQuery, List<VideoHearing>>(It.IsAny<GetAllocationHearingsBySearchQuery>()), Times.Once);
        }
    }
}
