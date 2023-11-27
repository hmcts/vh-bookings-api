using BookingsApi.Contract.V1.Requests;

namespace BookingsApi.IntegrationTests.Api.V1.Persons
{
    public class UpdatePersonDetailsTests : ApiTest
    {
        [Test]
        public async Task should_return_bad_request_when_request_is_invalid()
        {
            // arrange
            var personId = Guid.NewGuid();
            var request = new UpdatePersonDetailsRequest();
            
            // act
            using var client = Application.CreateClient();
            var result = await client
                .PutAsync(ApiUriFactory.PersonEndpoints.UpdatePersonDetails(personId), RequestBody.Set(request));


            // assert
            result.IsSuccessStatusCode.Should().BeFalse();
            result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            var validationProblemDetails = await ApiClientResponse.GetResponses<ValidationProblemDetails>(result.Content);
            validationProblemDetails.Errors.SelectMany(x => x.Value).Should()
                .Contain($"Username is required");
        }
    }
}
