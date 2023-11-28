namespace BookingsApi.IntegrationTests.Api.V1.HearingParticipants
{
    public class GetParticipantInHearingTests : ApiTest
    {
        [Test]
        public async Task should_return_bad_request_when_hearing_id_is_invalid()
        {
            // arrange
            var hearingId = Guid.Empty;
            var participantId = Guid.NewGuid();
            
            // act
            using var client = Application.CreateClient();
            var result = await client
                .GetAsync(ApiUriFactory.ParticipantsEndpoints.GetParticipantInHearing(hearingId, participantId));


            // assert
            result.IsSuccessStatusCode.Should().BeFalse();
            result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            var validationProblemDetails = await ApiClientResponse.GetResponses<ValidationProblemDetails>(result.Content);
            validationProblemDetails.Errors.SelectMany(x => x.Value).Should()
                .Contain($"Please provide a valid {nameof(hearingId)}");
        }
        
        [Test]
        public async Task should_return_bad_request_when_participant_id_is_invalid()
        {
            // arrange
            var hearingId = Guid.NewGuid();
            var participantId = Guid.Empty;
            
            // act
            using var client = Application.CreateClient();
            var result = await client
                .GetAsync(ApiUriFactory.ParticipantsEndpoints.GetParticipantInHearing(hearingId, participantId));


            // assert
            result.IsSuccessStatusCode.Should().BeFalse();
            result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            var validationProblemDetails = await ApiClientResponse.GetResponses<ValidationProblemDetails>(result.Content);
            validationProblemDetails.Errors.SelectMany(x => x.Value).Should()
                .Contain($"Please provide a valid {nameof(participantId)}");
        }
    }
}
