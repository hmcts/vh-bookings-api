using System;
using System.Collections.Generic;
using BookingsApi.DAL.Queries.Core;
using BookingsApi.DAL.Services;
using BookingsApi.Domain;
using BookingsApi.Infrastructure.Services.IntegrationEvents;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using Testing.Common.Builders.Domain;

namespace BookingsApi.UnitTests.Controllers.WorkAllocationsController
{
    public class WorkAllocationsControllerTest
    {
        protected BookingsApi.Controllers.WorkAllocationsController Controller;
        protected Mock<IQueryHandler> QueryHandlerMock;
        protected Mock<IHearingAllocationService> HearingAllocationServiceMock;
        private Mock<ILogger<BookingsApi.Controllers.WorkAllocationsController>> _loggerMock;
        protected Mock<IEventPublisher> _eventPublisherMock;
        
        [SetUp]
        public void Setup()
        {
            _loggerMock = new Mock<ILogger<BookingsApi.Controllers.WorkAllocationsController>>();
            QueryHandlerMock = new Mock<IQueryHandler>();
            HearingAllocationServiceMock = new Mock<IHearingAllocationService>();
            _eventPublisherMock = new Mock<IEventPublisher>();

            Controller = new BookingsApi.Controllers.WorkAllocationsController(HearingAllocationServiceMock.Object,
                QueryHandlerMock.Object, _loggerMock.Object, _eventPublisherMock.Object);
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