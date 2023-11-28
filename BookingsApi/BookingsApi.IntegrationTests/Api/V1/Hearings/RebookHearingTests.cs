using BookingsApi.Domain.Enumerations;

namespace BookingsApi.IntegrationTests.Api.V1.Hearings
{
    public class RebookHearingTests: ApiTest
    {
        [Test]
        public async Task should_return_bad_request_when_hearing_status_is_not_failed()
        {
            // arrange
            var hearing = await Hooks.SeedVideoHearing(status: BookingStatus.Created);
            
            // act
            using var client = Application.CreateClient();
            var result = await client
                .PostAsync(ApiUriFactory.HearingsEndpoints.RebookHearing(hearing.Id), null);
            
            // assert
            result.IsSuccessStatusCode.Should().BeFalse();
            result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            var validationProblemDetails = await ApiClientResponse.GetResponses<ValidationProblemDetails>(result.Content);
            validationProblemDetails.Errors.SelectMany(x => x.Value).Should()
                .Contain($"Hearing must have a status of {nameof(BookingStatus.Failed)}");
        }
    }
}
