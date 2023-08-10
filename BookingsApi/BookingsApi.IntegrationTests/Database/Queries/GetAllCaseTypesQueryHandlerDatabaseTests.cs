using BookingsApi.DAL.Queries;

namespace BookingsApi.IntegrationTests.Database.Queries
{
    public class GetAllCaseTypesQueryHandlerDatabaseTests : DatabaseTestsBase
    {
        private GetAllCaseTypesQueryHandler _handler;
        private int FinancialRemedyCaseTypeId => 2;
        private DateTime? _originalCaseTypeExpirationDate;
        
        private int CivilMoneyClaimsCaseTypeId => 1;
        private int HearingTypeId => 299; // Application Hearings
        private DateTime? _originalHearingTypeExpirationDate;

        [SetUp]
        public void Setup()
        {
            var context = new BookingsDbContext(BookingsDbContextOptions);
            _handler = new GetAllCaseTypesQueryHandler(context);
            InitGetOriginalExpirationDate();
        }

        protected override async Task Cleanup()
        {
            await UpdateCaseTypeExpirationDate(_originalCaseTypeExpirationDate);
            await UpdateHearingTypeExpirationDate(_originalHearingTypeExpirationDate, CivilMoneyClaimsCaseTypeId, HearingTypeId);
        }

        [Test]
        public async Task Should_return_all_case_types_and_their_hearing_types()
        {
            var caseTypes = await _handler.Handle(new GetAllCaseTypesQuery(hideExpired:false));
            var financialRemedy = caseTypes.First(c => c.Name == "Financial Remedy");
            financialRemedy.Should().NotBeNull();
            financialRemedy.HearingTypes.Count.Should().BePositive();
        }
        
        [Test]
        public async Task Should_return_all_case_types_and_their_hearing_types_except_expired()
        {
            // arrange
            var expiredDate = DateTime.Today.AddDays(-1);
            await UpdateCaseTypeExpirationDate(expiredDate);
            await UpdateHearingTypeExpirationDate(expiredDate, CivilMoneyClaimsCaseTypeId, HearingTypeId);

            // act
            var caseTypes = await _handler.Handle(new GetAllCaseTypesQuery(hideExpired:true));

            // assert
            caseTypes.Should().NotContain(c => c.Id == FinancialRemedyCaseTypeId);
            var civilMoneyClaimsCaseType = caseTypes.First(x=> x.Id == CivilMoneyClaimsCaseTypeId);
            civilMoneyClaimsCaseType.HearingTypes.Should().NotContain(ht => ht.Id == HearingTypeId);
        }
        
        private void InitGetOriginalExpirationDate()
        {
            using var db = new BookingsDbContext(BookingsDbContextOptions);
            var caseTypeFr = db.CaseTypes.Find(FinancialRemedyCaseTypeId);
            _originalCaseTypeExpirationDate = caseTypeFr!.ExpirationDate;
            
            var caseTypeCmc = db.CaseTypes.Include(x=> x.HearingTypes).First(c=> c.Id == CivilMoneyClaimsCaseTypeId);
            _originalHearingTypeExpirationDate = caseTypeCmc!.HearingTypes.First(x=>x.Id == HearingTypeId).ExpirationDate;
        }
        
        private async Task UpdateCaseTypeExpirationDate(DateTime? expirationDate)
        {
            await using var db = new BookingsDbContext(BookingsDbContextOptions);
            var caseType = await db.CaseTypes.FindAsync(FinancialRemedyCaseTypeId);
            caseType!.ExpirationDate = expirationDate;
            await db.SaveChangesAsync();
        }
        
        private async Task UpdateHearingTypeExpirationDate(DateTime? expirationDate, int caseTypeId, int hearingTypeId)
        {
            await using var db = new BookingsDbContext(BookingsDbContextOptions);
            
            var caseType = db.CaseTypes.Include(x=> x.HearingTypes).First(c=> c.Id == caseTypeId);
            caseType!.HearingTypes.First(x=> x.Id == hearingTypeId).ExpirationDate = expirationDate;
            await db.SaveChangesAsync();
        }
    }
}