using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using BookingsApi.Domain;
using BookingsApi.Domain.Enumerations;
using BookingsApi.Domain.RefData;
using BookingsApi.Common;
using BookingsApi.Common.Configuration;
using BookingsApi.Common.Services;
using BookingsApi.Contract.V1.Requests;
using BookingsApi.Contract.V1.Requests.Enums;
using BookingsApi.Contract.V1.Responses;
using BookingsApi.DAL.Commands;
using BookingsApi.DAL.Commands.Core;
using BookingsApi.DAL.Queries;
using BookingsApi.DAL.Queries.Core;
using BookingsApi.Infrastructure.Services.IntegrationEvents;
using BookingsApi.Infrastructure.Services.IntegrationEvents.Events;
using BookingsApi.Infrastructure.Services.ServiceBusQueue;
using Castle.Core.Internal;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Moq;
using NUnit.Framework;
using Testing.Common.Assertions;
using Testing.Common.Builders.Domain;
using BookingsApi.DAL.Services;
using BookingsApi.Services;
using FluentAssertions.Common;

namespace BookingsApi.UnitTests.Controllers.HearingsController
{
    public class HearingsControllerTests
    {
        protected BookingsApi.Controllers.V1.HearingsController Controller;
        protected Mock<IQueryHandler> QueryHandlerMock;
        protected Mock<ICommandHandler> CommandHandlerMock;
        protected Mock<IRandomGenerator> RandomGenerator;
        protected Mock<IHearingService> HearingServiceMock;
        protected KinlyConfiguration KinlyConfiguration;
        protected Mock<IFeatureToggles> FeatureTogglesMock;
        protected Mock<IVhLogger> Logger;

        private IEventPublisher _eventPublisher;
        protected Mock<IEventPublisher> EventPublisherMock;
        protected ServiceBusQueueClientFake SbQueueClient;

        [SetUp]
        public void Setup()
        {
            SbQueueClient = new ServiceBusQueueClientFake();
            QueryHandlerMock = new Mock<IQueryHandler>();
            CommandHandlerMock = new Mock<ICommandHandler>();
            HearingServiceMock = new Mock<IHearingService>();
            KinlyConfiguration = new KinlyConfiguration { SipAddressStem = "@WhereAreYou.com" };
            RandomGenerator = new Mock<IRandomGenerator>();
            FeatureTogglesMock = new Mock<IFeatureToggles>();
            _eventPublisher = new EventPublisher(SbQueueClient);
            EventPublisherMock = new Mock<IEventPublisher>();
            Logger = new Mock<IVhLogger>();

            FeatureTogglesMock.Setup(r => r.AdminSearchToggle()).Returns(false);
            Controller = GetControllerObject(false);
        }

        protected BookingsApi.Controllers.V1.HearingsController GetControllerObject(bool withQueueClient)
        {
            var eventPublisher = withQueueClient ? _eventPublisher : EventPublisherMock.Object;
            var bookingService = new BookingService(eventPublisher, CommandHandlerMock.Object, QueryHandlerMock.Object);
            return new BookingsApi.Controllers.V1.HearingsController(QueryHandlerMock.Object, CommandHandlerMock.Object,
                bookingService, RandomGenerator.Object, new OptionsWrapper<KinlyConfiguration>(KinlyConfiguration),
                HearingServiceMock.Object, FeatureTogglesMock.Object, Logger.Object);
        }

        [Test]
        public async Task Should_use_utc_now_at_midnight_when_no_from_date_is_provided()
        {
            // Arrange
            var expectedDate = DateTime.UtcNow.Date;

            QueryHandlerMock
                .Setup(x =>
                    x.Handle<GetBookingsByCaseTypesQuery, CursorPagedResult<VideoHearing, string>>(
                        It.IsAny<GetBookingsByCaseTypesQuery>()))
                .ReturnsAsync(new CursorPagedResult<VideoHearing, string>(new List<VideoHearing>(), "next cursor"));

            // Act
            await Controller.GetHearingsByTypes(new GetHearingRequest { Types = new List<int>()});

            // Assert
            QueryHandlerMock.Verify(
                x => x.Handle<GetBookingsByCaseTypesQuery, CursorPagedResult<VideoHearing, string>>(
                    It.Is<GetBookingsByCaseTypesQuery>(x => x.StartDate == expectedDate)), Times.Once);
        }

