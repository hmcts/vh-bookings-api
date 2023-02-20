using System;
using System.Linq;
using System.Threading.Tasks;
using BookingsApi.DAL;
using BookingsApi.DAL.Commands;
using BookingsApi.DAL.Exceptions;
using BookingsApi.Domain.Enumerations;
using FluentAssertions;
using NUnit.Framework;

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
        var command = new AddJusticeUserCommand("test1", "test2", "should_add_a_justice_user@testdb.com",
            "should_add_a_justice_user_contact@testdb.com", "db@test.com", (int) roleId)
        {
            Telephone = "01234567890"
        };

        await _commandHandler.Handle(command);

        await using var db = new BookingsDbContext(BookingsDbContextOptions);
        var justiceUser = db.JusticeUsers.FirstOrDefault(ju => ju.Username == command.Username);
        justiceUser.Should().NotBeNull();
        justiceUser.FirstName.Should().Be(command.FirstName);
        justiceUser.Lastname.Should().Be(command.Lastname);
        justiceUser.ContactEmail.Should().Be(command.ContactEmail);
        justiceUser.CreatedBy.Should().Be(command.CreatedBy);
        justiceUser.UserRoleId.Should().Be(command.RoleId);
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

    [TearDown]
    public new async Task TearDown()
    {
        await using var db = new BookingsDbContext(BookingsDbContextOptions);
        db.JusticeUsers.RemoveRange(db.JusticeUsers.Where(ju => ju.CreatedBy == "db@test.com"));
        await db.SaveChangesAsync();
    }
}