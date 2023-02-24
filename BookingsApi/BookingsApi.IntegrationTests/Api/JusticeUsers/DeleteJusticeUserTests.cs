using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using BookingsApi.DAL;
using BookingsApi.Domain;
using BookingsApi.Domain.Enumerations;
using BookingsApi.IntegrationTests.Helper;
using FizzWare.NBuilder;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using Testing.Common.Builders.Api;

namespace BookingsApi.IntegrationTests.Api.JusticeUsers
{
    public class DeleteJusticeUserTests : ApiTest
    {
        private const string Username = "api_test_delete_justice_user@test.com";

        [Test]
        public async Task Should_delete_justice_user()
        {
            // Arrange
            using var client = Application.CreateClient();
            var hearing = await SeedHearing();
            var justiceUser = await SeedJusticeUser(hearing.Id);

            // Act
            var result = await client.DeleteAsync(
                ApiUriFactory.JusticeUserEndpoints.DeleteJusticeUser(justiceUser.Id));
            
            // Assert
            result.IsSuccessStatusCode.Should().BeTrue();
            result.StatusCode.Should().Be(HttpStatusCode.NoContent);

            await using var db = new BookingsDbContext(BookingsDbContextOptions);
            var foundJusticeUser = await db.JusticeUsers
                .IgnoreQueryFilters()
                .Include(u => u.VhoWorkHours)
                .Include(u => u.VhoNonAvailability)
                .Include(u => u.Allocations)
                .FirstOrDefaultAsync(x => x.Id == justiceUser.Id);
            foundJusticeUser.Should().NotBeNull();
            foundJusticeUser.Deleted.Should().BeTrue();
            foundJusticeUser.VhoWorkHours.Count.Should().Be(justiceUser.VhoWorkHours.Count);
            foreach (var workHour in foundJusticeUser.VhoWorkHours)
            {
                workHour.Deleted.Should().BeTrue();
            }
            foundJusticeUser.VhoNonAvailability.Count.Should().Be(justiceUser.VhoNonAvailability.Count);
            foreach (var nonAvailability in foundJusticeUser.VhoNonAvailability)
            {
                nonAvailability.Deleted.Should().BeTrue();
            }
            foundJusticeUser.Allocations.Should().BeEmpty();
        }

        [Test]
        public async Task Should_return_not_found_when_justice_user_does_not_exist()
        {
            // Arrange
            using var client = Application.CreateClient();
            var id = Guid.NewGuid();

            // Act
            var result = await client.DeleteAsync(
                ApiUriFactory.JusticeUserEndpoints.DeleteJusticeUser(id));
            
            // Assert
            result.IsSuccessStatusCode.Should().BeFalse();
            result.StatusCode.Should().Be(HttpStatusCode.NotFound);

            var responseBody = await ApiClientResponse.GetResponses<string>(result.Content);
            responseBody.Should().Be($"Justice user with id {id} not found");
        }

        [Test]
        public async Task Should_return_bad_request_when_invalid_id_specified()
        {
            // Arrange
            using var client = Application.CreateClient();
            var id = Guid.Empty;
            
            // Act
            var result = await client.DeleteAsync(
                ApiUriFactory.JusticeUserEndpoints.DeleteJusticeUser(id));
            
            // Assert
            result.IsSuccessStatusCode.Should().BeFalse();
            result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            var responseBody = await ApiClientResponse.GetResponses<ValidationProblemDetails>(result.Content);
            var errors = responseBody.Errors["id"];
            errors[0].Should().Be($"Please provide a valid {nameof(id)}");
        }
        
        [TearDown]
        public async Task TearDown()
        {
            await using var db = new BookingsDbContext(BookingsDbContextOptions);
            var justiceUser = db.JusticeUsers.FirstOrDefault(x => x.Username == Username);
            if (justiceUser != null)
            {
                db.Remove(justiceUser);
                await db.SaveChangesAsync();
            }
        }

        private async Task<VideoHearing> SeedHearing() => await Hooks.SeedVideoHearing();

        private async Task<JusticeUser> SeedJusticeUser(Guid allocatedHearingId)
        {
            await using var db = new BookingsDbContext(BookingsDbContextOptions);
            
            // Justice user
            var justiceUser = db.JusticeUsers.Add(new JusticeUser
            {
                ContactEmail = Username,
                Username = Username,
                UserRoleId = (int)UserRoleId.Vho,
                CreatedBy = "integration.test@test.com",
                CreatedDate = DateTime.UtcNow,
                FirstName = "ApiTest",
                Lastname = "User",
            });
            
            // Work hours
            for (var i = 1; i <= 7; i++)
            {
                justiceUser.Entity.VhoWorkHours.Add(new VhoWorkHours
                {
                    DayOfWeekId = i, 
                    StartTime = new TimeSpan(8, 0, 0), 
                    EndTime = new TimeSpan(17, 0, 0)
                });
            }
            
            // Non availabilities
            justiceUser.Entity.VhoNonAvailability.Add(new VhoNonAvailability
            {
                StartTime = new DateTime(2022, 1, 1, 8, 0, 0),
                EndTime = new DateTime(2022, 1, 1, 17, 0, 0)
            });
            justiceUser.Entity.VhoNonAvailability.Add(new VhoNonAvailability
            {
                StartTime = new DateTime(2022, 1, 2, 8, 0, 0),
                EndTime = new DateTime(2022, 1, 2, 17, 0, 0)
            });
            
            // Allocations
            db.Allocations.Add(new Allocation
            {
                HearingId = allocatedHearingId,
                JusticeUserId = justiceUser.Entity.Id
            });

            await db.SaveChangesAsync();

            return justiceUser.Entity;
        }
    }
}
