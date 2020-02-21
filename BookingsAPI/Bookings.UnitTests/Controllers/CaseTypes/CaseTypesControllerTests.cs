
using Bookings.API.Controllers;
using Bookings.DAL.Queries.Core;
using Moq;
using NUnit.Framework;

namespace Bookings.UnitTests.Controllers
{
    public class CaseTypesControllerTests
    {
        protected Mock<IQueryHandler> QueryHandler;
        protected CaseTypesController Controller;

        [SetUp]
        public void TestInitialize()
        {
            QueryHandler = new Mock<IQueryHandler>();

            Controller = new CaseTypesController(QueryHandler.Object);
        }
    }
}
