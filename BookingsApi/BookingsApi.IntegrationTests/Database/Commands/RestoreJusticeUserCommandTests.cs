using BookingsApi.DAL.Commands;
using BookingsApi.DAL.Exceptions;

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
            })!.Message.Should().Be($"Justice user with id {id} not found");
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
            var justiceUser = await Hooks.SeedJusticeUser(username, "Test", "User", isTeamLead: false, isDeleted: true,
                initWorkHours: true);
            db.Attach(justiceUser);

            justiceUser.AddOrUpdateNonAvailability(
                new DateTime(2022, 1, 1, 8, 0, 0),
                new DateTime(2022, 1, 1, 17, 0, 0)
                );
            
            justiceUser.AddOrUpdateNonAvailability(
                new DateTime(2022, 1, 2, 8, 0, 0),
                new DateTime(2022, 1, 2, 17, 0, 0)
            );
            
            hearing.AllocateVho(justiceUser);
            
            // call delete again to clear non availability hours
            justiceUser.Delete();
            
            // Allocations
            
            await db.SaveChangesAsync();

            return justiceUser;
        }
    }
}
