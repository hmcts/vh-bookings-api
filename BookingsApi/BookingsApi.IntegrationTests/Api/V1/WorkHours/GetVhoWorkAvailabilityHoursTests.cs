using BookingsApi.Contract.V1.Responses;

namespace BookingsApi.IntegrationTests.Api.V1.WorkHours
{
    public class GetVhoWorkAvailabilityHoursTests : ApiTest
    {
        [Test]
        public async Task should_return_bad_request_when_username_is_invalid()
        {
            // arrange
            var username = "invalid-username";
            
            // act
            using var client = Application.CreateClient();
            var result = await client
                .GetAsync(ApiUriFactory.WorkHoursEndpoints.GetVhoWorkAvailabilityHours(username));

            // assert
            result.IsSuccessStatusCode.Should().BeFalse();
            result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            var validationProblemDetails = await ApiClientResponse.GetResponses<ValidationProblemDetails>(result.Content);
            validationProblemDetails.Errors.SelectMany(x => x.Value).Should()
                .Contain($"Please provide a valid {nameof(username)}");
        }

        [Test]
        public async Task should_return_not_found_and_400_when_justice_user_does_not_exist()
        {
            // arrange
            var username = "madeup@notreal.com";
            
            // act
            using var client = Application.CreateClient();
            var result = await client
                .GetAsync(ApiUriFactory.WorkHoursEndpoints.GetVhoWorkAvailabilityHours(username));

            // assert
            result.IsSuccessStatusCode.Should().BeFalse();
            result.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Test]
        public async Task should_return_okay_and_work_hours()
        {
            // arrange
            var justiceUser = await Hooks.SeedJusticeUser($"{Guid.NewGuid():N}@test.com", "Saving", "Work Hours",
                initWorkHours: true);
            var expectedWorkHours = justiceUser.VhoWorkHours.ToList();

            var client = GetBookingsApiClient();
            
            // act
            var result = await client.GetVhoWorkAvailabilityHoursAsync(justiceUser.Username);
            
            
            // assert
            result.Should().NotBeEmpty();
            var workHours = result.ToList();
            workHours.Count.Should().Be(justiceUser.VhoWorkHours.Count);
            // match the work hours with the justice user work hours
            foreach (var workHour in workHours)
            {
                var dayOfWWeekHours = expectedWorkHours.Single(x => x.DayOfWeekId == workHour.DayOfWeekId);
                workHour.StartTime.Should().Be(dayOfWWeekHours.StartTime);
                workHour.EndTime.Should().Be(dayOfWWeekHours.EndTime);
            }
        }
    }
}
