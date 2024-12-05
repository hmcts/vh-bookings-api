using BookingsApi.DAL.Queries;
using BookingsApi.DAL.Queries.BaseQueries;

namespace BookingsApi.IntegrationTests.Database.Queries
{
    public class GetAllCaseTypesQueryHandlerDatabaseTests : DatabaseTestsBase
    {
        private GetAllCaseTypesQueryHandler _handler;
        private static int FinancialRemedyCaseTypeId => 2;
        private DateTime? _originalCaseTypeExpirationDate;

        [SetUp]
        public async Task Setup()
        {
            var context = new BookingsDbContext(BookingsDbContextOptions);
            _handler = new GetAllCaseTypesQueryHandler(context);
            await InitGetOriginalExpirationDate();
        }

        protected override async Task Cleanup()
        {
            await UpdateCaseTypeExpirationDate(_originalCaseTypeExpirationDate);
        }

        [Test]
        public async Task Should_return_all_case_types_including_expired()
        {
            var caseTypes = await _handler.Handle(new GetAllCaseTypesQuery(includeDeleted:true));
            var financialRemedy = caseTypes.First(c => c.Name == "Financial Remedy");
            financialRemedy.Should().NotBeNull();
        }
        
        [Test]
        public async Task Should_return_all_case_types_except_expired()
        {
            // arrange
            var expiredDate = DateTime.Today.AddDays(-1);
            await UpdateCaseTypeExpirationDate(expiredDate);

            // act
            var caseTypes = await _handler.Handle(new GetAllCaseTypesQuery(includeDeleted:false));

            // assert
            caseTypes.Should().NotContain(c => c.Id == FinancialRemedyCaseTypeId);
        }
        
        private async Task InitGetOriginalExpirationDate()
        {
            await using var db = new BookingsDbContext(BookingsDbContextOptions);
            var caseTypeFr = await CaseTypes.Get(db).FirstAsync(x=> x.Id == FinancialRemedyCaseTypeId);
            _originalCaseTypeExpirationDate = caseTypeFr!.ExpirationDate;
        }
        
        private async Task UpdateCaseTypeExpirationDate(DateTime? expirationDate)
        {
            await using var db = new BookingsDbContext(BookingsDbContextOptions);
            var caseType = await CaseTypes.Get(db).FirstAsync(x=> x.Id == FinancialRemedyCaseTypeId);
            caseType!.ExpirationDate = expirationDate;
            await db.SaveChangesAsync();
        }
    }
}