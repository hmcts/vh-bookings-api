using System.Collections.Generic;
using System.Net;
using BookingsApi.Contract.V1.Responses;
using BookingsApi.DAL.Queries;
using BookingsApi.Domain;
using BookingsApi.Domain.Enumerations;
using BookingsApi.Domain.RefData;
using Castle.Core.Internal;
using Microsoft.AspNetCore.Mvc;

namespace BookingsApi.UnitTests.Controllers.WorkAllocationsController
{
    public class GetUnallocatedHearingsTests : WorkAllocationsControllerTest
    {

        [Test]
        public async Task Should_Return_List_Of_Unallocated_Hearings()
        {
            var caseNames = new List<string>
            {
                "Test Case"
            };

            var hearing1 = GetHearing("1231", caseNames);
            hearing1.UpdateStatus(BookingStatus.Created, "administrator", string.Empty);

            var hearing2 = GetHearing("123");

            hearing2.UpdateStatus(BookingStatus.Created, "administrator", string.Empty);
            hearing2.UpdateHearingDetails(new HearingVenue(1, "venue1"), DateTime.Now.AddDays(2),
                15, "123", "note", "administrator", new List<Case> { new Case("123", "name") }, true);
            
            var hearingList = new List<VideoHearing> { hearing1, hearing2 };

            QueryHandlerMock
                .Setup(x => x.Handle<GetAllocationHearingsBySearchQuery, List<VideoHearing>>(It.IsAny<GetAllocationHearingsBySearchQuery>()))
                .ReturnsAsync(hearingList);

             var result = await Controller.GetUnallocatedHearings();

            result.Should().NotBeNull();

            var objectResult = (OkObjectResult)result;

            objectResult.StatusCode.Should().Be((int)HttpStatusCode.OK);
            var response = (List<HearingDetailsResponse>)objectResult.Value;

            response.Should().NotBeNull();
            response.Count.Should().Be(2);

            response.First(x => x.Id == hearing1.Id).Should().NotBeNull();
            response.First(x => x.Id == hearing2.Id).Should().NotBeNull();

            QueryHandlerMock
                .Verify(x => x.Handle<GetAllocationHearingsBySearchQuery, List<VideoHearing>>(It.IsAny<GetAllocationHearingsBySearchQuery>()), Times.Once);
        }
        
        [Test]
        public async Task Should_Return_empty_List_Of_Unallocated_Hearings_if_Not_Found()
        {
            QueryHandlerMock
                .Setup(x => x.Handle<GetAllocationHearingsBySearchQuery, List<VideoHearing>>(It.IsAny<GetAllocationHearingsBySearchQuery>()))
                .ReturnsAsync(new List<VideoHearing>());
            
            var result = await Controller.GetUnallocatedHearings();

            result.Should().NotBeNull();

            var objectResult = (OkObjectResult)result;

            objectResult.StatusCode.Should().Be((int)HttpStatusCode.OK);
            var response = (List<HearingDetailsResponse>)objectResult.Value;

            response.Should().NotBeNull();
            response.Count.Should().Be(0);

            QueryHandlerMock
                .Verify(x => x.Handle<GetAllocationHearingsBySearchQuery, List<VideoHearing>>(It.IsAny<GetAllocationHearingsBySearchQuery>()), Times.Once);
        }

        private static VideoHearing GetHearing(string caseNumber, List<string> caseNames)
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

        private new static VideoHearing GetHearing(string caseNumber)
        {
            var hearing = new VideoHearingBuilder().Build();

            if (!caseNumber.IsNullOrEmpty())
            {
                hearing.AddCase(caseNumber, "Case name", true);
            }

            hearing.AddEndpoints(new List<Endpoint>
                { new Endpoint("new endpoint", Guid.NewGuid().ToString(), "pin", null) });

            return hearing;
        }
    }
}
