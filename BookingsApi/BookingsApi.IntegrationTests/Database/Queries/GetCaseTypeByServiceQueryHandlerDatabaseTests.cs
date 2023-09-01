using BookingsApi.Common.Services;
using BookingsApi.DAL.Queries;
using BookingsApi.Domain.RefData;
using Moq;

namespace BookingsApi.IntegrationTests.Database.Queries
{
    public class GetCaseTypeByServiceQueryHandlerDatabaseTests : DatabaseTestsBase
    {
        private GetCaseRolesForCaseServiceQueryHandler _handler;

        [SetUp]
        public void Setup()
        {
            var context = new BookingsDbContext(BookingsDbContextOptions);
            _handler = new GetCaseRolesForCaseServiceQueryHandler(context);
        }
        
        [Test] public async Task should_get_case_type_by_service_id()
        {
            var caseTypeName = "Generic";
            var serviceId = "vhG1"; // intentionally not the same case to verify case sensitivity is ignored
            var caseType = await _handler.Handle(new GetCaseRolesForCaseServiceQuery(serviceId));
            caseType.Should().NotBeNull();
            caseType.Name.Should().Be(caseTypeName);
            caseType.HearingTypes.Should().NotBeEmpty();
        }
    }
}