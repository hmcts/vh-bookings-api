using System.Collections.Generic;
using System.Net;
using BookingsApi.Contract.V1.Requests;
using BookingsApi.Contract.V1.Responses;
using BookingsApi.DAL.Commands.Core;
using BookingsApi.DAL.Queries;
using BookingsApi.DAL.Queries.Core;
using BookingsApi.Domain;
using BookingsApi.Domain.RefData;
using BookingsApi.Infrastructure.Services.AsynchronousProcesses;
using BookingsApi.Infrastructure.Services.IntegrationEvents;
using BookingsApi.Infrastructure.Services.Publishers;
using BookingsApi.Infrastructure.Services.ServiceBusQueue;
using BookingsApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace BookingsApi.UnitTests.Controllers.HearingsController
{
    public class HearingsControllerTests
    {
        protected BookingsApi.Controllers.V1.HearingsController Controller;
        protected Mock<IQueryHandler> QueryHandlerMock;
        protected Mock<ICommandHandler> CommandHandlerMock;

        private IEventPublisher _eventPublisher;
        protected Mock<IEventPublisher> EventPublisherMock;
        protected ServiceBusQueueClientFake SbQueueClient;

        protected IBookingAsynchronousProcess BookingAsynchronousProcess;
        protected IFirstdayOfMultidayBookingAsynchronousProcess FirstdayOfMultidayBookingAsyncProcess;
        protected IClonedBookingAsynchronousProcess ClonedBookingAsynchronousProcess;
        protected ICreateConferenceAsynchronousProcess CreateConferenceAsynchronousProcess;
        protected IEventPublisherFactory PublisherFactory;

        [SetUp]
        public void Setup()
        {
            SbQueueClient = new ServiceBusQueueClientFake();
            QueryHandlerMock = new Mock<IQueryHandler>();
            CommandHandlerMock = new Mock<ICommandHandler>();
            
            _eventPublisher = new EventPublisher(SbQueueClient);
            EventPublisherMock = new Mock<IEventPublisher>();
            PublisherFactory = EventPublisherFactoryInstance.Get(EventPublisherMock.Object);

            BookingAsynchronousProcess = new SingledayHearingAsynchronousProcess(PublisherFactory);
            FirstdayOfMultidayBookingAsyncProcess = new FirstdayOfMultidayHearingAsynchronousProcess(PublisherFactory);
            ClonedBookingAsynchronousProcess = new ClonedMultidaysAsynchronousProcess(PublisherFactory);
            CreateConferenceAsynchronousProcess = new CreateConferenceAsynchronousProcess(PublisherFactory);
            Controller = GetControllerObject(false);
        }

        protected BookingsApi.Controllers.V1.HearingsController GetControllerObject(bool withQueueClient)
        {
            var eventPublisher = withQueueClient ? _eventPublisher : EventPublisherMock.Object;
            var bookingService = new BookingService(eventPublisher, CommandHandlerMock.Object, QueryHandlerMock.Object,
                BookingAsynchronousProcess, FirstdayOfMultidayBookingAsyncProcess, ClonedBookingAsynchronousProcess, 
                CreateConferenceAsynchronousProcess);
            return new BookingsApi.Controllers.V1.HearingsController(QueryHandlerMock.Object, CommandHandlerMock.Object,
                bookingService);
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
            response.PrevPageUrl.Should().StartWith("hearings/types?types=1&types=2&cursor=0&limit=2");
            response.NextPageUrl.Should().StartWith("hearings/types?types=1&types=2&cursor=next-cursor&limit=2");
        }

        [Test]
        public async Task Should_return_bookings_list_for_participant_last_name_search()
        {
            var caseTypes = new List<int>();
            
            var lastName = "PARTICIPANT_LAST_NAME";

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
    }
}