namespace BookingsApi.IntegrationTests.Api.V1.WorkHours
{
    public class GetVhoNonAvailabilityHoursTests : ApiTest
    {
        [Test]
        public async Task should_return_bad_request_when_username_is_invalid()
        {
            // arrange
            var username = "invalid-username";
            
            // act
            using var client = Application.CreateClient();
            var result = await client
                .GetAsync(ApiUriFactory.WorkHoursEndpoints.GetVhoNonAvailabilityHours(username));

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
                .GetAsync(ApiUriFactory.WorkHoursEndpoints.GetVhoNonAvailabilityHours(username));

            // assert
            result.IsSuccessStatusCode.Should().BeFalse();
            result.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Test]
        public async Task should_return_okay_and_non_availability_hours()
        {
            // arrange
            var justiceUser = await Hooks.SeedJusticeUser($"{Guid.NewGuid():N}@test.com", "Saving", "Work Hours",
                initWorkHours:false, initNonAvailabilities:true);
            var expectedNonAvailabilities = justiceUser.VhoNonAvailability.ToList();
            
            
            var client = GetBookingsApiClient();
            
            // act
            var result = await client.GetVhoNonAvailabilityHoursAsync(justiceUser.Username);
            
            // assert
            result.Should().NotBeEmpty();
            var nonAvailabilities = result.ToList();
            nonAvailabilities.Count.Should().Be(justiceUser.VhoNonAvailability.Count);
            // match the non-availability hours with the justice user non availability hours

            foreach (var nonAvailability in nonAvailabilities)
            {
                var expected = expectedNonAvailabilities.Find(na => na.Id == nonAvailability.Id);
                expected.Should().NotBeNull();
                expected.StartTime.Should().Be(nonAvailability.StartTime);
                expected.EndTime.Should().Be(nonAvailability.EndTime);
            }
        }
    }
}
