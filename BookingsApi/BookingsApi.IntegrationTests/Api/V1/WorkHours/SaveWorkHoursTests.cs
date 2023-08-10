using BookingsApi.Contract.V1.Requests;
using Microsoft.EntityFrameworkCore;

namespace BookingsApi.IntegrationTests.Api.V1.WorkHours;

public class SaveWorkHoursTests : ApiTest
{
    [Test]
    public async Task should_return_400_and_validation_problems_when_an_incorrect_payload_is_given()
    {
        // arrange
        var username = "dupe@test.com";
        var request = new List<UploadWorkHoursRequest>
        {
            new()
            {
                Username = username,
                WorkingHours = new List<WorkingHours>
                {
                    new(1, null, 1, 1, 1)
                },
            },
            new()
            {
                Username = username,
                WorkingHours = new List<WorkingHours>
                {
                    new(1, 9, 0, 17, 1),
                    new(2, 9, 0, 17, 1),
                    new(3, 9, 0, 17, 1),
                    new(4, 9, 0, 17, 1),
                    new(5, 9, 0, 17, 1),
                    new(6, null, null, null, null),
                    new(7, null, null, null, null),
                },
            }
        };

        // act
        using var client = Application.CreateClient();
        var result = await client.PostAsync(ApiUriFactory.WorkHoursEndpoints.SaveWorkHours, RequestBody.Set(request));

        // assert
        result.IsSuccessStatusCode.Should().BeFalse();
        result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var validationProblemDetails = await ApiClientResponse.GetResponses<ValidationProblemDetails>(result.Content);
        validationProblemDetails.Errors[username][0].Should()
            .Be("Multiple entries for user. Only one row per user required");
        validationProblemDetails.Errors[$"{username}, Day Numbers"][0].Should()
            .Be("Must specify one entry for each day of the week for days 1-7");
    }

    [Test]
    public async Task should_return_200_and_save_work_hours_and_an_empty_failed_upload_list()
    {
        // arrange
        var justiceUser = await Hooks.SeedJusticeUser($"{Guid.NewGuid():N}@test.com", "Saving", "Work Hours",
            initWorkHours: false);
        
        var request = new List<UploadWorkHoursRequest>
        {
            new()
            {
                Username = justiceUser.Username,
                WorkingHours = new List<WorkingHours>
                {
                    new(1, 9, 0, 17, 1),
                    new(2, 9, 0, 17, 1),
                    new(3, 9, 0, 17, 1),
                    new(4, 9, 0, 17, 1),
                    new(5, 9, 0, 17, 1),
                    new(6, null, null, null, null),
                    new(7, null, null, null, null),
                },
            }
        };
        
        // act
        using var client = Application.CreateClient();
        var result = await client.PostAsync(ApiUriFactory.WorkHoursEndpoints.SaveWorkHours, RequestBody.Set(request));

        // assert
        result.IsSuccessStatusCode.Should().BeTrue();
        result.StatusCode.Should().Be(HttpStatusCode.OK);
        var failedUploadResponse = await ApiClientResponse.GetResponses<List<string>>(result.Content);
        failedUploadResponse.Should().BeEmpty();
        
        await using var db = new BookingsDbContext(BookingsDbContextOptions);
        var updatedJusticeUser = await db.JusticeUsers.Include(x => x.VhoWorkHours).FirstAsync(x => x.Username == justiceUser.Username);
        updatedJusticeUser.VhoWorkHours.Should().HaveCount(7);
        for (var dayOfWeekId = 1; dayOfWeekId < 8; dayOfWeekId++)
        {
            updatedJusticeUser.VhoWorkHours.First(x => x.DayOfWeekId == dayOfWeekId).StartTime.Should().Be(request[0].WorkingHours[dayOfWeekId - 1].StartTime);
            updatedJusticeUser.VhoWorkHours.First(x => x.DayOfWeekId == dayOfWeekId).EndTime.Should().Be(request[0].WorkingHours[dayOfWeekId - 1].EndTime);
        }
    }

    [Test]
    public async Task should_return_200_and_a_non_empty_failed_upload_list_when_justice_user_does_not_exist()
    {
        // arrange
        var username = "madeup@tofail.com";
        var request = new List<UploadWorkHoursRequest>
        {
            new()
            {
                Username = username,
                WorkingHours = new List<WorkingHours>
                {
                    new(1, 9, 0, 17, 1),
                    new(2, 9, 0, 17, 1),
                    new(3, 9, 0, 17, 1),
                    new(4, 9, 0, 17, 1),
                    new(5, 9, 0, 17, 1),
                    new(6, null, null, null, null),
                    new(7, null, null, null, null),
                },
            }
        };

        // act
        using var client = Application.CreateClient();
        var result = await client.PostAsync(ApiUriFactory.WorkHoursEndpoints.SaveWorkHours, RequestBody.Set(request));

        // assert
        result.IsSuccessStatusCode.Should().BeTrue();
        result.StatusCode.Should().Be(HttpStatusCode.OK);
        var failedUploadUsernameResponse = await ApiClientResponse.GetResponses<List<string>>(result.Content);
        failedUploadUsernameResponse.Should().Contain(username);
    }
}