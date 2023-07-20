using System;
using System.Collections.Generic;
using BookingsApi.DAL.Queries.Core;
using BookingsApi.DAL.Services;
using BookingsApi.Domain;
using BookingsApi.Infrastructure.Services.IntegrationEvents;
using BookingsApi.Infrastructure.Services.ServiceBusQueue;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using Testing.Common.Builders.Domain;

namespace BookingsApi.UnitTests.Controllers.WorkAllocationsController
{
    public class WorkAllocationsControllerTest
    {
        protected BookingsApi.Controllers.V1.WorkAllocationsController Controller;
        protected Mock<IQueryHandler> QueryHandlerMock;
        protected Mock<IHearingAllocationService> HearingAllocationServiceMock;
        protected ServiceBusQueueClientFake ServiceBus { get; set; }
        private Mock<ILogger<BookingsApi.Controllers.V1.WorkAllocationsController>> _loggerMock;

        [SetUp]
        public void Setup()
        {
            _loggerMock = new Mock<ILogger<BookingsApi.Controllers.V1.WorkAllocationsController>>();
            QueryHandlerMock = new Mock<IQueryHandler>();
            HearingAllocationServiceMock = new Mock<IHearingAllocationService>();
            ServiceBus = new ServiceBusQueueClientFake();

            Controller = new BookingsApi.Controllers.V1.WorkAllocationsController(HearingAllocationServiceMock.Object,
                QueryHandlerMock.Object, _loggerMock.Object, new EventPublisher(ServiceBus));
        }
        
        protected static VideoHearing GetHearing(string caseNumber)
        {
            var hearing = new VideoHearingBuilder().Build();

            if (!string.IsNullOrEmpty(caseNumber))
            {
                hearing.AddCase(caseNumber, "Case name", true);
            }

            hearing.AddEndpoints(new List<Endpoint>
                { new("new endpoint", Guid.NewGuid().ToString(), "pin", null) });

            return hearing;
        }
    }
}