using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Bookings.Api.Contract.Requests;
using Bookings.Api.Contract.Responses;
using Bookings.Common.Configuration;
using Bookings.Common.Services;
using Bookings.DAL.Commands;
using Bookings.DAL.Commands.Core;
using Bookings.DAL.Queries;
using Bookings.DAL.Queries.Core;
using Bookings.Domain;
using Bookings.Domain.RefData;
using Bookings.Infrastructure.Services.IntegrationEvents;
using Bookings.Infrastructure.Services.IntegrationEvents.Events;
using Bookings.Infrastructure.Services.ServiceBusQueue;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Moq;
using NUnit.Framework;
using Testing.Common.Assertions;
using Testing.Common.Builders.Domain;

namespace Bookings.UnitTests.Controllers.HearingsController
{
    public class HearingsControllerTest
    {
        protected API.Controllers.HearingsController Controller;
        protected Mock<IQueryHandler> QueryHandlerMock;
        protected Mock<ICommandHandler> CommandHandlerMock;
        protected Mock<IRandomGenerator> RandomGenerator;
        protected KinlyConfiguration KinlyConfiguration;

        private IEventPublisher _eventPublisher;
        private ServiceBusQueueClientFake _sbQueueClient;

        [SetUp]
        public void Setup()
        {
            _sbQueueClient = new ServiceBusQueueClientFake();
            QueryHandlerMock = new Mock<IQueryHandler>();
            CommandHandlerMock = new Mock<ICommandHandler>();
            KinlyConfiguration = new KinlyConfiguration {SipAddressStem = "@WhereAreYou.com"};
            RandomGenerator = new Mock<IRandomGenerator>();
            _eventPublisher = new EventPublisher(_sbQueueClient);

            Controller = new API.Controllers.HearingsController(QueryHandlerMock.Object, CommandHandlerMock.Object,
                _eventPublisher, RandomGenerator.Object, new OptionsWrapper<KinlyConfiguration>(KinlyConfiguration));
        }