        [Test]
        public async Task Should_use_passed_from_date_when_from_date_is_provided()
        {
            // Arrange
            var expectedDate = DateTime.UtcNow.Date.AddDays(3);

            QueryHandlerMock
                .Setup(x =>
                    x.Handle<GetBookingsByCaseTypesQuery, CursorPagedResult<VideoHearing, string>>(
                        It.IsAny<GetBookingsByCaseTypesQuery>()))
                .ReturnsAsync(new CursorPagedResult<VideoHearing, string>(new List<VideoHearing>(), "next cursor"));

            // Act
            await Controller.GetHearingsByTypes(
                new GetHearingRequest 
                { 
                    Types = new List<int>(),
                    Cursor = GetHearingRequest.DefaultCursor,
                    Limit = 100,
                    FromDate = expectedDate
                });

            // Assert
            QueryHandlerMock.Verify(
                x => x.Handle<GetBookingsByCaseTypesQuery, CursorPagedResult<VideoHearing, string>>(
                    It.Is<GetBookingsByCaseTypesQuery>(x => x.StartDate == expectedDate)), Times.Once);
        }

        [Test]
        public async Task Should_return_bad_request_if_invalid_case_types()
        {
            var caseTypes = new List<int> { 44, 78 };
            QueryHandlerMock
                .Setup(x => x.Handle<GetAllCaseTypesQuery, List<CaseType>>(It.IsAny<GetAllCaseTypesQuery>()))
                .ReturnsAsync(new List<CaseType> { new CaseType(44, "Financial"), new CaseType(2, "Civil") });

            var result = await Controller.GetHearingsByTypes(
                new GetHearingRequest
                {
                    Types = caseTypes,
                    Cursor = GetHearingRequest.DefaultCursor,
                    Limit = 2
                });

            result.Should().NotBeNull();
            result.Should().NotBeNull();
            var objectResult = (ObjectResult)result.Result;
            objectResult.StatusCode.Should().Be((int)HttpStatusCode.BadRequest);
            ((SerializableError)objectResult.Value).ContainsKeyAndErrorMessage("Hearing types",
                "Invalid value for hearing types");
        }

        [Test]
        public async Task Should_return_bookings()
        {
            var caseTypes = new List<int>();
            QueryHandlerMock
                .Setup(x => x.Handle<GetAllCaseTypesQuery, List<CaseType>>(It.IsAny<GetAllCaseTypesQuery>()))
                .ReturnsAsync(new List<CaseType>());

            QueryHandlerMock
                .Setup(x =>
                    x.Handle<GetBookingsByCaseTypesQuery, CursorPagedResult<VideoHearing, string>>(
                        It.IsAny<GetBookingsByCaseTypesQuery>()))
                .ReturnsAsync(new CursorPagedResult<VideoHearing, string>(new List<VideoHearing>(), "next cursor"));

            var result = await Controller.GetHearingsByTypes(
               new GetHearingRequest
               {
                   Types = caseTypes,
                   Cursor = GetHearingRequest.DefaultCursor,
                   Limit = 2
               });

            result.Should().NotBeNull();
            var objectResult = (ObjectResult)result.Result;
            objectResult.StatusCode.Should().Be((int)HttpStatusCode.OK);
            var response = (BookingsResponse)((ObjectResult)result.Result).Value;
            response.PrevPageUrl.Should().Be("hearings/types?types=&cursor=0&limit=2");
            response.NextPageUrl.Should().Be("hearings/types?types=&cursor=next cursor&limit=2");
            QueryHandlerMock.Verify(q => q.Handle<GetBookingsByCaseTypesQuery, CursorPagedResult<VideoHearing, string>>
                (It.Is<GetBookingsByCaseTypesQuery>(g => g.Cursor == null)), Times.Once);
        }

