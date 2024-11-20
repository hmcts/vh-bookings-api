using BookingsApi.Controllers.V1;
using BookingsApi.DAL.Commands.Core;
using BookingsApi.DAL.Queries.Core;

namespace BookingsApi.UnitTests.Controllers.Persons
{
    public class PersonsControllerTest
    {
        protected PersonsController Controller;
        protected Mock<IQueryHandler> QueryHandlerMock;
        protected Mock<ICommandHandler> CommandHandlerMock;

        [SetUp]
        public void Setup()
        {
            QueryHandlerMock = new Mock<IQueryHandler>();
            CommandHandlerMock = new Mock<ICommandHandler>();
            Controller = new PersonsController(QueryHandlerMock.Object, CommandHandlerMock.Object);
        }
    }
}
