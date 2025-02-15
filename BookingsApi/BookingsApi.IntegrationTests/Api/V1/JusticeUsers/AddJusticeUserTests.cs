using Bogus;
using BookingsApi.Contract.V1.Requests;
using BookingsApi.Contract.V1.Responses;
using BookingsApi.Validations.V1;
using FizzWare.NBuilder;
using JusticeUserRole = BookingsApi.Contract.V1.Requests.Enums.JusticeUserRole;

namespace BookingsApi.IntegrationTests.Api.V1.JusticeUsers
{
    public class AddJusticeUserTests : ApiTest
    {
        private AddJusticeUserRequest _request;
        private static readonly Faker Faker = new();

        [Test]
        public async Task should_add_new_justice_user()
        {
            // arrange
            _request = BuildValidAddJusticeUserRequest();
            using var client = Application.CreateClient();
            
            // act
            var result = await client.PostAsync(ApiUriFactory.JusticeUserEndpoints.AddJusticeUser, RequestBody.Set(_request));
            
            // assert
            result.IsSuccessStatusCode.Should().BeTrue();
            result.StatusCode.Should().Be(HttpStatusCode.Created);
            
            var getJusticeUserUri = result.Headers.Location;
            var getResponse = await client.GetAsync(getJusticeUserUri);
            var createdResponse = await ApiClientResponse.GetResponses<JusticeUserResponse>(result.Content);
            var justiceUserResponse = await ApiClientResponse.GetResponses<JusticeUserResponse>(getResponse.Content);
            
            createdResponse.Should().BeEquivalentTo(justiceUserResponse);
            await using var db = new BookingsDbContext(BookingsDbContextOptions);
            var justiceUser = await db.JusticeUsers.FirstAsync(x => x.Username == _request.Username);
            justiceUser.Id.Should().Be(justiceUserResponse.Id);
            Hooks.AddJusticeUserForCleanup(justiceUser.Id);
        }

        [Test]
        public async Task should_return_bad_request_when_an_invalid_payload_to_create_a_justice_user_is_sent()
        {
            // arrange
            _request = BuildValidAddJusticeUserRequest();
            _request.FirstName = null;
            using var client = Application.CreateClient();
            
            // act
            var result = await client.PostAsync(
                ApiUriFactory.JusticeUserEndpoints.AddJusticeUser, RequestBody.Set(_request));
            
            // assert
            result.IsSuccessStatusCode.Should().BeFalse();
            
            result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            var validationProblemDetails = await ApiClientResponse.GetResponses<ValidationProblemDetails>(result.Content);
            validationProblemDetails.Errors[nameof(_request.FirstName)][0].Should()
                .Be(AddJusticeUserRequestValidation.NoFirstNameErrorMessage);
        }

        [Test]
        public async Task should_return_conflict_when_an_existing_username_is_provided_when_adding_a_new_justice_user()
        {
            // arrange
            _request = BuildValidAddJusticeUserRequest();
            using var client = Application.CreateClient();
            
            // act
            var result1 = await client.PostAsync(
                ApiUriFactory.JusticeUserEndpoints.AddJusticeUser, RequestBody.Set(_request));
            var result2 = await client.PostAsync(
                ApiUriFactory.JusticeUserEndpoints.AddJusticeUser, RequestBody.Set(_request));
            // assert
            result1.IsSuccessStatusCode.Should().BeTrue();
            var createdResponse1 = await ApiClientResponse.GetResponses<JusticeUserResponse>(result1.Content);
            Hooks.AddJusticeUserForCleanup(createdResponse1.Id);
            result2.IsSuccessStatusCode.Should().BeFalse();
            result2.StatusCode.Should().Be(HttpStatusCode.Conflict);
        }
        
        private static AddJusticeUserRequest BuildValidAddJusticeUserRequest()
        {
            return Builder<AddJusticeUserRequest>.CreateNew()
                .With(x=> x.Username, "api_test_add_justice_user_valid@test.com")
                .With(x=> x.ContactEmail, Faker.Internet.Email())
                .With(x=>x.ContactTelephone, "01234567890")
                .With(x=>x.Roles, new List<JusticeUserRole>() { JusticeUserRole.Vho })
                .Build();
        }
    }
}