        [Test]
        public async Task Should_return_next_and_previous_page_urls()
        {
            var caseTypes = new List<int> { 1, 2 };
            QueryHandlerMock
                .Setup(x => x.Handle<GetAllCaseTypesQuery, List<CaseType>>(It.IsAny<GetAllCaseTypesQuery>()))
                .ReturnsAsync(new List<CaseType> { new CaseType(1, "Financial"), new CaseType(2, "Civil") });

            QueryHandlerMock
                .Setup(x =>
                    x.Handle<GetBookingsByCaseTypesQuery, CursorPagedResult<VideoHearing, string>>(
                        It.IsAny<GetBookingsByCaseTypesQuery>()))
                .ReturnsAsync(new CursorPagedResult<VideoHearing, string>(new List<VideoHearing>(), "next-cursor"));

            var result = await Controller.GetHearingsByTypes(
               new GetHearingRequest
               {
                   Types = caseTypes,
                   Cursor = GetHearingRequest.DefaultCursor,
                   Limit = 2
               });

            result.Should().NotBeNull();
            var response = (BookingsResponse)((ObjectResult)result.Result).Value;
            response.PrevPageUrl.Should().Be("hearings/types?types=1&types=2&cursor=0&limit=2");
            response.NextPageUrl.Should().Be("hearings/types?types=1&types=2&cursor=next-cursor&limit=2");
        }

        [Test]
        public async Task Should_change_hearing_status_to_cancelled()
        {
            var controller = GetControllerObject(true);
            var request = new UpdateBookingStatusRequest
            {
                UpdatedBy = "email@hmcts.net",
                Status = UpdateBookingStatus.Cancelled,
                CancelReason = "Adjournment"
            };
            var hearing = GetHearing("123");
            var hearingId = hearing.Id;
            hearing.UpdateStatus(BookingStatus.Created, "autoTest", null);

            QueryHandlerMock
                .Setup(x => x.Handle<GetHearingByIdQuery, VideoHearing>(It.IsAny<GetHearingByIdQuery>()))
                .ReturnsAsync(hearing);

            var result = await controller.UpdateBookingStatus(hearingId, request);

            result.Should().NotBeNull();
            var objectResult = (NoContentResult)result;
            objectResult.StatusCode.Should().Be((int)HttpStatusCode.NoContent);

            var message = SbQueueClient.ReadMessageFromQueue();
            var typedMessage = (HearingCancelledIntegrationEvent)message.IntegrationEvent;
            typedMessage.Should().NotBeNull();
            typedMessage.HearingId.Should().Be(hearingId);
        }

        [Test]
        public async Task Should_not_update_booking_and_return_badrequest_with_an_invalid_hearingid()
        {
            var request = new UpdateBookingStatusRequest();
            var hearingId = Guid.Empty;

            var result = await Controller.UpdateBookingStatus(hearingId, request);

            result.Should().NotBeNull();
            var objectResult = (BadRequestObjectResult)result;
            objectResult.StatusCode.Should().Be((int)HttpStatusCode.BadRequest);
            ((SerializableError)objectResult.Value).ContainsKeyAndErrorMessage(nameof(hearingId),
                $"Please provide a valid {nameof(hearingId)}");
        }

