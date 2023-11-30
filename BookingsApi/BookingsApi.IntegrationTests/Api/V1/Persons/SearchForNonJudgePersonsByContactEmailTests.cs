namespace BookingsApi.IntegrationTests.Api.V1.Persons
{
    public class SearchForNonJudgePersonsByContactEmailTests : ApiTest
    {
        [Test]
        public async Task should_return_bad_request_when_contact_email_is_invalid()
        {
            // arrange
            var contactEmail = "invalid-email";
            
            // act
            using var client = Application.CreateClient();
            var result = await client
                .GetAsync(ApiUriFactory.PersonEndpoints.SearchForNonJudicialPersonsByContactEmail(contactEmail));


            // assert
            result.IsSuccessStatusCode.Should().BeFalse();
            result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            var validationProblemDetails = await ApiClientResponse.GetResponses<ValidationProblemDetails>(result.Content);
            validationProblemDetails.Errors.SelectMany(x => x.Value).Should()
                .Contain($"Please provide a valid {nameof(contactEmail)}");
        }
    }
}
