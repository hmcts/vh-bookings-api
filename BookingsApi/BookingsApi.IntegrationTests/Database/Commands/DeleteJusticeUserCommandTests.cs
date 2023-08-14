using BookingsApi.DAL.Commands;
using BookingsApi.DAL.Exceptions;
using BookingsApi.Domain.Enumerations;
using BookingsApi.Domain.RefData;
using Microsoft.EntityFrameworkCore;

namespace BookingsApi.IntegrationTests.Database.Commands
{
    public class DeleteJusticeUserCommandTests : DatabaseTestsBase
    {
        private DeleteJusticeUserCommandHandler _commandHandler;
        private VideoHearing _hearing;

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
            _hearing = await Hooks.SeedVideoHearing();
            var justiceUserToDelete = await SeedJusticeUser("should_delete_justice_user@testdb.com", _hearing.Id);

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
            })!.Message.Should().Be($"Justice user with id {id} not found");
        }

        private async Task<JusticeUser> SeedJusticeUser(string username, Guid allocatedHearingId)
        {
            await using var db = new BookingsDbContext(BookingsDbContextOptions);
            
            // Justice user
            var justiceUser = db.JusticeUsers.Add(new JusticeUser
            {
                ContactEmail = username,
                Username = username,
                CreatedBy = "db@test.com",
                CreatedDate = DateTime.UtcNow,
                FirstName = "Test",
                Lastname = "User",
            });
            var userRole = db.UserRoles.First(e => e.Id == (int)UserRoleId.Vho);
            db.JusticeUserRoles.Add(new JusticeUserRole(justiceUser.Entity, userRole));
            
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
            _hearing.AllocateVho(justiceUser.Entity);

            Hooks._seededJusticeUserIds.Add(justiceUser.Entity.Id);
            await db.SaveChangesAsync();

            return justiceUser.Entity;
        }
    }
}
