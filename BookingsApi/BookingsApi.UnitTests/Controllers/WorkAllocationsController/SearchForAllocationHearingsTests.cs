using System;
using BookingsApi.Contract.Responses;
using BookingsApi.Domain;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using BookingsApi.Contract.Requests;
using BookingsApi.DAL.Queries;
using BookingsApi.UnitTests.Controllers.WorkAllocationsController;

namespace BookingsApi.UnitTests.Controllers.HearingsController
{
    public class SearchForAllocationHearingsTests : WorkAllocationsControllerTest
    {
        [Test]
        public async Task Should_return_an_empty_list_if_no_records_found_for_the_given_parameters()
        {
            var query = new SearchForAllocationHearingsRequest()
            {
                CaseNumber = "caseNumber",
                Cso = new[]{Guid.NewGuid()},
                CaseType =  new[]{"caseType"},
                FromDate = new DateTime(),
                ToDate = new DateTime()
            };

            QueryHandlerMock
                .Setup(x => x.Handle<GetAllocationHearingsBySearchQuery, List<VideoHearing>>(It.IsAny<GetAllocationHearingsBySearchQuery>()))
                .ReturnsAsync(new List<VideoHearing>());

            var result = await Controller.SearchForAllocationHearings(query);

            result.Should().NotBeNull();

            var objectResult = (OkObjectResult)result;

            objectResult.StatusCode.Should().Be((int)HttpStatusCode.OK);

            var hearingDetailsResponse = (List<HearingDetailsResponse>)objectResult.Value;

            hearingDetailsResponse.Should().NotBeNull();

            hearingDetailsResponse.Should().BeEmpty();
            
            QueryHandlerMock
                .Verify(x => x.Handle<GetAllocationHearingsBySearchQuery, List<VideoHearing>>(It.IsAny<GetAllocationHearingsBySearchQuery>()), Times.Once);
        }

        [Test]
        public async Task Should_return_return_a_list_of_hearings()
        {
            var cNumber = "caseNumber";
            var query = new SearchForAllocationHearingsRequest()
            {
                CaseNumber = cNumber,
                Cso = new[]{Guid.NewGuid()},
                CaseType =  new[]{"caseType"},
                FromDate = new DateTime(),
                ToDate = new DateTime()
            };
            var hearingsByCaseNumber = new List<VideoHearing> { GetHearing(cNumber) };

            QueryHandlerMock
                .Setup(x => x.Handle<GetAllocationHearingsBySearchQuery, List<VideoHearing>>(It.IsAny<GetAllocationHearingsBySearchQuery>()))
                .ReturnsAsync(hearingsByCaseNumber);

            var result = await Controller.SearchForAllocationHearings(query);

            result.Should().NotBeNull();

            var objectResult = (OkObjectResult)result;

            objectResult.StatusCode.Should().Be((int)HttpStatusCode.OK);

            var hearingDetailsResponse = (List<HearingDetailsResponse>)objectResult.Value;

            hearingDetailsResponse.Should().NotBeNull();

            hearingDetailsResponse[0].Cases[0].Number.Should().Be(cNumber);

            QueryHandlerMock
                .Verify(x => x.Handle<GetAllocationHearingsBySearchQuery, List<VideoHearing>>(It.IsAny<GetAllocationHearingsBySearchQuery>()), Times.Once);
        }
    }
}
