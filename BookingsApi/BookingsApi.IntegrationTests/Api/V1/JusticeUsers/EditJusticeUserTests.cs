using BookingsApi.Contract.V1.Requests;
using BookingsApi.Contract.V1.Responses;
using BookingsApi.Validations.V1;
using FizzWare.NBuilder;
using Microsoft.EntityFrameworkCore;
using JusticeUserRole = BookingsApi.Contract.V1.Requests.Enums.JusticeUserRole;

namespace BookingsApi.IntegrationTests.Api.V1.JusticeUsers
{
    public class EditJusticeUserTests : ApiTest
    {
        private EditJusticeUserRequest _request;

        [Test]
        public async Task should_edit_justice_user()
        {
            // arrange
            using var client = Application.CreateClient();
            var justiceUserToEdit = await Hooks.SeedJusticeUser("api_test_edit_justice_user@test.com", "ApiTest",
                "User", initWorkHours: true);
            _request = BuildValidEditJusticeUserRequest(justiceUserToEdit);
            
            // act
            var result = await client.PatchAsync(
                ApiUriFactory.JusticeUserEndpoints.EditJusticeUser, RequestBody.Set(_request));
            
            // assert
            result.IsSuccessStatusCode.Should().BeTrue();
            result.StatusCode.Should().Be(HttpStatusCode.OK);
            
            var response = await ApiClientResponse.GetResponses<JusticeUserResponse>(result.Content);
            
            await using var db = new BookingsDbContext(BookingsDbContextOptions);
            var justiceUser = db.JusticeUsers
                .Include(ju => ju.JusticeUserRoles).ThenInclude(jur => jur.UserRole)
                .First(x => x.Username == _request.Username);
            
            justiceUser.Should().NotBeNull();
            justiceUser.JusticeUserRoles.Should().Contain(jur => jur.UserRole.IsVhTeamLead);
            justiceUser.Id.Should().Be(response.Id);
        }

        [Test]
        public async Task should_return_bad_request_when_an_invalid_payload_to_edit_a_justice_user_is_sent()
        {
            // arrange
            _request = Builder<EditJusticeUserRequest>.CreateNew()
                .With(x => x.Username, null)
                .Build();
            
            using var client = Application.CreateClient();
            
            // act
            var result = await client.PatchAsync(
                ApiUriFactory.JusticeUserEndpoints.EditJusticeUser, RequestBody.Set(_request));
            
            // assert
            result.IsSuccessStatusCode.Should().BeFalse();
            
            result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            var validationProblemDetails = await ApiClientResponse.GetResponses<ValidationProblemDetails>(result.Content);
            validationProblemDetails.Errors[nameof(_request.Username)][0].Should()
                .Be(EditJusticeUserRequestValidation.NoUsernameErrorMessage);
        }
        
        [Test]
        public async Task should_return_not_found_when_a_justice_user_with_requested_id_not_found()
        {
            // arrange
            _request = Builder<EditJusticeUserRequest>.CreateNew().Build();
            
            using var client = Application.CreateClient();
            
            // act
            var result = await client.PatchAsync(
                ApiUriFactory.JusticeUserEndpoints.EditJusticeUser, RequestBody.Set(_request));
            
            // assert
            result.IsSuccessStatusCode.Should().BeFalse();
            
            result.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }
        
        private static EditJusticeUserRequest BuildValidEditJusticeUserRequest(JusticeUser justiceUser)
        {
            return Builder<EditJusticeUserRequest>.CreateNew()
                .With(x=> x.Username, justiceUser.Username)
                .With(x=> x.Id, justiceUser.Id)
                .With(x => x.Roles = new List<JusticeUserRole>() { JusticeUserRole.VhTeamLead })
                .Build();
        }
    }
}