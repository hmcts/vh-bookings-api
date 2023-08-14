using BookingsApi.DAL.Commands;
using BookingsApi.DAL.Exceptions;
using BookingsApi.Domain.Enumerations;
using Microsoft.EntityFrameworkCore;

namespace BookingsApi.IntegrationTests.Database.Commands
{
    public class RestoreJusticeUserCommandTests : DatabaseTestsBase
    {
        private RestoreJusticeUserCommandHandler _commandHandler;
        private VideoHearing _hearing;

        [SetUp]
        public void Setup()
        {
            var context = new BookingsDbContext(BookingsDbContextOptions);
            _commandHandler = new RestoreJusticeUserCommandHandler(context);
        }

        [Test]
        public async Task should_restore_justice_user()
        {
            // Arrange
            _hearing = await Hooks.SeedVideoHearing();
            var justiceUserToRestore = await SeedDeletedJusticeUser("should_delete_justice_user4@testdb.com", _hearing);

            var command = new RestoreJusticeUserCommand(justiceUserToRestore.Id);
            
            // Act
            await _commandHandler.Handle(command);
            
            // Assert
            await using var db = new BookingsDbContext(BookingsDbContextOptions);
            var justiceUser = await db.JusticeUsers
                .IgnoreQueryFilters()
                .Include(u => u.VhoWorkHours)
                .Include(u => u.VhoNonAvailability)
                .Include(u => u.Allocations)
                .FirstOrDefaultAsync(x => x.Id == justiceUserToRestore.Id);
            justiceUser.Should().NotBeNull();
            justiceUser.Deleted.Should().BeFalse();
            
            justiceUser.VhoWorkHours.Count.Should().Be(justiceUserToRestore.VhoWorkHours.Count);
            foreach (var workHour in justiceUser.VhoWorkHours)
            {
                workHour.Deleted.Should().BeTrue();
            }
            justiceUser.VhoNonAvailability.Count.Should().Be(justiceUserToRestore.VhoNonAvailability.Count);
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
            
            var command = new RestoreJusticeUserCommand(id);

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
            db.JusticeUsers.RemoveRange(db.JusticeUsers.IgnoreQueryFilters().Include(x => x.JusticeUserRoles)
                .Where(ju => ju.CreatedBy == "db@test.com"));
            await db.SaveChangesAsync();
        }

        private async Task<JusticeUser> SeedDeletedJusticeUser(string username, VideoHearing hearing)
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
                Lastname = "User"
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
            _hearing.AllocateVho(justiceUser.Entity);

            await db.SaveChangesAsync();

            var newUser = db.JusticeUsers
                // .IgnoreQueryFilters()
                .Where(x => x.Id == justiceUser.Entity.Id)
                .Include(x => x.Allocations).ThenInclude(x => x.Hearing)
                .Include(x => x.VhoWorkHours)
                .Include(x => x.VhoNonAvailability)
                .Include(x => x.JusticeUserRoles).ThenInclude(x => x.UserRole)
                .FirstOrDefault();
            
            newUser.Delete();
            await db.SaveChangesAsync();
            
            return newUser;
        }
    }
}