        [Test]
        public async Task Should_remove_hearing_with_status_created_and_send_event_to_video()
        {
            var hearing = GetHearing("123");
            var hearingId = hearing.Id;
            hearing.UpdateStatus(BookingStatus.Created, "administrator", string.Empty);

            QueryHandlerMock
                .Setup(x => x.Handle<GetHearingByIdQuery, VideoHearing>(It.IsAny<GetHearingByIdQuery>()))
                .ReturnsAsync(hearing);
            var controller = GetControllerObject(true);
            var result = await controller.RemoveHearing(hearingId);

            result.Should().NotBeNull();
            var objectResult = (NoContentResult)result;
            objectResult.StatusCode.Should().Be((int)HttpStatusCode.NoContent);

            var message = SbQueueClient.ReadMessageFromQueue();
            var typedMessage = (HearingCancelledIntegrationEvent)message.IntegrationEvent;
            typedMessage.Should().NotBeNull();
            typedMessage.HearingId.Should().Be(hearingId);
        }

        [Test]
        public async Task Should_not_remove_hearing_and_return_badrequest_with_an_invalid_hearingid()
        {
            var hearingId = Guid.Empty;

            var result = await Controller.RemoveHearing(hearingId);

            result.Should().NotBeNull();
            var objectResult = (BadRequestObjectResult)result;
            objectResult.StatusCode.Should().Be((int)HttpStatusCode.BadRequest);
            ((SerializableError)objectResult.Value).ContainsKeyAndErrorMessage(nameof(hearingId),
                $"Please provide a valid {nameof(hearingId)}");
        }

        [Test]
        public async Task Should_not_remove_hearing_and_return_badrequest_without_VideoHearing()
        {
            var hearingId = Guid.NewGuid();
            var hearing = GetHearing("123");
            hearing.UpdateStatus(BookingStatus.Created, "administrator", string.Empty);

            QueryHandlerMock
                .Setup(x => x.Handle<GetHearingByIdQuery, VideoHearing>(It.IsAny<GetHearingByIdQuery>()))
                .ReturnsAsync((VideoHearing)null);

            var result = await Controller.RemoveHearing(hearingId);

            result.Should().NotBeNull();
            var objectResult = (NotFoundObjectResult)result;
            objectResult.StatusCode.Should().Be((int)HttpStatusCode.NotFound);
            objectResult.Value.Should().Be($"{hearingId} does not exist");
        }

        [Test]
        public async Task Should_change_hearing_status_to_failed()
        {
            var request = new UpdateBookingStatusRequest
            {
                UpdatedBy = "email@hmcts.net",
                Status = UpdateBookingStatus.Failed,
                CancelReason = ""
            };
            
            var hearing = GetHearing("123");
            QueryHandlerMock
               .Setup(x => x.Handle<GetHearingByIdQuery, VideoHearing>(It.IsAny<GetHearingByIdQuery>()))
               .ReturnsAsync(hearing);

            CommandHandlerMock.Setup(x => x.Handle(It.IsAny<UpdateHearingStatusCommand>()));

            var result = await Controller.UpdateBookingStatus(hearing.Id, request);

            result.Should().NotBeNull();
            var objectResult = (NoContentResult)result;
            objectResult.StatusCode.Should().Be((int)HttpStatusCode.NoContent);

            CommandHandlerMock.Verify(c => c.Handle(It.IsAny<UpdateHearingStatusCommand>()), Times.Once);
        }

        [Test]
        public async Task AnonymiseParticipantAndCaseByHearingId_Returns_Ok()
        {
            var hearingIds = new List<Guid> { Guid.NewGuid(), Guid.NewGuid() };

            var response = await Controller.AnonymiseParticipantAndCaseByHearingId(hearingIds) as OkResult;

            response.StatusCode.Should().Be((int)HttpStatusCode.OK);
            CommandHandlerMock
                .Verify(c =>
                        c.Handle(It.Is<AnonymiseCaseAndParticipantCommand>(prop =>
                            prop.HearingIds.Equals(hearingIds))),
                    Times.Once);
        }

