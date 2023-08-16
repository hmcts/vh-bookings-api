namespace BookingsApi.IntegrationTests.Api.V1.WorkHours;

public class DeleteVhoNonAvailabilityHoursTests : ApiTest
{
    [Test]
    public async Task should_return_bad_request_when_an_invalid_input_is_given()
    {
        // arrange
        var id = 0;
        var username = "tofail";
        

        // act
        using var client = Application.CreateClient();
        var result = await client.DeleteAsync(ApiUriFactory.WorkHoursEndpoints.DeleteVhoNonAvailabilityHours(username, id));

        // assert
        result.IsSuccessStatusCode.Should().BeFalse();
        result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        
        var validationProblemDetails = await ApiClientResponse.GetResponses<ValidationProblemDetails>(result.Content);
        validationProblemDetails.Errors["username"][0].Should()
            .Be("Please provide a valid username");
        validationProblemDetails.Errors["nonAvailabilityId"][0].Should()
            .Be($"Please provide a valid nonAvailabilityId");
    }

    [Test]
    public async Task should_return_not_found_when_justice_user_does_not_exist()
    {
        // arrange
        var id = 1;
        var username = "doesnotexist@test.com";
        

        // act
        using var client = Application.CreateClient();
        var result = await client.DeleteAsync(ApiUriFactory.WorkHoursEndpoints.DeleteVhoNonAvailabilityHours(username, id));

        // assert
        result.IsSuccessStatusCode.Should().BeFalse();
        result.StatusCode.Should().Be(HttpStatusCode.NotFound);
        
        var response = await ApiClientResponse.GetResponses<string>(result.Content);
        response.Should().Be($"Justice user with username {username} not found");
    }

    [Test]
    public async Task should_return_not_found_when_nonavailability_id_does_not_exist()
    {
        // arrange
        var username = "test@deletenonavailability.com";
        await Hooks.SeedJusticeUser(username, "Delete", "NonAvailability", initWorkHours: false);
        var id = 9999999;
        
        // act
        using var client = Application.CreateClient();
        var result = await client.DeleteAsync(ApiUriFactory.WorkHoursEndpoints.DeleteVhoNonAvailabilityHours(username, id));
        
        // assert
        result.IsSuccessStatusCode.Should().BeFalse();
        result.StatusCode.Should().Be(HttpStatusCode.NotFound);
        
        var response = await ApiClientResponse.GetResponses<string>(result.Content);
        response.Should().Be($"Non working hour {id} does not exist");
    }

    [Test]
    public async Task should_remove_non_available_slot()
    {
        var username = "test@deletenonavailability.com";
        var justiceUser = await Hooks.SeedJusticeUser(username, "Delete", "NonAvailability", initWorkHours: false);
        
        await using var db = new BookingsDbContext(BookingsDbContextOptions);
        db.Attach(justiceUser);
        justiceUser.AddOrUpdateNonAvailability(new DateTime(2099, 7, 1), new DateTime(2099, 7, 2));
        await db.SaveChangesAsync();
        db.ChangeTracker.Clear();
        
        var nonAvailability = justiceUser.VhoNonAvailability.First();
        
        // act
        using var client = Application.CreateClient();
        var result = await client.DeleteAsync(ApiUriFactory.WorkHoursEndpoints.DeleteVhoNonAvailabilityHours(username, nonAvailability.Id));
        
        // assert
        result.IsSuccessStatusCode.Should().BeTrue();
        result.StatusCode.Should().Be(HttpStatusCode.OK);
        
        var updatedUser = await db.JusticeUsers.Include(x => x.VhoNonAvailability).FirstAsync(x => x.Username == justiceUser.Username);
        updatedUser.VhoNonAvailability.Where(x=> !x.Deleted).Should().BeEmpty();
    }
}