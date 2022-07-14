using BookingsApi.Controllers;
using BookingsApi.DAL.Queries.Core;
using Moq;
using NUnit.Framework;

namespace BookingsApi.UnitTests.Controllers.CaseTypes
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
