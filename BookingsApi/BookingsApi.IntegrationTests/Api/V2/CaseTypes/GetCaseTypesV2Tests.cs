using BookingsApi.Client;
using BookingsApi.Domain.RefData;

namespace BookingsApi.IntegrationTests.Api.V2.CaseTypes;

public class GetCaseTypesV2Tests : ApiTest
{
    [Test]
    public async Task should_get_all_case_types_not_deleted()
    {
        // arrange
        var caseType = new CaseType(3, "Generic")
        {
            ServiceId = "ZZY1",
            IsAudioRecordingAllowed = true
        };
        using var client = Application.CreateClient();
        var bookingsApiClient = BookingsApiClient.GetClient(client);

        // act
        var caseRoleResponses = await bookingsApiClient.GetCaseTypesV2Async(includeDeleted: false);

        // assert
        caseRoleResponses.Should().NotBeEmpty();

        var expectedCaseType = caseRoleResponses.FirstOrDefault(x => x.ServiceId == caseType.ServiceId);
        expectedCaseType.Should().NotBeNull();
        expectedCaseType.Id.Should().Be(caseType.Id);
        expectedCaseType.IsAudioRecordingAllowed.Should().Be(caseType.IsAudioRecordingAllowed);
    }
}