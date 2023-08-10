using BookingsApi.Domain.Enumerations;

namespace BookingsApi.IntegrationTests.Api.V1.JusticeUsers
{
    public class DeleteJusticeUserTests : ApiTest
    {
        private Guid? _justiceUserId;

        [Test]
        public async Task Should_delete_justice_user()
        {
            // Arrange
            using var client = Application.CreateClient();
            var justiceUserToDelete = await SeedJusticeUser("api_test_delete_justice_user@test.com");

            // Act
            var result = await client.DeleteAsync(
                ApiUriFactory.JusticeUserEndpoints.DeleteJusticeUser(justiceUserToDelete.Id));
            
            // Assert
            result.IsSuccessStatusCode.Should().BeTrue();
            result.StatusCode.Should().Be(HttpStatusCode.NoContent);

            await using var db = new BookingsDbContext(BookingsDbContextOptions);
            var justiceUser = db.JusticeUsers.FirstOrDefault(x => x.Id == justiceUserToDelete.Id);
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
        
        [TearDown]
        public new async Task TearDown()
        {
            await using var db = new BookingsDbContext(BookingsDbContextOptions);

            var justiceUserRoles = db.JusticeUserRoles
                .IgnoreQueryFilters()
                .Where(x => x.JusticeUser.Id == _justiceUserId);
            if(justiceUserRoles.Any())
                db.RemoveRange(justiceUserRoles);
                
            var justiceUser = db.JusticeUsers.IgnoreQueryFilters().FirstOrDefault(x => x.Id == _justiceUserId);
            if (justiceUser != null)
            {
                db.Remove(justiceUser);
                await db.SaveChangesAsync();
            }
            
            _justiceUserId = null;
        }

        private async Task<JusticeUser> SeedJusticeUser(string username)
        {
            await using var db = new BookingsDbContext(BookingsDbContextOptions);
  
            var justiceUser = db.JusticeUsers.Add(new JusticeUser
            {
                ContactEmail = username,
                Username = username,
                CreatedBy = "deletejusticeuser.test@test.com",
                CreatedDate = DateTime.UtcNow,
                FirstName = "ApiTest",
                Lastname = "User",
            });
            var userRole = db.UserRoles.First(e => e.Id == (int)UserRoleId.Vho);
            db.JusticeUserRoles.Add(new JusticeUserRole(justiceUser.Entity, userRole));
            
            for (var i = 1; i <= 7; i++)
            {
                justiceUser.Entity.VhoWorkHours.Add(new VhoWorkHours
                {
                    DayOfWeekId = i, 
                    StartTime = new TimeSpan(8, 0, 0), 
                    EndTime = new TimeSpan(17, 0, 0)
                });
            }
            
            await db.SaveChangesAsync();

            _justiceUserId = justiceUser.Entity.Id;
            
            return justiceUser.Entity;
        }
    }
}