        [Test(Description = "With AdminSearchToggle On")]
        public async Task Should_return_bookings_list_for_case_number_search()
        {
            var caseTypes = new List<int> { 1, 2 };

            const string searchTerm = "CASE_NUMBER";

            FeatureTogglesMock.Setup(r => r.AdminSearchToggle()).Returns(true);

            QueryHandlerMock
                .Setup(x => x.Handle<GetAllCaseTypesQuery, List<CaseType>>(It.IsAny<GetAllCaseTypesQuery>()))
                .ReturnsAsync(new List<CaseType> { new CaseType(1, "Financial"), new CaseType(2, "Civil") });

            QueryHandlerMock
                .Setup(x =>
                    x.Handle<GetBookingsByCaseTypesQuery, CursorPagedResult<VideoHearing, string>>(
                        It.IsAny<GetBookingsByCaseTypesQuery>()))
                .ReturnsAsync(new CursorPagedResult<VideoHearing, string>(new List<VideoHearing>(), "next-cursor"));

            var objectResult  = (await Controller.GetHearingsByTypes(
               new GetHearingRequest
               {
                   Types = caseTypes,
                   Cursor = GetHearingRequest.DefaultCursor,
                   Limit = 2,
                   CaseNumber = searchTerm
               }))
               .Result as ObjectResult;

            var response = (BookingsResponse)objectResult.Value;

            objectResult.Should().NotBeNull();
            objectResult.StatusCode.Should().Be((int)HttpStatusCode.OK);
            response.Limit.Should().Be(2);
            response.NextCursor.Should().Be("next-cursor");
            response.PrevPageUrl.Should().Be($"hearings/types?types=1&types=2&cursor=0&limit=2&caseNumber={searchTerm}&venueIds=");
            response.NextPageUrl.Should().Be($"hearings/types?types=1&types=2&cursor=next-cursor&limit=2&caseNumber={searchTerm}&venueIds=");

            QueryHandlerMock.Verify(
                x => x.Handle<GetBookingsByCaseTypesQuery, CursorPagedResult<VideoHearing, string>>(
                    It.IsAny<GetBookingsByCaseTypesQuery>()), Times.Once);
        }

        [Test(Description = "With AdminSearchToggle On")]
        public async Task Should_return_bookings_list_for_venue_ids_search()
        {
            var caseTypes = new List<int>();
            var venueIds = new List<int> { 1, 2, 3 };
            
            FeatureTogglesMock.Setup(r => r.AdminSearchToggle()).Returns(true);
            
            QueryHandlerMock
                .Setup(x => x.Handle<GetHearingVenuesQuery, List<HearingVenue>>(It.IsAny<GetHearingVenuesQuery>()))
                .ReturnsAsync(new List<HearingVenue> { new HearingVenue(1, "Birmingham"), new HearingVenue(2, "Manchester"), new HearingVenue(3, "London") });
            
            QueryHandlerMock
                .Setup(x =>
                    x.Handle<GetBookingsByCaseTypesQuery, CursorPagedResult<VideoHearing, string>>(
                        It.IsAny<GetBookingsByCaseTypesQuery>()))
                .ReturnsAsync(new CursorPagedResult<VideoHearing, string>(new List<VideoHearing>(), "next-cursor"));
            
            var objectResult = (await Controller.GetHearingsByTypes(
              new GetHearingRequest
              {
                  Types = caseTypes,
                  Cursor = GetHearingRequest.DefaultCursor,
                  Limit = 2,
                  VenueIds = venueIds
              }))
              .Result as ObjectResult;

            var response = (BookingsResponse)objectResult.Value;

            objectResult.Should().NotBeNull();
            objectResult.StatusCode.Should().Be((int)HttpStatusCode.OK);
            response.Limit.Should().Be(2);
            response.NextCursor.Should().Be("next-cursor");
            response.PrevPageUrl.Should().Be($"hearings/types?types=&cursor=0&limit=2&venueIds=1&venueIds=2&venueIds=3");
            response.NextPageUrl.Should().Be($"hearings/types?types=&cursor=next-cursor&limit=2&venueIds=1&venueIds=2&venueIds=3");

            QueryHandlerMock.Verify(
                x => x.Handle<GetBookingsByCaseTypesQuery, CursorPagedResult<VideoHearing, string>>(
                    It.IsAny<GetBookingsByCaseTypesQuery>()), Times.Once);
        }
        
