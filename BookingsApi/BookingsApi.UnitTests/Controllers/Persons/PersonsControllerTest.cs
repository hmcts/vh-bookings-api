using BookingsApi.Controllers.V1;
using BookingsApi.DAL.Commands.Core;
using BookingsApi.DAL.Queries.Core;
using BookingsApi.Infrastructure.Services.IntegrationEvents;
using Microsoft.Extensions.Logging;

namespace BookingsApi.UnitTests.Controllers.Persons
{
    public class PersonsControllerTest
    {
        protected PersonsController Controller;
        protected Mock<IQueryHandler> QueryHandlerMock;
        protected Mock<ICommandHandler> CommandHandlerMock;
        protected Mock<IEventPublisher> EventPublisherMock;
        protected Mock<ILogger<PersonsController>> LoggerMock;

        [SetUp]
        public void Setup()
        {
            QueryHandlerMock = new Mock<IQueryHandler>();
            CommandHandlerMock = new Mock<ICommandHandler>();
            EventPublisherMock = new Mock<IEventPublisher>();
            LoggerMock = new Mock<ILogger<PersonsController>>();
            Controller = new PersonsController(QueryHandlerMock.Object, CommandHandlerMock.Object,
                EventPublisherMock.Object, LoggerMock.Object);
        }
    }
}
