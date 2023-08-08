using System;
using System.Linq;
using BookingsApi.Contract.V1.Responses;
using BookingsApi.Domain;
using JusticeUserRole = BookingsApi.Contract.V1.Requests.Enums.JusticeUserRole;

namespace BookingsApi.IntegrationTests.Api.V1.JusticeUsers;

public class GetJusticeUserListTests : ApiTest
{
    [Test]
    public async Task should_return_all_except_deleted()
    {
        // arrange
        var j1 = await Hooks.SeedJusticeUser($"{Guid.NewGuid():N}@test.com", "testfirstname1", "testsurname1");
        var j2 = await Hooks.SeedJusticeUser($"{Guid.NewGuid():N}@test.com", "testfirstname2", "testsurname2");
        var j3 = await Hooks.SeedJusticeUser($"{Guid.NewGuid():N}@test.com", "testfirstname3", "testsurname3");
        var j4 = await Hooks.SeedJusticeUser($"{Guid.NewGuid():N}@test.com", "testfirstname3", "testsurname3", isDeleted:true);
        
        // act
        using var client = Application.CreateClient();
        var result = await client.GetAsync(ApiUriFactory.JusticeUserEndpoints.GetJusticeUserList(string.Empty, false));

        // assert
        result.IsSuccessStatusCode.Should().BeTrue();
        result.StatusCode.Should().Be(HttpStatusCode.OK);
       
        var justiceUserListResponses = await ApiClientResponse.GetResponses<List<JusticeUserResponse>>(result.Content);
        justiceUserListResponses.Should().NotBeEmpty();
        justiceUserListResponses.Should().Contain(response => response.ContactEmail == j1.ContactEmail);
        justiceUserListResponses.Should().Contain(response => response.ContactEmail == j2.ContactEmail);
        justiceUserListResponses.Should().Contain(response => response.ContactEmail == j3.ContactEmail);
        justiceUserListResponses.Should().NotContain(response => response.ContactEmail == j4.ContactEmail);
    }
    
    [Test]
    public async Task should_return_all_including_deleted()
    {
        // arrange
        var j1 = await Hooks.SeedJusticeUser($"{Guid.NewGuid():N}@test.com", "testfirstname1", "testsurname1");
        var j2 = await Hooks.SeedJusticeUser($"{Guid.NewGuid():N}@test.com", "testfirstname2", "testsurname2");
        var j3 = await Hooks.SeedJusticeUser($"{Guid.NewGuid():N}@test.com", "testfirstname3", "testsurname3");
        var j4 = await Hooks.SeedJusticeUser($"{Guid.NewGuid():N}@test.com", "testfirstname3", "testsurname3", isDeleted:true);
        
        // act
        using var client = Application.CreateClient();
        var result = await client.GetAsync(ApiUriFactory.JusticeUserEndpoints.GetJusticeUserList(string.Empty, true));

        // assert
        result.IsSuccessStatusCode.Should().BeTrue();
        result.StatusCode.Should().Be(HttpStatusCode.OK);
       
        var justiceUserListResponses = await ApiClientResponse.GetResponses<List<JusticeUserResponse>>(result.Content);
        justiceUserListResponses.Should().NotBeEmpty();
        justiceUserListResponses.Should().Contain(response => response.ContactEmail == j1.ContactEmail);
        justiceUserListResponses.Should().Contain(response => response.ContactEmail == j2.ContactEmail);
        justiceUserListResponses.Should().Contain(response => response.ContactEmail == j3.ContactEmail);
        justiceUserListResponses.Should().Contain(response => response.ContactEmail == j4.ContactEmail);
    }

    [Test]
    public async Task should_return_matching_search_term()
    {
        // arrange
        var j1 = await Hooks.SeedJusticeUser($"{Guid.NewGuid():N}@test.com", "testfirstname1", "testsurname1");
        var j2 = await Hooks.SeedJusticeUser($"{Guid.NewGuid():N}@test.com", "testfirstname2", "testsurname2");
        var j3 = await Hooks.SeedJusticeUser($"{Guid.NewGuid():N}@test.com", "testfirstname3", "testsurname3");
        var j4 = await Hooks.SeedJusticeUser($"{Guid.NewGuid():N}@test.com", "testfirstname3", "testsurname3", isDeleted:true);
        
        // act
        using var client = Application.CreateClient();
        var result = await client.GetAsync(ApiUriFactory.JusticeUserEndpoints.GetJusticeUserList(j1.ContactEmail, true));

        // assert
        result.IsSuccessStatusCode.Should().BeTrue();
        result.StatusCode.Should().Be(HttpStatusCode.OK);
       
        var justiceUserListResponses = await ApiClientResponse.GetResponses<List<JusticeUserResponse>>(result.Content);
        justiceUserListResponses.Should().NotBeEmpty();
        justiceUserListResponses.Should().Contain(response => response.ContactEmail == j1.ContactEmail);
        justiceUserListResponses.Should().NotContain(response => response.ContactEmail == j2.ContactEmail);
        justiceUserListResponses.Should().NotContain(response => response.ContactEmail == j3.ContactEmail);
        justiceUserListResponses.Should().NotContain(response => response.ContactEmail == j4.ContactEmail);
    }
    
    
}

public class GetJusticeUserByUsernameTests : ApiTest
{
    private JusticeUser _justiceUser;

    [Test]
    public async Task should_return_not_found_when_provided_username_is_not_found()
    {
        // arrange
        var username = "madeupusername@test.com";

        // act
        using var client = Application.CreateClient();
        var result = await client.GetAsync(ApiUriFactory.JusticeUserEndpoints.GetJusticeUserByUsername(username));

        // assert
        result.IsSuccessStatusCode.Should().BeFalse();
        result.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Test]
    public async Task should_return_justice_user_when_username_is_matched()
    {
        // arrange
        _justiceUser = await Hooks.SeedJusticeUser($"{Guid.NewGuid():N}", "testfirstname", "testsurname");
        var username = _justiceUser.Username;

        // act
        using var client = Application.CreateClient();
        var result = await client.GetAsync(ApiUriFactory.JusticeUserEndpoints.GetJusticeUserByUsername(username));

        // assert
        result.IsSuccessStatusCode.Should().BeTrue();
        result.StatusCode.Should().Be(HttpStatusCode.OK);
        var justiceUserResponse = await ApiClientResponse.GetResponses<JusticeUserResponse>(result.Content);
        justiceUserResponse.Id.Should().Be(_justiceUser.Id);
        justiceUserResponse.FirstName.Should().Be(_justiceUser.FirstName);
        justiceUserResponse.Lastname.Should().Be(_justiceUser.Lastname);
        justiceUserResponse.ContactEmail.Should().Be(_justiceUser.ContactEmail);
        justiceUserResponse.Username.Should().Be(_justiceUser.Username);
        justiceUserResponse.Telephone.Should().Be(_justiceUser.Telephone);
        justiceUserResponse.IsVhTeamLeader.Should().Be(_justiceUser.IsTeamLeader());
        justiceUserResponse.CreatedBy.Should().Be(_justiceUser.CreatedBy);
        justiceUserResponse.FullName.Should().Be(_justiceUser.FirstName + " " + _justiceUser.Lastname);
        justiceUserResponse.Deleted.Should().BeFalse();
        justiceUserResponse.UserRoles.Should().Contain(JusticeUserRole.Vho);
    }

    [TearDown]
    public new async Task TearDown()
    {
        if (_justiceUser != null)
        {
            await Hooks.ClearSeededJusticeUsersAsync();
            _justiceUser = null;
        }
    }
}