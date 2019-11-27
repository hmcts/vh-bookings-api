﻿using Bookings.API.Controllers;
using Bookings.DAL.Commands.Core;
using Bookings.DAL.Queries;
using Bookings.DAL.Queries.Core;
using Bookings.Domain;
using Bookings.Domain.RefData;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Bookings.Api.Contract.Responses;
using Bookings.Infrastructure.Services.IntegrationEvents;
using Bookings.Infrastructure.Services.ServiceBusQueue;
using Bookings.Api.Contract.Requests;
using System;
using Testing.Common.Builders.Domain;
using Bookings.Infrastructure.Services.IntegrationEvents.Events;

namespace Bookings.UnitTests.Controllers
{
    public class HearingsControllerTest
    {
        private HearingsController _controller;
        private Mock<IQueryHandler> _queryHandlerMock;
        private Mock<ICommandHandler> _commandHandlerMock;
        private IEventPublisher _eventPublisher;
        private ServiceBusQueueClientFake _sbQueueClient;

        [SetUp]
        public void Setup()
        {
            _sbQueueClient = new ServiceBusQueueClientFake();
            _queryHandlerMock = new Mock<IQueryHandler>();
            _commandHandlerMock = new Mock<ICommandHandler>();
            _eventPublisher = new EventPublisher(_sbQueueClient);
            _controller = new HearingsController(_queryHandlerMock.Object, _commandHandlerMock.Object,
                _eventPublisher);
        }

        [Test]
        public async Task should_return_bad_request_if_invalid_case_types()
        {
            var caseTypes = new List<int> { 44, 78 };
            _queryHandlerMock
             .Setup(x => x.Handle<GetAllCaseTypesQuery, List<CaseType>>(It.IsAny<GetAllCaseTypesQuery>()))
             .ReturnsAsync(new List<CaseType>());

            var result = await _controller.GetHearingsByTypes(caseTypes, "0", 2);

            result.Should().NotBeNull();
            var objectResult = (ObjectResult)result.Result;
            objectResult.StatusCode.Should().Be((int)HttpStatusCode.BadRequest);
        }

        [Test]
        public async Task should_return_bookings()
        {
            var caseTypes = new List<int>();
            _queryHandlerMock
             .Setup(x => x.Handle<GetAllCaseTypesQuery, List<CaseType>>(It.IsAny<GetAllCaseTypesQuery>()))
             .ReturnsAsync(new List<CaseType>());

            _queryHandlerMock
                .Setup(x => x.Handle<GetBookingsByCaseTypesQuery, CursorPagedResult<VideoHearing, string>>(It.IsAny<GetBookingsByCaseTypesQuery>()))
                .ReturnsAsync(new CursorPagedResult<VideoHearing, string>(new List<VideoHearing>(), "next cursor"));
            var result = await _controller.GetHearingsByTypes(caseTypes, "0", 2);

            result.Should().NotBeNull();
            var objectResult = (ObjectResult)result.Result;
            objectResult.StatusCode.Should().Be((int)HttpStatusCode.OK);
        }

        [Test]
        public async Task should_return_next_and_previous_page_urls()
        {
            var caseTypes = new List<int> { 1 };
            _queryHandlerMock
                .Setup(x => x.Handle<GetAllCaseTypesQuery, List<CaseType>>(It.IsAny<GetAllCaseTypesQuery>()))
                .ReturnsAsync(new List<CaseType> { new CaseType(1, "Financial") });

            _queryHandlerMock
                .Setup(x => x.Handle<GetBookingsByCaseTypesQuery, CursorPagedResult<VideoHearing, string>>(It.IsAny<GetBookingsByCaseTypesQuery>()))
                .ReturnsAsync(new CursorPagedResult<VideoHearing, string>(new List<VideoHearing>(), "next-cursor"));
            var result = await _controller.GetHearingsByTypes(caseTypes, "0", 2);

            result.Should().NotBeNull();
            var response = (BookingsResponse)((ObjectResult)result.Result).Value;
            response.PrevPageUrl.Should().Be("hearings/types?types=1&cursor=0&limit=2");
            response.NextPageUrl.Should().Be("hearings/types?types=1&cursor=next-cursor&limit=2");
        }

        [Test]
        public async Task should_change_hearing_status_to_created_and_send_event_notification()
        {
            var request = new UpdateBookingStatusRequest
            {
                UpdatedBy = "email@toupdate.com",
                Status = Api.Contract.Requests.Enums.UpdateBookingStatus.Created
            };
            var hearingId = Guid.NewGuid();
            var hearing = GetHearing();

            _queryHandlerMock
             .Setup(x => x.Handle<GetHearingByIdQuery, VideoHearing>(It.IsAny<GetHearingByIdQuery>()))
             .ReturnsAsync(hearing);

            var result = await _controller.UpdateBookingStatus(hearingId, request);

            result.Should().NotBeNull();
            var objectResult = (NoContentResult)result;
            objectResult.StatusCode.Should().Be((int)HttpStatusCode.NoContent);

            var message = _sbQueueClient.ReadMessageFromQueue();
            var typedMessage = (HearingIsReadyForVideoIntegrationEvent)message.IntegrationEvent;
            typedMessage.Should().NotBeNull();
            typedMessage.Hearing.HearingId.Should().Be(hearing.Id);
        }

