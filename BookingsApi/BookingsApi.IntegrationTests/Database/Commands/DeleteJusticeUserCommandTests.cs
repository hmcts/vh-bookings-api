using System;
using System.Linq;
using System.Threading.Tasks;
using BookingsApi.DAL;
using BookingsApi.DAL.Commands;
using BookingsApi.DAL.Exceptions;
using BookingsApi.Domain;
using BookingsApi.Domain.Enumerations;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;

namespace BookingsApi.IntegrationTests.Database.Commands
{
    public class DeleteJusticeUserCommandTests : DatabaseTestsBase
    {
        private DeleteJusticeUserCommandHandler _commandHandler;

        [SetUp]
        public void Setup()
        {
            var context = new BookingsDbContext(BookingsDbContextOptions);
            _commandHandler = new DeleteJusticeUserCommandHandler(context);
        }

        [Test]
        public async Task should_delete_justice_user()
        {
            // Arrange
            var hearing = await SeedHearing();
            var justiceUserToDelete = await SeedJusticeUser("should_delete_justice_user@testdb.com", hearing.Id);

            var command = new DeleteJusticeUserCommand(justiceUserToDelete.Id);
            
            // Act
            await _commandHandler.Handle(command);
            
            // Assert
            await using var db = new BookingsDbContext(BookingsDbContextOptions);
            var justiceUser = await db.JusticeUsers
                .IgnoreQueryFilters()
                .Include(u => u.VhoWorkHours)
                .Include(u => u.VhoNonAvailability)
                .Include(u => u.Allocations)
                .FirstOrDefaultAsync(x => x.Id == justiceUserToDelete.Id);
            justiceUser.Should().NotBeNull();
            justiceUser.Deleted.Should().BeTrue();
            justiceUser.VhoWorkHours.Count.Should().Be(justiceUserToDelete.VhoWorkHours.Count);
            foreach (var workHour in justiceUser.VhoWorkHours)
            {
                workHour.Deleted.Should().BeTrue();
            }
            justiceUser.VhoNonAvailability.Count.Should().Be(justiceUserToDelete.VhoNonAvailability.Count);
            foreach (var nonAvailability in justiceUser.VhoNonAvailability)
            {
                nonAvailability.Deleted.Should().BeTrue();
            }
            justiceUser.Allocations.Should().BeEmpty();
        }
        
        [Test]
        public void should_throw_an_exception_when_justice_user_does_not_exist()
        {
            // Arrange
            var id = Guid.NewGuid();
            
            var command = new DeleteJusticeUserCommand(id);

            // Act & Assert
            Assert.ThrowsAsync<JusticeUserNotFoundException>(async () =>
            {
                await _commandHandler.Handle(command);
            }).Message.Should().Be($"Justice user with id {id} not found");
        }

        [TearDown]
        public new async Task TearDown()
        {
            await using var db = new BookingsDbContext(BookingsDbContextOptions);
            db.JusticeUsers.RemoveRange(db.JusticeUsers.IgnoreQueryFilters().Where(ju => ju.CreatedBy == "db@test.com"));
            await db.SaveChangesAsync();
        }
        
        private async Task<VideoHearing> SeedHearing() => await Hooks.SeedVideoHearing();

        private async Task<JusticeUser> SeedJusticeUser(string username, Guid allocatedHearingId)
        {
            await using var db = new BookingsDbContext(BookingsDbContextOptions);
            
            // Justice user
            var justiceUser = db.JusticeUsers.Add(new JusticeUser
            {
                ContactEmail = username,
                Username = username,
                //UserRoleIds = new []{(int)UserRoleId.Vho},
                CreatedBy = "db@test.com",
                CreatedDate = DateTime.UtcNow,
                FirstName = "Test",
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
