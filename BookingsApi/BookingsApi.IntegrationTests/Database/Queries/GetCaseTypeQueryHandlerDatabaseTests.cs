using System.Linq;
using System.Threading.Tasks;
using BookingsApi.Common.Services;
using BookingsApi.DAL;
using BookingsApi.DAL.Queries;
using BookingsApi.Domain.RefData;
using FluentAssertions;
using Moq;
using NUnit.Framework;

namespace BookingsApi.IntegrationTests.Database.Queries
{
    public class GetCaseTypeQueryHandlerDatabaseTests : DatabaseTestsBase
    {
        private GetCaseTypeQueryHandler _handler;
        private Mock<IFeatureToggles> _featureTogglesMock;

        [SetUp]
        public void Setup()
        {
            var context = new BookingsDbContext(BookingsDbContextOptions);
            _featureTogglesMock = new Mock<IFeatureToggles>();
            _featureTogglesMock.Setup(x => x.ReferenceDataToggle()).Returns(false);
            _handler = new GetCaseTypeQueryHandler(context, _featureTogglesMock.Object);
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

        [Test] public async Task should_get_case_type_by_service_id()
        {
            _featureTogglesMock.Setup(x => x.ReferenceDataToggle()).Returns(true);
            var caseTypeName = "Generic";
            var serviceId = "vhG1"; // intentionally not the same case to verify case sensitivity is ignored
            var caseType = await _handler.Handle(new GetCaseTypeQuery(serviceId));
            caseType.Should().NotBeNull();
            caseType.Name.Should().Be(caseTypeName);
            caseType.HearingTypes.Should().NotBeEmpty();
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