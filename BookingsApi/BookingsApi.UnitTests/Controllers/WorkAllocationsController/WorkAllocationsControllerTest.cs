using BookingsApi.DAL.Queries.Core;
using BookingsApi.DAL.Services;
using BookingsApi.Domain;
using BookingsApi.Infrastructure.Services.IntegrationEvents;
using BookingsApi.Infrastructure.Services.ServiceBusQueue;

namespace BookingsApi.UnitTests.Controllers.WorkAllocationsController
{
    public class WorkAllocationsControllerTest
    {
        protected BookingsApi.Controllers.V1.WorkAllocationsController Controller;
        protected Mock<IQueryHandler> QueryHandlerMock;
        protected Mock<IHearingAllocationService> HearingAllocationServiceMock;
        protected ServiceBusQueueClientFake ServiceBus { get; set; }

        [SetUp]
        public void Setup()
        {
            QueryHandlerMock = new Mock<IQueryHandler>();
            HearingAllocationServiceMock = new Mock<IHearingAllocationService>();
            ServiceBus = new ServiceBusQueueClientFake();

            Controller = new BookingsApi.Controllers.V1.WorkAllocationsController(HearingAllocationServiceMock.Object,
                QueryHandlerMock.Object, new EventPublisher(ServiceBus));
        }
        
        protected static VideoHearing GetHearing(string caseNumber)
        {
            var hearing = new VideoHearingBuilder().Build();

            if (!string.IsNullOrEmpty(caseNumber))
            {
                hearing.AddCase(caseNumber, "Case name", true);
            }

            hearing.AddEndpoints([
                new Endpoint(Guid.NewGuid().ToString(), "new endpoint", Guid.NewGuid().ToString(), "pin")
            ]);

            return hearing;
        }
    }
}