        [Test]
        public async Task Should_return_bad_request_if_invalid_venue_ids()
        {
            var venueIds = new List<int> { 7, 50 };
            QueryHandlerMock
                .Setup(x => x.Handle<GetHearingVenuesQuery, List<HearingVenue>>(It.IsAny<GetHearingVenuesQuery>()))
                .ReturnsAsync(new List<HearingVenue> { new HearingVenue(7, "Tribunal"), new HearingVenue(33, "Private Law") });
        
            var result = await Controller.GetHearingsByTypes(
             new GetHearingRequest
             {
                 Types = null,
                 Cursor = GetHearingRequest.DefaultCursor,
                 Limit = 2,
                 VenueIds = venueIds
             });
             

            result.Should().NotBeNull();
            result.Should().NotBeNull();
            var objectResult = (ObjectResult)result.Result;
            objectResult.StatusCode.Should().Be((int)HttpStatusCode.BadRequest);
            ((SerializableError)objectResult.Value).ContainsKeyAndErrorMessage("Venue ids",
                "Invalid value for venue ids");
        }

        [Test]
        public async Task Should_return_bookings_list_for_participant_last_name_search()
        {
            var caseTypes = new List<int>();
            
            var lastName = "PARTICIPANT_LAST_NAME";

            FeatureTogglesMock.Setup(r => r.AdminSearchToggle()).Returns(true);

            QueryHandlerMock
                .Setup(x => x.Handle<GetHearingVenuesQuery, List<HearingVenue>>(It.IsAny<GetHearingVenuesQuery>()))
                .ReturnsAsync(new List<HearingVenue> { new HearingVenue(1, "Birmingham"), new HearingVenue(2, "Manchester"), new HearingVenue(3, "London") });

            QueryHandlerMock
                .Setup(x =>
                    x.Handle<GetBookingsByCaseTypesQuery, CursorPagedResult<VideoHearing, string>>(
                        It.IsAny<GetBookingsByCaseTypesQuery>()))
                .ReturnsAsync(new CursorPagedResult<VideoHearing, string>(new List<VideoHearing>(), "next-cursor"));

            var objectResult = (await Controller.GetHearingsByTypes(
              new GetHearingRequest
              {
                  Types = caseTypes,
                  Cursor = GetHearingRequest.DefaultCursor,
                  Limit = 2,
                  LastName = lastName
              }))
              .Result as ObjectResult;

            var response = (BookingsResponse)objectResult.Value;

            objectResult.Should().NotBeNull();
            objectResult.StatusCode.Should().Be((int)HttpStatusCode.OK);
            response.Limit.Should().Be(2);
            response.NextCursor.Should().Be("next-cursor");
            response.PrevPageUrl.Should().Be($"hearings/types?types=&cursor=0&limit=2&venueIds=&lastName={lastName}");
            response.NextPageUrl.Should().Be($"hearings/types?types=&cursor=next-cursor&limit=2&venueIds=&lastName={lastName}");

            QueryHandlerMock.Verify(
                x => x.Handle<GetBookingsByCaseTypesQuery, CursorPagedResult<VideoHearing, string>>(
                    It.IsAny<GetBookingsByCaseTypesQuery>()), Times.Once);
        }

