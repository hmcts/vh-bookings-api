using Bookings.Api.Contract.Requests;
using Bookings.API.Controllers;
using Bookings.Common.Configuration;
using Bookings.Common.Services;
using Bookings.DAL.Commands.Core;
using Bookings.Infrastructure.Services.IntegrationEvents;
using Bookings.Infrastructure.Services.ServiceBusQueue;
using Microsoft.Extensions.Options;
using Moq;
using NUnit.Framework;
using System;

namespace Bookings.UnitTests.Controllers.EndPointController
{
    public class EndPointsControllerTests
    {
        protected AddEndpointRequest _request;
        protected Guid _hearingId;

        protected Mock<ICommandHandler> CommandHandlerMock;
        protected Mock<IRandomGenerator> RandomGenerator;
        private IEventPublisher _eventPublisher;
        private ServiceBusQueueClientFake _sbQueueClient;
        protected KinlyConfiguration KinlyConfiguration;

        protected EndPointsController Controller;

        [SetUp]
        public void TestInitialize()
        {
            _hearingId = Guid.NewGuid();
            _request = new AddEndpointRequest { DisplayName = "DisplayNameAdded" };

            _sbQueueClient = new ServiceBusQueueClientFake();
            CommandHandlerMock = new Mock<ICommandHandler>();
            RandomGenerator = new Mock<IRandomGenerator>();
            _eventPublisher = new EventPublisher(_sbQueueClient);
            KinlyConfiguration = new KinlyConfiguration { SipAddressStem = "@videohearings.com" };

            Controller = new EndPointsController(CommandHandlerMock.Object, RandomGenerator.Object, new OptionsWrapper<KinlyConfiguration>(KinlyConfiguration));
        }
    }
}