        [Test]
        public async Task should_change_hearing_status_to_cancelled()
        {
            var request = new UpdateBookingStatusRequest
            {
                UpdatedBy = "email@toupdate.com",
                Status = Api.Contract.Requests.Enums.UpdateBookingStatus.Cancelled
            };
            var hearingId = Guid.NewGuid();
            var hearing = GetHearing();

            _queryHandlerMock
             .Setup(x => x.Handle<GetHearingByIdQuery, VideoHearing>(It.IsAny<GetHearingByIdQuery>()))
             .ReturnsAsync(hearing);

            var result = await _controller.UpdateBookingStatus(hearingId, request);

            result.Should().NotBeNull();
            var objectResult = (NoContentResult)result;
            objectResult.StatusCode.Should().Be((int)HttpStatusCode.NoContent);

            var message = _sbQueueClient.ReadMessageFromQueue();
            var typedMessage = (HearingCancelledIntegrationEvent)message.IntegrationEvent;
            typedMessage.Should().NotBeNull();
            typedMessage.HearingId.Should().Be(hearingId);
        }

        [Test]
        public async Task should_update_hearing_with_status_created_and_send_event_to_video()
        {
            var request = new UpdateHearingRequest
            {
                ScheduledDateTime = DateTime.Now.AddDays(2),
                HearingRoomName = "123",
                ScheduledDuration = 15,
                OtherInformation = "note",
                HearingVenueName = "venue1",
                Cases = new List<CaseRequest> { new CaseRequest { Name = "123XX", Number = "123YY", IsLeadCase = true } },
            };

            var hearingId = Guid.NewGuid();
            var hearing = GetHearing();
            hearing.UpdateStatus(Bookings.Domain.Enumerations.BookingStatus.Created, "administrator");
            hearing.UpdateHearingDetails(new HearingVenue(1, "venue1"), DateTime.Now.AddDays(2),
                15, "123", "note", "administrator", new List<Case> { new Case("123", "name") }, true, false);
            _queryHandlerMock
            .Setup(x => x.Handle<GetHearingByIdQuery, VideoHearing>(It.IsAny<GetHearingByIdQuery>()))
            .ReturnsAsync(hearing);

            var venues = new List<HearingVenue> { new HearingVenue(1, "venue1"), };
            _queryHandlerMock
           .Setup(x => x.Handle<GetHearingVenuesQuery, List<HearingVenue>>(It.IsAny<GetHearingVenuesQuery>()))
           .ReturnsAsync(venues);

              var result = await _controller.UpdateHearingDetails(hearingId, request);

            result.Should().NotBeNull();
            var objectResult = (OkObjectResult)result;
            objectResult.StatusCode.Should().Be((int)HttpStatusCode.OK);

            var message = _sbQueueClient.ReadMessageFromQueue();
            var typedMessage = (HearingDetailsUpdatedIntegrationEvent)message.IntegrationEvent;
            typedMessage.Should().NotBeNull();
            typedMessage.Hearing.CaseName.Should().Be("name");
        }

        [Test]
        public async Task should_remove_hearing_with_status_created_and_send_event_to_video()
        {
            var hearingId = Guid.NewGuid();
            var hearing = GetHearing();
            hearing.UpdateStatus(Bookings.Domain.Enumerations.BookingStatus.Created, "administrator");

            _queryHandlerMock
            .Setup(x => x.Handle<GetHearingByIdQuery, VideoHearing>(It.IsAny<GetHearingByIdQuery>()))
            .ReturnsAsync(hearing);

            var result = await _controller.RemoveHearing(hearingId);

            result.Should().NotBeNull();
            var objectResult = (NoContentResult)result;
            objectResult.StatusCode.Should().Be((int)HttpStatusCode.NoContent);

            var message = _sbQueueClient.ReadMessageFromQueue();
            var typedMessage = (HearingCancelledIntegrationEvent)message.IntegrationEvent;
            typedMessage.Should().NotBeNull();
            typedMessage.HearingId.Should().Be(hearingId);
        }

        private VideoHearing GetHearing()
        {
            var hearing = new VideoHearingBuilder().Build();
            hearing.AddCase("123", "Case name", true);
            foreach (var participant in hearing.Participants)
            {
                participant.HearingRole = new HearingRole(1, "Name") { UserRole = new UserRole(1, "User"), };
                participant.CaseRole = new CaseRole(1, "Name");
            }

            return hearing;
        }
    }
}