        [Test(Description = "With AdminSearchToggle On")]
        public async Task Should_return_bookings_list_without_judge_search()
        {
            var caseTypes = new List<int> { 1, 2 };

            FeatureTogglesMock.Setup(r => r.AdminSearchToggle()).Returns(true);

            QueryHandlerMock
                .Setup(x => x.Handle<GetAllCaseTypesQuery, List<CaseType>>(It.IsAny<GetAllCaseTypesQuery>()))
                .ReturnsAsync(new List<CaseType> { new CaseType(1, "Financial"), new CaseType(2, "Civil") });

            QueryHandlerMock
                .Setup(x =>
                    x.Handle<GetBookingsByCaseTypesQuery, CursorPagedResult<VideoHearing, string>>(
                        It.IsAny<GetBookingsByCaseTypesQuery>()))
                .ReturnsAsync(new CursorPagedResult<VideoHearing, string>(new List<VideoHearing>(), "next-cursor"));

            var objectResult = (await Controller.GetHearingsByTypes(
               new GetHearingRequest
               {
                   Types = caseTypes,
                   Cursor = GetHearingRequest.DefaultCursor,
                   Limit = 2,
                   NoJudge = true
               }))
               .Result as ObjectResult;

            var response = (BookingsResponse)objectResult.Value;

            objectResult.Should().NotBeNull();
            objectResult.StatusCode.Should().Be((int)HttpStatusCode.OK);
            response.Limit.Should().Be(2);
            response.NextCursor.Should().Be("next-cursor");
            response.PrevPageUrl.Should().Be($"hearings/types?types=1&types=2&cursor=0&limit=2&venueIds=");
            response.NextPageUrl.Should().Be($"hearings/types?types=1&types=2&cursor=next-cursor&limit=2&venueIds=");

            QueryHandlerMock.Verify(
                x => x.Handle<GetBookingsByCaseTypesQuery, CursorPagedResult<VideoHearing, string>>(
                    It.IsAny<GetBookingsByCaseTypesQuery>()), Times.Once);
        }

        [Test]
        public async Task Should_rebook_valid_hearing()
        {
            var hearing = GetHearing("123");
            hearing.UpdateStatus(BookingStatus.Failed, "administrator", string.Empty);
            QueryHandlerMock
                .Setup(x => x.Handle<GetHearingByIdQuery, VideoHearing>(It.IsAny<GetHearingByIdQuery>()))
                .ReturnsAsync(hearing);

            var result = await Controller.RebookHearing(hearing.Id);

            result.Should().NotBeNull();
            var objectResult = (NoContentResult)result;
            objectResult.StatusCode.Should().Be((int)HttpStatusCode.NoContent);
        }

        [Test]
        public async Task Should_return_not_found_when_rebooking_a_hearing_which_does_not_exist()
        {
            var hearingId = Guid.NewGuid();
            QueryHandlerMock
                .Setup(x => x.Handle<GetHearingByIdQuery, VideoHearing>(It.IsAny<GetHearingByIdQuery>()))
                .ReturnsAsync((VideoHearing)null);

            var result = await Controller.RebookHearing(hearingId);
            
            result.Should().NotBeNull();
            var objectResult = (NotFoundResult)result;
            objectResult.StatusCode.Should().Be((int)HttpStatusCode.NotFound);
        }

        [TestCase(BookingStatus.Created)]
        [TestCase(BookingStatus.Booked)]
        [TestCase(BookingStatus.Cancelled)]
        public async Task Should_return_bad_request_when_rebooking_a_hearing_with_invalid_status(BookingStatus status)
        {
            var hearing = GetHearing("123");
            if (hearing.Status != status)
            {
                hearing.UpdateStatus(status, "administrator", "reason");   
            }
            QueryHandlerMock
                .Setup(x => x.Handle<GetHearingByIdQuery, VideoHearing>(It.IsAny<GetHearingByIdQuery>()))
                .ReturnsAsync(hearing);
            
            var result = await Controller.RebookHearing(hearing.Id);
            
            result.Should().NotBeNull();
            var objectResult = (BadRequestObjectResult)result;
            objectResult.StatusCode.Should().Be((int)HttpStatusCode.BadRequest);
            ((SerializableError)objectResult.Value).ContainsValue($"Hearing must have a status of {nameof(BookingStatus.Failed)}");
        }
  
        protected static VideoHearing GetHearing(string caseNumber)
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