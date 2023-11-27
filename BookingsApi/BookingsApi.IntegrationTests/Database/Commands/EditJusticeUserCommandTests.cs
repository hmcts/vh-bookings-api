using AcceptanceTests.Common.Model.UserRole;
using BookingsApi.DAL.Commands;
using BookingsApi.DAL.Exceptions;
using BookingsApi.Domain.Enumerations;

namespace BookingsApi.IntegrationTests.Database.Commands;

public class EditJusticeUserCommandTests : DatabaseTestsBase
{
    private EditJusticeUserCommandHandler _commandHandler;
    private VideoHearing _hearing;

    [SetUp]
    public void Setup()
    {
        var context = new BookingsDbContext(BookingsDbContextOptions);
        _commandHandler = new EditJusticeUserCommandHandler(context);
    }

    [TestCase(UserRoleId.Vho)]
    [TestCase(UserRoleId.VhTeamLead)]
    public async Task should_edit_a_justice_user(UserRoleId roleId)
    {
        // Arrange
        _hearing = await Hooks.SeedVideoHearing();
        var fName = "First";
        var lName = "Last";
        var number = "12345";
        var userToEdit = await SeedJusticeUser(_hearing.Id, "testuser@hmcts.net");
        
        var command = new EditJusticeUserCommand(userToEdit.Id, userToEdit.Username,fName, lName, number, (int)roleId);
        
        // Act
        await _commandHandler.Handle(command);
        await using var db = new BookingsDbContext(BookingsDbContextOptions);
        var justiceUser = db.JusticeUsers
            .Include(ju => ju.JusticeUserRoles).ThenInclude(jur => jur.UserRole)
            .FirstOrDefault(ju => ju.Username == command.Username);
        Hooks.AddJusticeUserForCleanup(justiceUser.Id);
        // Assert
        justiceUser.Should().NotBeNull();
        justiceUser.JusticeUserRoles.Select(e => e.UserRole.Id).Should().Contain((int)roleId);
        justiceUser.FirstName.Should().Be(fName);
        justiceUser.Lastname.Should().Be(lName);
        justiceUser.Telephone.Should().Be(number);
    }
    
    [TestCase(UserRoleId.Vho)]
    [TestCase(UserRoleId.VhTeamLead)]
    public async Task should_edit_a_justice_user_not_update_first_last_name(UserRoleId roleId)
    {
        // Arrange
        _hearing = await Hooks.SeedVideoHearing();
        var userToEdit = await SeedJusticeUser(_hearing.Id, "testuser@hmcts.net");
        var fName = userToEdit.FirstName;
        var lName = userToEdit.Lastname;
        var number = userToEdit.Telephone;
        var command = new EditJusticeUserCommand(userToEdit.Id, userToEdit.Username, null, null, null, (int)roleId);
        
        // Act
        await _commandHandler.Handle(command);
        await using var db = new BookingsDbContext(BookingsDbContextOptions);
        var justiceUser = db.JusticeUsers
            .Include(ju => ju.JusticeUserRoles).ThenInclude(jur => jur.UserRole)
            .FirstOrDefault(ju => ju.Username == command.Username);
        Hooks.AddJusticeUserForCleanup(justiceUser.Id);
        // Assert
        justiceUser.Should().NotBeNull();
        justiceUser.JusticeUserRoles.Select(e => e.UserRole.Id).Should().Contain((int)roleId);
        justiceUser.FirstName.Should().Be(fName);
        justiceUser.Lastname.Should().Be(lName);
        justiceUser.Telephone.Should().Be(number);
    }

    [Test]
    public void should_throw_an_exception_when_justice_user_not_found()
    {
        // Arrange
        var id = Guid.NewGuid();
        var command = new EditJusticeUserCommand(id, "testuser2@hmcts.net", null, null, null, (int)UserRole.VideoHearingsOfficer);

        // Act & Assert
        Assert.ThrowsAsync<JusticeUserNotFoundException>(async () =>
        {
            await _commandHandler.Handle(command);
        }).Message.Should().Be($"Justice user with id {id} not found");
    }

    private async Task<JusticeUser> SeedJusticeUser(Guid allocatedHearingId, string username)
    {
        await using var db = new BookingsDbContext(BookingsDbContextOptions);
        
        // Justice user
        var justiceUser = db.JusticeUsers.Add(new JusticeUser
        {
            ContactEmail = "testuser@hmcts.net",
            Username = username,
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
        _hearing.AllocateJusticeUser(justiceUser.Entity);

        await TestDataManager.SeedJusticeUsersRole(db, justiceUser.Entity, (int)UserRoleId.Vho);

        await db.SaveChangesAsync();

        return justiceUser.Entity;
    }
}