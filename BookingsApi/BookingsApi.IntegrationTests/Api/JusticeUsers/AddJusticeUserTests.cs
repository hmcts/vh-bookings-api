using System.Linq;
using System.Net;
using System.Threading.Tasks;
using BookingsApi.Contract.Requests;
using BookingsApi.Contract.Responses;
using BookingsApi.DAL;
using BookingsApi.IntegrationTests.Helper;
using BookingsApi.Validations;
using FizzWare.NBuilder;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using NUnit.Framework;
using Testing.Common.Builders.Api;

namespace BookingsApi.IntegrationTests.Api.JusticeUsers
{
    public class AddAJusticeUserTests : ApiTest
    {
        private AddJusticeUserRequest _request;

        [Test]
        public async Task should_add_new_justice_user()
        {
            // arrange
            _request = BuildValidAddJusticeUserRequest();
            using var client = Application.CreateClient();
            
            // act
            var result = await client.PostAsync(
                ApiUriFactory.JusticeUserEndpoints.AddAJusticeUser, RequestBody.Set(_request));
            
            // assert
            result.IsSuccessStatusCode.Should().BeTrue();
            result.StatusCode.Should().Be(HttpStatusCode.Created);
            
            var getJusticeUserUri = result.Headers.Location;
            var getResponse = await client.GetAsync(getJusticeUserUri);
            var justiceUserResponse = await ApiClientResponse.GetResponses<JusticeUserResponse>(getResponse.Content);
            
            await using var db = new BookingsDbContext(BookingsDbContextOptions);
            var justiceUser = db.JusticeUsers.FirstOrDefault(x => x.Username == _request.Username);
            justiceUser.Should().NotBeNull();
            justiceUser.Id.Should().Be(justiceUserResponse.Id);
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
                ApiUriFactory.JusticeUserEndpoints.AddAJusticeUser, RequestBody.Set(_request));
            
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
                ApiUriFactory.JusticeUserEndpoints.AddAJusticeUser, RequestBody.Set(_request));
            var result2 = await client.PostAsync(
                ApiUriFactory.JusticeUserEndpoints.AddAJusticeUser, RequestBody.Set(_request));
            // assert
            result1.IsSuccessStatusCode.Should().BeTrue();
            result2.IsSuccessStatusCode.Should().BeFalse();
            result2.StatusCode.Should().Be(HttpStatusCode.Conflict);
        }

        [TearDown]
        public async Task TearDown()
        {
            await using var db = new BookingsDbContext(BookingsDbContextOptions);
            var justiceUser = db.JusticeUsers.FirstOrDefault(x => x.Username == _request.Username);
            if (justiceUser != null)
            {
                db.Remove(justiceUser);
                await db.SaveChangesAsync();
            }
        }
        
        private static AddJusticeUserRequest BuildValidAddJusticeUserRequest()
        {
            return Builder<AddJusticeUserRequest>.CreateNew()
                .With(x=> x.Username, "api_test_add_justice_user_valid@test.com")
                .With(x=> x.ContactEmail, Faker.Internet.Email())
                .Build();
        }
    }
}