using BookingsApi.Contract.V2.Responses;
using BookingsApi.DAL.Helper;

namespace BookingsApi.IntegrationTests.Api.V1.HearingRoles;

public class GetHearingRolesTests : ApiTest
{
    [Test]
    public async Task should_get_all_standard_hearing_roles()
    {
        // arrange
        using var client = Application.CreateClient();

        // act
        var result = await client.GetAsync(ApiUriFactory.HearingRolesEndpoints.GetHearingRoles());

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