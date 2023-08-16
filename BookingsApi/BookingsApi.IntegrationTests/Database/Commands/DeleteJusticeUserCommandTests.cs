using BookingsApi.DAL.Commands;
using BookingsApi.DAL.Exceptions;
using BookingsApi.Domain.Enumerations;
using BookingsApi.Domain.RefData;

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
            var justiceUserToDelete = await SeedJusticeUser("should_delete_justice_user@testdb.com");
            await Hooks.AddAllocation(_hearing, justiceUserToDelete);

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
                .FirstAsync(x => x.Id == justiceUserToDelete.Id);
            justiceUser.Should().NotBeNull();
            justiceUser.Deleted.Should().BeTrue();
            justiceUser.VhoWorkHours.Should().BeEmpty();
            justiceUser.VhoNonAvailability.Should().BeEmpty();
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

        private async Task<JusticeUser> SeedJusticeUser(string username)
        {
            await using var db = new BookingsDbContext(BookingsDbContextOptions);
            
            // Justice user
            var justiceUser = await Hooks.SeedJusticeUser(username, "Test", "User", isTeamLead: false, initWorkHours: true);
            db.Attach(justiceUser);
            
            justiceUser.AddOrUpdateNonAvailability(
                new DateTime(2022, 1, 1, 8, 0, 0),
                new DateTime(2022, 1, 1, 17, 0, 0)
                );
            
            justiceUser.AddOrUpdateNonAvailability(
                new DateTime(2022, 1, 2, 8, 0, 0),
                new DateTime(2022, 1, 2, 17, 0, 0)
            );
            
            // Allocations
            _hearing.AllocateVho(justiceUser);
            await db.SaveChangesAsync();

            return justiceUser;
        }
    }
}
