namespace BookingsApi.IntegrationTests.Api.V1.Persons
{
    public class UpdatePersonUsernameTests : ApiTest
    {
        [Test]
        public async Task should_return_bad_request_when_contact_email_is_invalid()
        {
            // arrange
            var contactEmail = "invalid-email";
            var username = "email@email.com";
            
            // act
            using var client = Application.CreateClient();
            var result = await client
                .PutAsync(ApiUriFactory.PersonEndpoints.UpdatePersonUsername(contactEmail, username), null);

            // assert
            result.IsSuccessStatusCode.Should().BeFalse();
            result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            var validationProblemDetails = await ApiClientResponse.GetResponses<ValidationProblemDetails>(result.Content);
            validationProblemDetails.Errors.SelectMany(x => x.Value).Should()
                .Contain($"Please provide a valid {nameof(contactEmail)}");
        }
        
        [Test]
        public async Task should_return_bad_request_when_username_is_invalid()
        {
            // arrange
            var contactEmail = "email@email.com";
            var username = "invalid-username";
            
            // act
            using var client = Application.CreateClient();
            var result = await client
                .PutAsync(ApiUriFactory.PersonEndpoints.UpdatePersonUsername(contactEmail, username), null);

            // assert
            result.IsSuccessStatusCode.Should().BeFalse();
            result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            var validationProblemDetails = await ApiClientResponse.GetResponses<ValidationProblemDetails>(result.Content);
            validationProblemDetails.Errors.SelectMany(x => x.Value).Should()
                .Contain($"Please provide a valid {nameof(username)}");
        }
    }
}
