using System.Linq;
using System.Threading.Tasks;
using BookingsApi.DAL;
using BookingsApi.DAL.Queries;
using BookingsApi.Domain.RefData;
using FluentAssertions;
using NUnit.Framework;

namespace BookingsApi.IntegrationTests.Database.Queries
{
    public class GetCaseTypeQueryHandlerDatabaseTests : DatabaseTestsBase
    {
        private GetCaseTypeQueryHandler _handler;

        [SetUp]
        public void Setup()
        {
            var context = new BookingsDbContext(BookingsDbContextOptions);
            _handler = new GetCaseTypeQueryHandler(context);
        }

        [Test]
        public async Task Should_have_user_roles_for_generic_applicant()
        {
            var caseTypeName = "Generic";
            var caseRoleName = "Applicant";
            var caseType = await _handler.Handle(new GetCaseTypeQuery(caseTypeName));
            caseType.Should().NotBeNull();
            AssertUserRolesForCaseRole(caseType, caseRoleName);
        }

        [Test]
        public async Task Should_have_user_roles_for_generic_respondent()
        {
            var caseTypeName = "Generic";
            var caseRoleName = "Respondent";
            var caseType = await _handler.Handle(new GetCaseTypeQuery(caseTypeName));
            caseType.Should().NotBeNull();
            AssertUserRolesForCaseRole(caseType, caseRoleName);
        }

        [Test]
        public async Task Should_have_user_roles_for_financial_remedy_applicant()
        {
            var caseTypeName = "Financial Remedy";
            var caseRoleName = "Applicant";
            var caseType = await _handler.Handle(new GetCaseTypeQuery(caseTypeName));
            caseType.Should().NotBeNull();
            AssertUserRolesForCaseRole(caseType, caseRoleName);
        }

        [Test]
        public async Task Should_have_user_roles_for_financial_remedy_respondent()
        {
            var caseTypeName = "Financial Remedy";
            var caseRoleName = "Respondent";
            var caseType = await _handler.Handle(new GetCaseTypeQuery(caseTypeName));
            caseType.Should().NotBeNull();
            AssertUserRolesForCaseRole(caseType, caseRoleName);
        }

        [Test]
        public async Task Should_have_sorted_user_roles_and_hearing_roles_for_financial_remedy()
        {
            var caseTypeName = "Financial Remedy";
            var caseType = await _handler.Handle(new GetCaseTypeQuery(caseTypeName));
            caseType.Should().NotBeNull();
            AssertCaseRolesAndHearingRolesAreSortedAscendingByName(caseType);
        }

        private void AssertCaseRolesAndHearingRolesAreSortedAscendingByName(CaseType caseType)
        {
            caseType.CaseRoles.Should().BeInAscendingOrder();
            foreach (var caseRole in caseType.CaseRoles)
            {
                caseRole.HearingRoles.Should().BeInAscendingOrder();
            }
        }

        private void AssertUserRolesForCaseRole(CaseType caseType, string caseRoleName)
        {
            var caseRole = caseType.CaseRoles.First(x => x.Name == caseRoleName);
            caseRole.Should().NotBeNull();

            caseRole.HearingRoles.Should().NotBeEmpty();
            foreach (var hearingRole in caseRole.HearingRoles)
            {
                hearingRole.UserRole.Should().NotBeNull();
            }
        }
    }
}