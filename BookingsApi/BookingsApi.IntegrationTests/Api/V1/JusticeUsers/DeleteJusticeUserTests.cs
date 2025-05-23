using BookingsApi.Domain.Enumerations;

namespace BookingsApi.IntegrationTests.Api.V1.JusticeUsers
{
    public class DeleteJusticeUserTests : ApiTest
    {
        [Test]
        public async Task Should_delete_justice_user()
        {
            // Arrange
            using var client = Application.CreateClient();
            var justiceUserToDelete = await Hooks.SeedJusticeUser("api_test_delete_justice_user@test.com", "ApiTest",
                "User", isTeamLead: false, initWorkHours: true);

            // Act
            var result = await client.DeleteAsync(
                ApiUriFactory.JusticeUserEndpoints.DeleteJusticeUser(justiceUserToDelete.Id));
            
            // Assert
            result.IsSuccessStatusCode.Should().BeTrue();
            result.StatusCode.Should().Be(HttpStatusCode.NoContent);

            await using var db = new BookingsDbContext(BookingsDbContextOptions);
            var justiceUser = await db.JusticeUsers.FirstOrDefaultAsync(x => x.Id == justiceUserToDelete.Id);
            justiceUser.Should().BeNull();
        }

        [Test]
        public async Task Should_return_not_found_when_justice_user_does_not_exist()
        {
            // Arrange
            using var client = Application.CreateClient();
            var id = Guid.NewGuid();

            // Act
            var result = await client.DeleteAsync(
                ApiUriFactory.JusticeUserEndpoints.DeleteJusticeUser(id));
            
            // Assert
            result.IsSuccessStatusCode.Should().BeFalse();
            result.StatusCode.Should().Be(HttpStatusCode.NotFound);

            var responseBody = await ApiClientResponse.GetResponses<string>(result.Content);
            responseBody.Should().Be($"Justice user with id {id} not found");
        }

        [Test]
        public async Task Should_return_bad_request_when_invalid_id_specified()
        {
            // Arrange
            using var client = Application.CreateClient();
            var id = Guid.Empty;
            
            // Act
            var result = await client.DeleteAsync(
                ApiUriFactory.JusticeUserEndpoints.DeleteJusticeUser(id));
            
            // Assert
            result.IsSuccessStatusCode.Should().BeFalse();
            result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            var responseBody = await ApiClientResponse.GetResponses<ValidationProblemDetails>(result.Content);
            var errors = responseBody.Errors["id"];
            errors[0].Should().Be($"Please provide a valid {nameof(id)}");
        }
    }
}
