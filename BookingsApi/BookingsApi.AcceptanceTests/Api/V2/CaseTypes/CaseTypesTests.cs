using System.Threading.Tasks;
using FluentAssertions;

namespace BookingsApi.AcceptanceTests.Api.V2.CaseTypes;

public class CaseTypesTests : ApiTest
{
    [Test]
    public async Task should_get_all_case_types()
    {
        // arrange/ act
        var allCaseTypes = await BookingsApiClient.GetCaseTypesV2Async(includeDeleted:false);

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
}