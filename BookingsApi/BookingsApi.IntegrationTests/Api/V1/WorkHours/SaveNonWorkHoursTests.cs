using System.Text.RegularExpressions;
using BookingsApi.Contract.V1.Requests;

namespace BookingsApi.IntegrationTests.Api.V1.WorkHours;

public class SaveNonWorkHoursTests : ApiTest
{
    [Test]
    public async Task should_return_400_and_validation_problems_when_incorrect_payload_is_given()
    {
        var username = "dupe@test.com";
        var request = new List<UploadNonWorkingHoursRequest>
        {
            new(username, new DateTime(2022, 2, 1, 0, 0, 0, DateTimeKind.Utc),
                new DateTime(2022, 1, 10, 0, 0, 0, DateTimeKind.Utc))
        };
        
        // act
        using var client = Application.CreateClient();
        var result = await client.PostAsync(ApiUriFactory.WorkHoursEndpoints.SaveNonWorkingHours, RequestBody.Set(request));

        // assert
        result.IsSuccessStatusCode.Should().BeFalse();
        result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var validationProblemDetails = await ApiClientResponse.GetResponses<ValidationProblemDetails>(result.Content);
        
        var regex = new Regex(@"End time \d{2}/\d{2}/\d{4} \d{2}:\d{2}:\d{2} is before start time \d{2}/\d{2}/\d{4} \d{2}:\d{2}:\d{2}\.");
        validationProblemDetails.Errors[username][0].Should()
            .MatchRegex(regex);
    }

    [Test]
    public async Task should_return_okay_and_save_non_working_hours()
    {
        // arrange
        var justiceUser = await Hooks.SeedJusticeUser($"{Guid.NewGuid():N}@test.com", "Saving", "Work Hours",
            initWorkHours: false);
        
        var request = new List<UploadNonWorkingHoursRequest>
        {
            new(justiceUser.Username, new DateTime(2022, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                new DateTime(2022, 2, 10, 0, 0, 0, DateTimeKind.Utc))
        };
        
        // act
        using var client = Application.CreateClient();
        var result = await client.PostAsync(ApiUriFactory.WorkHoursEndpoints.SaveNonWorkingHours, RequestBody.Set(request));

        // assert
        result.IsSuccessStatusCode.Should().BeTrue();
        result.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}