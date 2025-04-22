using BookingsApi.Contract.V1.Requests;
using BookingsApi.Validations.V1;

namespace BookingsApi.IntegrationTests.Api.V1.WorkHours;

public class UpdateVhoNonAvailabilityHoursTests : ApiTest
{
    [Test]
    public async Task should_return_204_and_update_non_working_hours()
    {
        // arrange
        var justiceUser = await Hooks.SeedJusticeUser($"{Guid.NewGuid():N}@test.com", "Saving", "Work Hours",
            initWorkHours: false);

        var request = new UpdateNonWorkingHoursRequest()
        {
            Hours = new List<NonWorkingHours>()
            {
                new NonWorkingHours()
                {
                    StartTime = new DateTime(2099, 7, 1, 9, 0, 0, DateTimeKind.Utc),
                    EndTime = new DateTime(2099, 7, 1, 10, 0, 0, DateTimeKind.Utc)
                }
            }
        };

        // act
        using var client = Application.CreateClient();
        var result =
            await client.PatchAsync(ApiUriFactory.WorkHoursEndpoints.UpdateVhoNonAvailabilityHours(justiceUser.Username),
                RequestBody.Set(request));

        // assert
        result.IsSuccessStatusCode.Should().BeTrue(result.Content.ReadAsStringAsync().Result);
        result.StatusCode.Should().Be(HttpStatusCode.NoContent);

        await using var db = new BookingsDbContext(BookingsDbContextOptions);
        var updatedJusticeUser = await db.JusticeUsers.Include(x => x.VhoNonAvailability)
            .FirstAsync(x => x.Username == justiceUser.Username);
        updatedJusticeUser.VhoNonAvailability.Should().Contain(x =>
            x.StartTime == request.Hours[0].StartTime && x.EndTime == request.Hours[0].EndTime);
    }

    [Test]
    public async Task should_return_bad_request_when_hours_are_empty()
    {
        // arrange
        var justiceUser = await Hooks.SeedJusticeUser($"{Guid.NewGuid():N}@test.com", "Saving", "Work Hours",
            initWorkHours: false);

        var request = new UpdateNonWorkingHoursRequest
        {
            Hours = new List<NonWorkingHours>()
        };

        // act
        using var client = Application.CreateClient();
        var result =
            await client.PatchAsync(ApiUriFactory.WorkHoursEndpoints.UpdateVhoNonAvailabilityHours(justiceUser.Username),
                RequestBody.Set(request));
        
        // assert
        result.IsSuccessStatusCode.Should().BeFalse();
        result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var validationProblemDetails = await ApiClientResponse.GetResponses<ValidationProblemDetails>(result.Content);
        validationProblemDetails.Errors.SelectMany(x => x.Value).Should()
            .Contain(UpdateNonWorkingHoursRequestValidation.HoursEmptyErrorMessage);
    }
    
    [Test]
    public async Task should_return_not_found_when_updated_non_availability_that_does_not_exist()
    {
        // arrange
        var justiceUser = await Hooks.SeedJusticeUser($"{Guid.NewGuid():N}@test.com", "Saving", "Work Hours",
            initWorkHours: false);

        var request = new UpdateNonWorkingHoursRequest()
        {
            Hours = new List<NonWorkingHours>()
            {
                new()
                {
                    Id = 999273,
                    StartTime = new DateTime(2099, 7, 1, 9, 0, 0, DateTimeKind.Utc),
                    EndTime = new DateTime(2099, 7, 1, 10, 0, 0, DateTimeKind.Utc)
                }
            }
        };

        // act
        using var client = Application.CreateClient();
        var result =
            await client.PatchAsync(ApiUriFactory.WorkHoursEndpoints.UpdateVhoNonAvailabilityHours(justiceUser.Username),
                RequestBody.Set(request));
        
        // assert
        result.IsSuccessStatusCode.Should().BeFalse();
        result.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Test]
    public async Task should_return_bad_request_when_adding_a_non_availability_that_overlaps_an_existing()
    {
        // arrange
        var justiceUser = await Hooks.SeedJusticeUser($"{Guid.NewGuid():N}@test.com", "Saving", "Work Hours",
            initNonAvailabilities: true);
        var existiNonAvailability = justiceUser.VhoNonAvailability[0];
        var secondNonAvailability = justiceUser.VhoNonAvailability[1];
        var date = existiNonAvailability.StartTime.Date;

        var request = new UpdateNonWorkingHoursRequest()
        {
            Hours = new List<NonWorkingHours>()
            {
                new ()
                {
                    Id = existiNonAvailability.Id,
                    StartTime = date.AddHours(10),
                    EndTime = secondNonAvailability.StartTime.Date.AddHours(10),
                }
            }
        };

        // act
        using var client = Application.CreateClient();
        var result =
            await client.PatchAsync(ApiUriFactory.WorkHoursEndpoints.UpdateVhoNonAvailabilityHours(justiceUser.Username),
                RequestBody.Set(request));

        // assert
        result.IsSuccessStatusCode.Should().BeFalse();
        result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var validationProblemDetails = await ApiClientResponse.GetResponses<ValidationProblemDetails>(result.Content);
        validationProblemDetails.Errors.SelectMany(x => x.Value).Should()
            .Contain(UpdateNonWorkingHoursRequestValidation.HoursOverlapErrorMessage);
    }

    [Test]
    public async Task should_return_not_found_when_justice_user_does_not_exist()
    {
        // arrange
        var username = "does@notexist.com";
        
        var request = new UpdateNonWorkingHoursRequest()
        {
            Hours = new List<NonWorkingHours>()
            {
                new NonWorkingHours()
                {
                    StartTime = new DateTime(2099, 7, 1, 9, 0, 0, DateTimeKind.Utc),
                    EndTime = new DateTime(2099, 7, 1, 10, 0, 0, DateTimeKind.Utc)
                }
            }
        };
        
        // act
        using var client = Application.CreateClient();
        var result =
            await client.PatchAsync(ApiUriFactory.WorkHoursEndpoints.UpdateVhoNonAvailabilityHours(username),
                RequestBody.Set(request));

        // assert
        result.IsSuccessStatusCode.Should().BeFalse(result.Content.ReadAsStringAsync().Result);
        result.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}