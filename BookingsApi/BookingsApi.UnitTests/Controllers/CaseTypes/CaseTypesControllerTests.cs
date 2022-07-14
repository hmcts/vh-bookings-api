using BookingsApi.Common.Services;
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
        protected Mock<IFeatureToggles> FeatureTogglesMock;

        [SetUp]
        public void TestInitialize()
        {
            QueryHandler = new Mock<IQueryHandler>();
            FeatureTogglesMock = new Mock<IFeatureToggles>();
            FeatureTogglesMock.Setup(x => x.ReferenceDataToggle()).Returns(false);
            Controller = new CaseTypesController(QueryHandler.Object, FeatureTogglesMock.Object);
        }
    }
}
