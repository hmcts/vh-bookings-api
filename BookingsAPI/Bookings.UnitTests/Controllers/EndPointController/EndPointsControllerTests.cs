using Bookings.API.Controllers;
using Bookings.Common.Services;
using Bookings.DAL.Commands.Core;
using Bookings.Infrastructure.Services.IntegrationEvents;
using Bookings.Infrastructure.Services.ServiceBusQueue;
using Moq;
using NUnit.Framework;

namespace Bookings.UnitTests.Controllers.EndPointController
{
    public class EndPointsControllerTests
    {
        protected Mock<ICommandHandler> CommandHandlerMock;
        protected Mock<IRandomGenerator> RandomGenerator;
        private IEventPublisher _eventPublisher;
        private ServiceBusQueueClientFake _sbQueueClient;

        protected EndPointsController Controller;

        [SetUp]
        public void TestInitialize()
        {
            _sbQueueClient = new ServiceBusQueueClientFake();
            CommandHandlerMock = new Mock<ICommandHandler>();
            RandomGenerator = new Mock<IRandomGenerator>();
            _eventPublisher = new EventPublisher(_sbQueueClient);

            Controller = new EndPointsController(CommandHandlerMock.Object, _eventPublisher, RandomGenerator.Object);
        }
    }
}
