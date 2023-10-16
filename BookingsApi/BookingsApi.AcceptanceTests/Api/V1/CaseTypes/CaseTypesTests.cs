using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;

namespace BookingsApi.AcceptanceTests.Api.V1.CaseTypes;

public class CaseTypesTests : ApiTest
{
    [Test]
    public async Task should_get_all_case_types()
    {
        // arrange/ act
        var allCaseTypes = await BookingsApiClient.GetCaseTypesAsync(includeDeleted:false);

        // assert
        allCaseTypes.Should().NotBeNullOrEmpty();
        allCaseTypes.Should().Contain(x => x.Name == "Generic");
        allCaseTypes.Should().AllSatisfy(caseTypeResponse =>
        {
            caseTypeResponse.Id.Should().NotBe(0);
            caseTypeResponse.Name.Should().NotBeNullOrWhiteSpace();
        });
        
        // consider adding an assertion for one service id of a known type?
    }

    [Test]
    public async Task should_get_case_roles_for_case_type()
    {
        // arrange
        const string caseType = "Generic";

        // act
        var caseRoles = await BookingsApiClient.GetCaseRolesForCaseTypeAsync(caseType);

        // assert
        caseRoles.Should().NotBeEmpty();
        caseRoles.Should().Contain(x => x.Name == "Applicant");
    }

    [Test]
    public async Task should_get_hearing_roles_for_case_roles()
    {
        // arrange
        const string caseType = "Generic";
        const string caseRole = "Applicant";

        // act
        var hearingRoles = await BookingsApiClient.GetHearingRolesForCaseRoleAsync(caseType, caseRole);

        // assert
        hearingRoles.Should().NotBeEmpty();
        hearingRoles.Should().Contain(x => x.Name == "Litigant in person");
    }
}