        [Test]
        public async Task Should_return_bad_request_if_invalid_case_types()
        {
            var caseTypes = new List<int> {44, 78};
            QueryHandlerMock
                .Setup(x => x.Handle<GetAllCaseTypesQuery, List<CaseType>>(It.IsAny<GetAllCaseTypesQuery>()))
                .ReturnsAsync(new List<CaseType> {new CaseType(44, "Financial"), new CaseType(2, "Civil")});

            var result = await Controller.GetHearingsByTypes(caseTypes, "0", 2);

            result.Should().NotBeNull();
            result.Should().NotBeNull();
            var objectResult = (ObjectResult) result.Result;
            objectResult.StatusCode.Should().Be((int) HttpStatusCode.BadRequest);
            ((SerializableError) objectResult.Value).ContainsKeyAndErrorMessage("Hearing types",
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

            var result = await Controller.GetHearingsByTypes(caseTypes, "0", 2);

            result.Should().NotBeNull();
            var objectResult = (ObjectResult) result.Result;
            objectResult.StatusCode.Should().Be((int) HttpStatusCode.OK);
            var response = (BookingsResponse) ((ObjectResult) result.Result).Value;
            response.PrevPageUrl.Should().Be("hearings/types?types=&cursor=0&limit=2");
            response.NextPageUrl.Should().Be("hearings/types?types=&cursor=next cursor&limit=2");
            QueryHandlerMock.Verify(q => q.Handle<GetBookingsByCaseTypesQuery, CursorPagedResult<VideoHearing, string>>
                (It.Is<GetBookingsByCaseTypesQuery>(g => g.Cursor == null)), Times.Once);
        }

        [Test]
        public async Task Should_return_next_and_previous_page_urls()
        {
            var caseTypes = new List<int> {1, 2};
            QueryHandlerMock
                .Setup(x => x.Handle<GetAllCaseTypesQuery, List<CaseType>>(It.IsAny<GetAllCaseTypesQuery>()))
                .ReturnsAsync(new List<CaseType> {new CaseType(1, "Financial"), new CaseType(2, "Civil")});

            QueryHandlerMock
                .Setup(x =>
                    x.Handle<GetBookingsByCaseTypesQuery, CursorPagedResult<VideoHearing, string>>(
                        It.IsAny<GetBookingsByCaseTypesQuery>()))
                .ReturnsAsync(new CursorPagedResult<VideoHearing, string>(new List<VideoHearing>(), "next-cursor"));
            var result = await Controller.GetHearingsByTypes(caseTypes, "0", 2);

            result.Should().NotBeNull();
            var response = (BookingsResponse) ((ObjectResult) result.Result).Value;
            response.PrevPageUrl.Should().Be("hearings/types?types=1&types=2&cursor=0&limit=2");
            response.NextPageUrl.Should().Be("hearings/types?types=1&types=2&cursor=next-cursor&limit=2");
        }

        [Test]
        public async Task Should_change_hearing_status_to_created_and_send_event_notification()
        {
            var request = new UpdateBookingStatusRequest
            {
                UpdatedBy = "email@toupdate.com",
                Status = Api.Contract.Requests.Enums.UpdateBookingStatus.Created
            };
            var hearingId = Guid.NewGuid();
            var hearing = GetHearing("123");

            QueryHandlerMock
                .Setup(x => x.Handle<GetHearingByIdQuery, VideoHearing>(It.IsAny<GetHearingByIdQuery>()))
                .ReturnsAsync(hearing);

            var result = await Controller.UpdateBookingStatus(hearingId, request);

            result.Should().NotBeNull();
            var objectResult = (NoContentResult) result;
            objectResult.StatusCode.Should().Be((int) HttpStatusCode.NoContent);

            var message = _sbQueueClient.ReadMessageFromQueue();
            var typedMessage = (HearingIsReadyForVideoIntegrationEvent) message.IntegrationEvent;
            typedMessage.Should().NotBeNull();
            typedMessage.Hearing.HearingId.Should().Be(hearing.Id);
        }

        [Test]
        public async Task Should_change_hearing_status_to_cancelled()
        {
            var request = new UpdateBookingStatusRequest
            {
                UpdatedBy = "email@toupdate.com",
                Status = Api.Contract.Requests.Enums.UpdateBookingStatus.Cancelled,
                CancelReason = "Adjournment"
            };
            var hearingId = Guid.NewGuid();
            var hearing = GetHearing("123");

            QueryHandlerMock
                .Setup(x => x.Handle<GetHearingByIdQuery, VideoHearing>(It.IsAny<GetHearingByIdQuery>()))
                .ReturnsAsync(hearing);

            var result = await Controller.UpdateBookingStatus(hearingId, request);

            result.Should().NotBeNull();
            var objectResult = (NoContentResult) result;
            objectResult.StatusCode.Should().Be((int) HttpStatusCode.NoContent);

            var message = _sbQueueClient.ReadMessageFromQueue();
            var typedMessage = (HearingCancelledIntegrationEvent) message.IntegrationEvent;
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
            var objectResult = (BadRequestObjectResult) result;
            objectResult.StatusCode.Should().Be((int) HttpStatusCode.BadRequest);
            ((SerializableError) objectResult.Value).ContainsKeyAndErrorMessage(nameof(hearingId),
                $"Please provide a valid {nameof(hearingId)}");
        }

        [Test]
        public async Task Should_update_hearing_with_status_created_and_send_event_to_video()
        {
            var request = new UpdateHearingRequest
            {
                ScheduledDateTime = DateTime.Now.AddDays(2),
                HearingRoomName = "123",
                ScheduledDuration = 15,
                OtherInformation = "note",
                HearingVenueName = "venue1",
                Cases = new List<CaseRequest> {new CaseRequest {Name = "123XX", Number = "123YY", IsLeadCase = true}},
            };

            var hearingId = Guid.NewGuid();
            var hearing = GetHearing("123");
            hearing.UpdateStatus(Bookings.Domain.Enumerations.BookingStatus.Created, "administrator", string.Empty);
            hearing.UpdateHearingDetails(new HearingVenue(1, "venue1"), DateTime.Now.AddDays(2),
                15, "123", "note", "administrator", new List<Case> {new Case("123", "name")}, true, true);
            QueryHandlerMock
                .Setup(x => x.Handle<GetHearingByIdQuery, VideoHearing>(It.IsAny<GetHearingByIdQuery>()))
                .ReturnsAsync(hearing);

            var venues = new List<HearingVenue> {new HearingVenue(1, "venue1"),};
            QueryHandlerMock
                .Setup(x => x.Handle<GetHearingVenuesQuery, List<HearingVenue>>(It.IsAny<GetHearingVenuesQuery>()))
                .ReturnsAsync(venues);

            var result = await Controller.UpdateHearingDetails(hearingId, request);

            result.Should().NotBeNull();
            var objectResult = (OkObjectResult) result;
            objectResult.StatusCode.Should().Be((int) HttpStatusCode.OK);

            var message = _sbQueueClient.ReadMessageFromQueue();
            var typedMessage = (HearingDetailsUpdatedIntegrationEvent) message.IntegrationEvent;
            typedMessage.Should().NotBeNull();
            typedMessage.Hearing.CaseName.Should().Be("name");
        }

        [Test]
        public async Task Should_return_notfound_when_no_matching_venue_found()
        {
            var request = new UpdateHearingRequest
            {
                ScheduledDateTime = DateTime.Now.AddDays(2),
                HearingRoomName = "123",
                ScheduledDuration = 15,
                OtherInformation = "note",
                HearingVenueName = "venue2",
                Cases = new List<CaseRequest> {new CaseRequest {Name = "123XX", Number = "123YY", IsLeadCase = true}},
            };

            var hearingId = Guid.NewGuid();
            var hearing = GetHearing("123");
            hearing.UpdateStatus(Bookings.Domain.Enumerations.BookingStatus.Created, "administrator", string.Empty);
            hearing.UpdateHearingDetails(new HearingVenue(1, "venue1"), DateTime.Now.AddDays(2),
                15, "123", "note", "administrator", new List<Case> {new Case("123", "name")}, true, true);
            QueryHandlerMock
                .Setup(x => x.Handle<GetHearingByIdQuery, VideoHearing>(It.IsAny<GetHearingByIdQuery>()))
                .ReturnsAsync(hearing);

            var venues = new List<HearingVenue> {new HearingVenue(1, "venue1"),};
            QueryHandlerMock
                .Setup(x => x.Handle<GetHearingVenuesQuery, List<HearingVenue>>(It.IsAny<GetHearingVenuesQuery>()))
                .ReturnsAsync(venues);

            var result = await Controller.UpdateHearingDetails(hearingId, request);

            result.Should().NotBeNull();
            var objectResult = (BadRequestObjectResult) result;
            objectResult.StatusCode.Should().Be((int) HttpStatusCode.BadRequest);
            ((SerializableError) objectResult.Value).ContainsKeyAndErrorMessage(nameof(request.HearingVenueName),
                "Hearing venue does not exist");
        }

        [Test]
        public async Task Should_return_badrequest_with_an_invalid_hearingid()
        {
            var request = new UpdateHearingRequest();
            var hearingId = Guid.Empty;

            var result = await Controller.UpdateHearingDetails(hearingId, request);

            result.Should().NotBeNull();
            var objectResult = (BadRequestObjectResult) result;
            objectResult.StatusCode.Should().Be((int) HttpStatusCode.BadRequest);
            ((SerializableError) objectResult.Value).ContainsKeyAndErrorMessage(nameof(hearingId),
                $"Please provide a valid {nameof(hearingId)}");
        }

        [Test]
        public async Task Should_remove_hearing_with_status_created_and_send_event_to_video()
        {
            var hearingId = Guid.NewGuid();
            var hearing = GetHearing("123");
            hearing.UpdateStatus(Bookings.Domain.Enumerations.BookingStatus.Created, "administrator", string.Empty);

            QueryHandlerMock
                .Setup(x => x.Handle<GetHearingByIdQuery, VideoHearing>(It.IsAny<GetHearingByIdQuery>()))
                .ReturnsAsync(hearing);

            var result = await Controller.RemoveHearing(hearingId);

            result.Should().NotBeNull();
            var objectResult = (NoContentResult) result;
            objectResult.StatusCode.Should().Be((int) HttpStatusCode.NoContent);

            var message = _sbQueueClient.ReadMessageFromQueue();
            var typedMessage = (HearingCancelledIntegrationEvent) message.IntegrationEvent;
            typedMessage.Should().NotBeNull();
            typedMessage.HearingId.Should().Be(hearingId);
        }

        [Test]
        public async Task Should_not_remove_hearing_and_return_badrequest_with_an_invalid_hearingid()
        {
            var hearingId = Guid.Empty;

            var result = await Controller.RemoveHearing(hearingId);

            result.Should().NotBeNull();
            var objectResult = (BadRequestObjectResult) result;
            objectResult.StatusCode.Should().Be((int) HttpStatusCode.BadRequest);
            ((SerializableError) objectResult.Value).ContainsKeyAndErrorMessage(nameof(hearingId),
                $"Please provide a valid {nameof(hearingId)}");
        }

        [Test]
        public async Task Should_change_hearing_status_to_failed()
        {
            var request = new UpdateBookingStatusRequest
            {
                UpdatedBy = "email@toupdate.com",
                Status = Api.Contract.Requests.Enums.UpdateBookingStatus.Failed,
                CancelReason = ""
            };
            var hearingId = Guid.NewGuid();

            CommandHandlerMock.Setup(x => x.Handle(It.IsAny<UpdateHearingStatusCommand>()));

            var result = await Controller.UpdateBookingStatus(hearingId, request);

            result.Should().NotBeNull();
            var objectResult = (NoContentResult) result;
            objectResult.StatusCode.Should().Be((int) HttpStatusCode.NoContent);

            CommandHandlerMock.Verify(c => c.Handle(It.IsAny<UpdateHearingStatusCommand>()), Times.Once);
        }

        protected static VideoHearing GetHearing(string caseNumber)
        {
            var hearing = new VideoHearingBuilder().Build();
            hearing.AddCase(caseNumber, "Case name", true);

            foreach (var participant in hearing.Participants)
            {
                participant.HearingRole = new HearingRole(1, "Name") {UserRole = new UserRole(1, "User"),};
                participant.CaseRole = new CaseRole(1, "Name");
            }

            hearing.AddEndpoints(new List<Endpoint>
                {new Endpoint("new endpoint", Guid.NewGuid().ToString(), "pin", null)});

            return hearing;
        }
    }
}
