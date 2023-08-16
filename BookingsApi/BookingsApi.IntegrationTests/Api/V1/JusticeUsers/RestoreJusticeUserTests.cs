using BookingsApi.Contract.V1.Requests;
using BookingsApi.Validations.V1;
using FizzWare.NBuilder;

namespace BookingsApi.IntegrationTests.Api.V1.JusticeUsers
{
    public class RestoreJusticeUserTests : ApiTest
    {
        private RestoreJusticeUserRequest _request;

        [Test]
        public async Task Should_restore_justice_user()
        {
            // Arrange
            using var client = Application.CreateClient();
            var justiceUserToRestore = await SeedDeletedJusticeUser("api_test_edit_justice_user@test.com");
            _request = BuildValidRestoreJusticeUserRequest(justiceUserToRestore);

            // Act
            var result = await client.PatchAsync(
                ApiUriFactory.JusticeUserEndpoints.RestoreJusticeUser, RequestBody.Set(_request));
            
            // Assert
            result.IsSuccessStatusCode.Should().BeTrue();
            result.StatusCode.Should().Be(HttpStatusCode.NoContent);

            await using var db = new BookingsDbContext(BookingsDbContextOptions);
            var justiceUser = db.JusticeUsers.FirstOrDefault(x => x.Id == justiceUserToRestore.Id);
            justiceUser.Should().NotBeNull();
            justiceUser!.Deleted.Should().BeFalse();
        }

        [Test]
        public async Task Should_return_not_found_when_justice_user_does_not_exist()
        {
            // Arrange
            using var client = Application.CreateClient();
            _request = new RestoreJusticeUserRequest()
            {
                Id = Guid.NewGuid(),
                Username = "random@test.com"
            };

            // Act
            var result = await client.PatchAsync(
                ApiUriFactory.JusticeUserEndpoints.RestoreJusticeUser, RequestBody.Set(_request));
            
            // Assert
            result.IsSuccessStatusCode.Should().BeFalse();
            result.StatusCode.Should().Be(HttpStatusCode.NotFound);

            var responseBody = await ApiClientResponse.GetResponses<string>(result.Content);
            responseBody.Should().Be($"Justice user with id {_request.Id} not found");
        }

        [Test]
        public async Task Should_return_bad_request_when_invalid_payload_provided()
        {
            // Arrange
            using var client = Application.CreateClient();
            _request = new RestoreJusticeUserRequest()
            {
                Id = Guid.Empty,
                Username = string.Empty
            };
            
            // Act
            var result = await client.PatchAsync(
                ApiUriFactory.JusticeUserEndpoints.RestoreJusticeUser, RequestBody.Set(_request));
            
            // Assert
            result.IsSuccessStatusCode.Should().BeFalse();
            result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            var validationProblemDetails = await ApiClientResponse.GetResponses<ValidationProblemDetails>(result.Content);
            validationProblemDetails.Errors[nameof(_request.Id)][0].Should()
                .Be(RestoreJusticeUserRequestValidation.NoIdErrorMessage);
            validationProblemDetails.Errors[nameof(_request.Username)][0].Should()
                .Be(RestoreJusticeUserRequestValidation.NoUsernameErrorMessage);
        }
        
        private async Task<JusticeUser> SeedDeletedJusticeUser(string username)
        {
            var justiceUser = await Hooks.SeedJusticeUser(username, "ApiTest", "User", initWorkHours: true);
            await using var db = new BookingsDbContext(BookingsDbContextOptions);

            db.Attach(justiceUser);
            justiceUser.Delete();
            await db.SaveChangesAsync();
            
            return justiceUser;
        }
        
        private static RestoreJusticeUserRequest BuildValidRestoreJusticeUserRequest(JusticeUser justiceUser)
        {
            return Builder<RestoreJusticeUserRequest>.CreateNew()
                .With(x=> x.Username, justiceUser.Username)
                .With(x=> x.Id, justiceUser.Id)
                .Build();
        }
    }
}
