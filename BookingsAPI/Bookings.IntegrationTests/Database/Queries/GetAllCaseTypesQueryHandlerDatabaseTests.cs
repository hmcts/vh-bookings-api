using System.Linq;
using System.Threading.Tasks;
using Bookings.DAL;
using Bookings.DAL.Queries;
using Bookings.Domain.RefData;
using FluentAssertions;
using NUnit.Framework;

namespace Bookings.IntegrationTests.Database.Queries
{
    public class GetAllCaseTypesQueryHandlerDatabaseTests : DatabaseTestsBase
    {
        private GetAllCaseTypesQueryHandler _handler;

        [SetUp]
        public void Setup()
        {
            var context = new BookingsDbContext(BookingsDbContextOptions);
            _handler = new GetAllCaseTypesQueryHandler(context);
        }

        [Test]
        public async Task Should_return_all_case_types_and_their_hearing_types()
        {
            var caseTypes = await _handler.Handle(new GetAllCaseTypesQuery());
            var financialRemedy = caseTypes.FirstOrDefault(c => c.Name == "Financial Remedy");
            financialRemedy.Should().NotBeNull();
            financialRemedy.HearingTypes.Count.Should().BePositive();
        }
    }
}