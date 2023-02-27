using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using BookingsApi.Contract.Requests;
using BookingsApi.Contract.Responses;
using BookingsApi.DAL.Dtos;
using BookingsApi.DAL.Queries;
using BookingsApi.Domain;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;

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

            var response = (List<HearingAllocationsResponse>)objectResult.Value;

            response.Should().NotBeNull();

            response.Should().BeEmpty();
            
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
            var checkForClashesResponse = new List<HearingAllocationResultDto>()
            {
                new()
                {
                    HearingId = Guid.NewGuid(),
                    CaseType = query.CaseType[0],
                    CaseNumber = query.CaseNumber,
                    Duration = 10,
                    ScheduledDateTime = DateTime.Today.AddHours(10).AddMinutes(20),
                    AllocatedCso = "test@cso.com",
                    HasWorkHoursClash = false,
                    HasNonAvailabilityClash = false
                }
            };

            var expectedResponses = new List<HearingAllocationsResponse>()
            {
                new()
                {
                    HearingId = checkForClashesResponse[0].HearingId,
                    CaseType = checkForClashesResponse[0].CaseType,
                    CaseNumber = checkForClashesResponse[0].CaseNumber,
                    Duration = checkForClashesResponse[0].Duration,
                    ScheduledDateTime = checkForClashesResponse[0].ScheduledDateTime,
                    AllocatedCso = checkForClashesResponse[0].AllocatedCso,
                    HasWorkHoursClash = false,
                    HasNonAvailabilityClash = false
                }
            };

            QueryHandlerMock
                .Setup(x =>
                    x.Handle<GetAllocationHearingsBySearchQuery, List<VideoHearing>>(
                        It.IsAny<GetAllocationHearingsBySearchQuery>()))
                .ReturnsAsync(hearingsByCaseNumber);

            HearingAllocationServiceMock.Setup(x => x.CheckForAllocationClashes(hearingsByCaseNumber)).Returns(checkForClashesResponse);
            
            var result = await Controller.SearchForAllocationHearings(query);

            result.Should().NotBeNull();

            var objectResult = (OkObjectResult)result;

            objectResult.StatusCode.Should().Be((int)HttpStatusCode.OK);

            var response = (List<HearingAllocationsResponse>)objectResult.Value;

            response.Should().NotBeNull();

            response[0].CaseNumber.Should().Be(cNumber);

            QueryHandlerMock
                .Verify(x => x.Handle<GetAllocationHearingsBySearchQuery, List<VideoHearing>>(It.IsAny<GetAllocationHearingsBySearchQuery>()), Times.Once);

            response.Should().BeEquivalentTo(expectedResponses);
        }
    }
}
