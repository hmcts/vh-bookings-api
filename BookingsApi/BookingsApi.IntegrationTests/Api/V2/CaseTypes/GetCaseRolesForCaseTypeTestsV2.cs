using BookingsApi.Contract.V2.Responses;
using BookingsApi.DAL.Helper;

namespace BookingsApi.IntegrationTests.Api.V2.CaseTypes;

public class GetHearingRolesTestsV2 : ApiTest
{
    [Test]
    public async Task should_get_all_standard_hearing_roles()
    {
        // arrange
        using var client = Application.CreateClient();

        // act
        var result = await client.GetAsync(ApiUriFactory.HearingRolesEndpointsV2.GetHearingRoles());

        // assert
        result.StatusCode.Should().Be(HttpStatusCode.OK);
        var hearingRoleResponses = await ApiClientResponse.GetResponses<List<HearingRoleResponseV2>>(result.Content);
        hearingRoleResponses.Should().NotBeEmpty();
        
        var expectedIndividuals = new List<string>()
        {
            "Applicant",
            "Appellant",
            "Claimant",
            "Defendant",
            "Expert",
            "Welfare Representative",
            "Intermediaries",
            "Interpreter",
            "Joint Party",
            "Litigation Friend",
            "Observer",
            "Other Party",
            "Police",
            "Respondent",
            "Support",
            "Witness",
            
        };
        
        var expectedRepresentatives = new List<string>()
        {
            "Barrister",
            "Legal Representative",
        };
        
        var expectedJudicialOfficeHolders = new List<string>()
        {
            "Panel Member",
        };
        
        var expectedJudge = new List<string>()
        {
            "Judge",
        };
        
        var expectedStaffMember = new List<string>()
        {
            "Staff Member"
        };
        
        
        hearingRoleResponses.Where(x=> x.UserRole == UserRoles.Individual).Select(x=> x.Name).Should().BeEquivalentTo(expectedIndividuals);
        hearingRoleResponses.Where(x=> x.UserRole == UserRoles.Representative).Select(x=> x.Name).Should().BeEquivalentTo(expectedRepresentatives);
        hearingRoleResponses.Where(x=> x.UserRole == UserRoles.JudicialOfficeHolder).Select(x=> x.Name).Should().BeEquivalentTo(expectedJudicialOfficeHolders);
        hearingRoleResponses.Where(x=> x.UserRole == UserRoles.Judge).Select(x=> x.Name).Should().BeEquivalentTo(expectedJudge);
        hearingRoleResponses.Where(x=> x.UserRole == UserRoles.StaffMember).Select(x=> x.Name).Should().BeEquivalentTo(expectedStaffMember);
    }
}

public class GetCaseRolesForCaseTypeTestsV2 : ApiTest
{
    [Test]
    public async Task should_get_all_case_roles_for_case_type()
    {
        // arrange
        const string caseType = "vhG1";
        using var client = Application.CreateClient();

        // act
        var result = await client.GetAsync(ApiUriFactory.CaseTypesEndpointsV2.GetCaseRolesForCaseType(caseType));

        // assert
        result.StatusCode.Should().Be(HttpStatusCode.OK);
        var caseRoleResponses = await ApiClientResponse.GetResponses<List<CaseRoleResponseV2>>(result.Content);
        caseRoleResponses.Should().NotBeEmpty();
        caseRoleResponses.Exists(x=> x.Name == "Applicant").Should().BeTrue();
    }

    [Test]
    public async Task should_return_not_found_when_invalid_case_type_is_provided()
    {
        // arrange
        const string caseType = "Made Up Case Service";
        using var client = Application.CreateClient();

        // act
        var result = await client.GetAsync(ApiUriFactory.CaseTypesEndpointsV2.GetCaseRolesForCaseType(caseType));

        // assert
        result.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}