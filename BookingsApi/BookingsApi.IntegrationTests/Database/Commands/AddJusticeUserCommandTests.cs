using BookingsApi.DAL.Commands;
using BookingsApi.DAL.Exceptions;
using BookingsApi.Domain.Enumerations;
using Microsoft.EntityFrameworkCore;

namespace BookingsApi.IntegrationTests.Database.Commands;

public class AddJusticeUserCommandTests : DatabaseTestsBase
{
    private AddJusticeUserCommandHandler _commandHandler;

    [SetUp]
    public void Setup()
    {
        var context = new BookingsDbContext(BookingsDbContextOptions);
        _commandHandler = new AddJusticeUserCommandHandler(context);
    }

    [TestCase(UserRoleId.Vho)]
    [TestCase(UserRoleId.VhTeamLead)]
    public async Task should_add_a_justice_user(UserRoleId roleId)
    {
        var command = new AddJusticeUserCommand(
            "test1", 
            "test2", 
            "should_add_a_justice_user@testdb.com",
            "should_add_a_justice_user_contact@testdb.com", 
            "db@test.com",
            (int) roleId)
        {
            Telephone = "01234567890"
        };

        await _commandHandler.Handle(command);

        await using var db = new BookingsDbContext(BookingsDbContextOptions);
        var justiceUser = db.JusticeUsers
            .Include(ju => ju.JusticeUserRoles).ThenInclude(jur => jur.UserRole)
            .FirstOrDefault(ju => ju.Username == command.Username);
        Hooks.AddJusticeUserForCleanup(justiceUser.Id);

        justiceUser.Should().NotBeNull();
        justiceUser.FirstName.Should().Be(command.FirstName);
        justiceUser.Lastname.Should().Be(command.Lastname);
        justiceUser.ContactEmail.Should().Be(command.ContactEmail);
        justiceUser.CreatedBy.Should().Be(command.CreatedBy);
        justiceUser.JusticeUserRoles.First().UserRole.Id.Should().Be(command.RoleIds.First());
        justiceUser.Telephone.Should().Be(command.Telephone);
    }

    [Test]
    public async Task should_throw_an_exception_when_adding_a_justice_user_with_an_existing_username()
    {
        var command = new AddJusticeUserCommand("test1", "test2", "should_add_a_justice_user@testdb.com",
            "should_throw_an_exception_when_adding_a_justice_user_with_an_existing_username@testdb.com", "db@test.com", (int) UserRoleId.Vho);
        
        await _commandHandler.Handle(command);

        Assert.ThrowsAsync<JusticeUserAlreadyExistsException>(async () =>
        {
            await _commandHandler.Handle(command);
        }).Message.Should().Be($"A justice user with the username {command.Username} already exists");
    }

    [Test]
    public async Task should_throw_an_exception_when_adding_a_justice_user_with_an_existing_username_who_has_been_deleted()
    {
        var username = "should_add_a_justice_user_deleted_user@testdb.com";
        
        var command = new AddJusticeUserCommand("test1", "test2", username,
            "should_throw_an_exception_when_adding_a_justice_user_with_an_existing_username_who_has_been_deleted@testdb.com", "db@test.com", (int) UserRoleId.Vho);

        await _commandHandler.Handle(command);

        await using var db = new BookingsDbContext(BookingsDbContextOptions);
        
        var justiceUser = db.JusticeUsers.FirstOrDefault(x => x.Username == username);
        justiceUser.Delete();
        
        await db.SaveChangesAsync();

        Assert.ThrowsAsync<JusticeUserAlreadyExistsException>(async () =>
        {
            await _commandHandler.Handle(command);
        }).Message.Should().Be($"A justice user with the username {command.Username} already exists");
    }
}