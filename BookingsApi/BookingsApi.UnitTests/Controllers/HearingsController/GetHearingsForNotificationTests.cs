using System.Collections.Generic;
using System.Net;
using BookingsApi.Contract.V1.Responses;
using BookingsApi.DAL.Dtos;
using BookingsApi.DAL.Queries;
using BookingsApi.Domain;
using BookingsApi.Domain.Enumerations;
using BookingsApi.Domain.RefData;
using Microsoft.AspNetCore.Mvc;

namespace BookingsApi.UnitTests.Controllers.HearingsController
{
    public class GetHearingsForNotificationTests : HearingsControllerTests
    {
        [Test]
        public async Task Should_Return_List_Of_Hearings_For_Notifcations_SingleDay()
        {
            var caseNames = new List<string>
            {
                "Test Case"
            };

            var hearing1 = GetHearing("1231", caseNames);
            hearing1.UpdateStatus(BookingStatus.Created, "administrator", string.Empty);
            var hearing1Dto = new HearingNotificationDto(hearing1, 1);
            
            var hearing2 = GetHearing("123");

            hearing2.UpdateStatus(BookingStatus.Created, "administrator", string.Empty);
            hearing2.UpdateHearingDetails(new HearingVenue(1, "venue1"), DateTime.Now.AddDays(2),
                15, "123", "note", "administrator", new List<Case> { new Case("123", "name") }, true);

            
            
            var hearing3 = GetHearing("1232", caseNames);
            hearing3.UpdateStatus(BookingStatus.Created, "administrator", string.Empty);
            hearing2.UpdateHearingDetails(new HearingVenue(1, "venue1"), DateTime.Now.AddDays(2),
                15, "123", "note", "administrator", new List<Case> { new Case("123", "name") }, true);
            var hearing2Dto = new HearingNotificationDto(hearing2, 1);
            var hearing3Dto = new HearingNotificationDto(hearing3, 1);
            
            var hearingList = new List<HearingNotificationDto> { hearing1Dto, hearing2Dto, hearing3Dto };

            QueryHandlerMock
                .Setup(x =>
                    x.Handle<GetHearingsForNotificationsQuery, List<HearingNotificationDto>>(It.IsAny<GetHearingsForNotificationsQuery>()))
                .ReturnsAsync(hearingList);

            var result = await Controller.GetHearingsForNotificationAsync();

            result.Should().NotBeNull();

            var objectResult = (OkObjectResult)result;

            objectResult.StatusCode.Should().Be((int)HttpStatusCode.OK);
            var response = (List<HearingNotificationResponse>)objectResult.Value;

            response.Should().NotBeNull();
            response.Count.Should().Be(3);

            response.First(x => x.Hearing.Id == hearing1.Id).Should().NotBeNull();
            response.First(x => x.Hearing.Id == hearing2.Id).Should().NotBeNull();
            response.First(x => x.Hearing.Id == hearing3.Id).Should().NotBeNull();

            QueryHandlerMock.Verify(
                x => x.Handle<GetHearingsForNotificationsQuery, List<HearingNotificationDto>>(It.IsAny<GetHearingsForNotificationsQuery>()),
                Times.Once);
        }
        
        [Test]
        public async Task Should_Return_List_Of_Hearings_For_Notifcations_MultiDay()
        {
            var caseNames = new List<string>
            {
                "Test Case"
            };

            var hearing1 = GetHearing("1231", caseNames);
            hearing1.UpdateStatus(BookingStatus.Created, "administrator", string.Empty);
            var hearing1Dto = new HearingNotificationDto(hearing1, 1);
            
            var hearing2 = GetHearing("123");

            hearing2.UpdateStatus(BookingStatus.Created, "administrator", string.Empty);
            hearing2.UpdateHearingDetails(new HearingVenue(1, "venue1"), DateTime.Now.AddDays(2),
                15, "123", "note", "administrator", new List<Case> { new Case("123", "name") }, true);

            
            
            var hearing3 = GetHearing("1232", caseNames);
            hearing3.UpdateStatus(BookingStatus.Created, "administrator", string.Empty);
            hearing2.UpdateHearingDetails(new HearingVenue(1, "venue1"), DateTime.Now.AddDays(2),
                15, "123", "note", "administrator", new List<Case> { new Case("123", "name") }, true);
            var hearing2Dto = new HearingNotificationDto(hearing2, 2);
            var hearing3Dto = new HearingNotificationDto(hearing3, 3);
            
            var hearingList = new List<HearingNotificationDto> { hearing1Dto, hearing2Dto, hearing3Dto };

            QueryHandlerMock
                .Setup(x =>
                    x.Handle<GetHearingsForNotificationsQuery, List<HearingNotificationDto>>(It.IsAny<GetHearingsForNotificationsQuery>()))
                .ReturnsAsync(hearingList);

            var result = await Controller.GetHearingsForNotificationAsync();

            result.Should().NotBeNull();

            var objectResult = (OkObjectResult)result;

            objectResult.StatusCode.Should().Be((int)HttpStatusCode.OK);
            var response = (List<HearingNotificationResponse>)objectResult.Value;

            response.Should().NotBeNull();
            response.Count.Should().Be(3);

            response.First(x => x.Hearing.Id == hearing1.Id).Should().NotBeNull();
            response.First(x => x.Hearing.Id == hearing2.Id).Should().NotBeNull();
            response.First(x => x.Hearing.Id == hearing3.Id).Should().NotBeNull();

            QueryHandlerMock.Verify(
                x => x.Handle<GetHearingsForNotificationsQuery, List<HearingNotificationDto>>(It.IsAny<GetHearingsForNotificationsQuery>()),
                Times.Once);
        }

        private static VideoHearing GetHearing(string caseNumber, List<string> caseNames)
        {
            var hearing = new VideoHearingBuilder().Build();

            if (!string.IsNullOrEmpty(caseNumber))
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

