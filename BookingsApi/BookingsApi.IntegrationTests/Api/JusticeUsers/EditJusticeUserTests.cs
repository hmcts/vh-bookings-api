using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using BookingsApi.Contract.Requests;
using BookingsApi.Contract.Requests.Enums;
using BookingsApi.Contract.Responses;
using BookingsApi.DAL;
using BookingsApi.Domain;
using BookingsApi.Domain.Enumerations;
using BookingsApi.IntegrationTests.Helper;
using BookingsApi.Validations;
using FizzWare.NBuilder;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using NUnit.Framework;
using Testing.Common.Builders.Api;

namespace BookingsApi.IntegrationTests.Api.JusticeUsers
{
    public class EditAJusticeUserTests : ApiTest
    {
        private EditJusticeUserRequest _request;
        private Guid? _justiceUserId;
        
        [Test]
        public async Task should_edit_justice_user()
        {
            // arrange
            using var client = Application.CreateClient();
            var justiceUserToEdit = await SeedJusticeUser("api_test_edit_justice_user@test.com");
            _request = BuildValidEditJusticeUserRequest(justiceUserToEdit);
            
            // act
            var result = await client.PatchAsync(
                ApiUriFactory.JusticeUserEndpoints.EditAJusticeUser, RequestBody.Set(_request));
            
            // assert
            result.IsSuccessStatusCode.Should().BeTrue();
            result.StatusCode.Should().Be(HttpStatusCode.OK);
            
            var response = await ApiClientResponse.GetResponses<JusticeUserResponse>(result.Content);
            
            await using var db = new BookingsDbContext(BookingsDbContextOptions);
            var justiceUser = db.JusticeUsers.FirstOrDefault(x => x.Username == _request.Username);
            justiceUser.Should().NotBeNull();
            justiceUser.UserRoleId.Should().Be(9);
            justiceUser.Id.Should().Be(response.Id);
        }

        [Test]
        public async Task should_return_bad_request_when_an_invalid_payload_to_edit_a_justice_user_is_sent()
        {
            // arrange
            _request = Builder<EditJusticeUserRequest>.CreateNew()
                .With(x => x.Username, null)
                .Build();
            
            using var client = Application.CreateClient();
            
            // act
            var result = await client.PatchAsync(
                ApiUriFactory.JusticeUserEndpoints.EditAJusticeUser, RequestBody.Set(_request));
            
            // assert
            result.IsSuccessStatusCode.Should().BeFalse();
            
            result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            var validationProblemDetails = await ApiClientResponse.GetResponses<ValidationProblemDetails>(result.Content);
            validationProblemDetails.Errors[nameof(_request.Username)][0].Should()
                .Be(EditJusticeUserRequestValidation.NoUsernameErrorMessage);
        }
        
        [Test]
        public async Task should_return_not_found_when_a_justice_user_with_requested_id_not_found()
        {
            // arrange
            _request = Builder<EditJusticeUserRequest>.CreateNew().Build();
            
            using var client = Application.CreateClient();
            
            // act
            var result = await client.PatchAsync(
                ApiUriFactory.JusticeUserEndpoints.EditAJusticeUser, RequestBody.Set(_request));
            
            // assert
            result.IsSuccessStatusCode.Should().BeFalse();
            
            result.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }
        
        [TearDown]
        public async Task TearDown()
        {
            await using var db = new BookingsDbContext(BookingsDbContextOptions);
            var justiceUser = db.JusticeUsers.FirstOrDefault(x => x.Id == _justiceUserId);
            if (justiceUser != null)
            {
                db.Remove(justiceUser);
                await db.SaveChangesAsync();
            }
        }
        
        private static EditJusticeUserRequest BuildValidEditJusticeUserRequest(JusticeUser justiceUser)
        {
            return Builder<EditJusticeUserRequest>.CreateNew()
                .With(x=> x.Username, justiceUser.Username)
                .With(x=> x.Id, justiceUser.Id)
                .With(x => x.Role = JusticeUserRole.VhTeamLead)
                .Build();
        }

        private async Task<JusticeUser> SeedJusticeUser(string username)
        {
            await using var db = new BookingsDbContext(BookingsDbContextOptions);
  
            var justiceUser = db.JusticeUsers.Add(new JusticeUser
            {
                ContactEmail = username,
                Username = username,
                UserRoleId = (int)UserRoleId.Vho,
                CreatedBy = "editjusticeuser.test@test.com",
                CreatedDate = DateTime.UtcNow,
                FirstName = "ApiTest",
                Lastname = "User",
            });
            
            for (var i = 1; i <= 7; i++)
            {
                justiceUser.Entity.VhoWorkHours.Add(new VhoWorkHours
                {
                    DayOfWeekId = i, 
                    StartTime = new TimeSpan(8, 0, 0), 
                    EndTime = new TimeSpan(17, 0, 0)
                });
            }
            
            await db.SaveChangesAsync();

            _justiceUserId = justiceUser.Entity.Id;
            
            return justiceUser.Entity;
        }
    }
}