using AcceptanceTests.Common.Model.UserRole;
using BookingsApi.DAL.Commands;
using BookingsApi.DAL.Exceptions;
using BookingsApi.Domain.Enumerations;

namespace BookingsApi.IntegrationTests.Database.Commands;

public class EditJusticeUserCommandTests : DatabaseTestsBase
{
    private EditJusticeUserCommandHandler _commandHandler;

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
        var hearing = await SeedHearing();
        var userToEdit = await SeedJusticeUser(hearing.Id, "testuser@hmcts.net");
        
        var command = new EditJusticeUserCommand(userToEdit.Id, userToEdit.Username, (int)roleId);
        
        // Act
        await _commandHandler.Handle(command);
        await using var db = new BookingsDbContext(BookingsDbContextOptions);
        var justiceUser = db.JusticeUsers
            .Include(ju => ju.JusticeUserRoles).ThenInclude(jur => jur.UserRole)
            .FirstOrDefault(ju => ju.Username == command.Username);
        Hooks._seededJusticeUserIds.Add(justiceUser.Id);
        // Assert
        justiceUser.Should().NotBeNull();
        justiceUser.JusticeUserRoles.Select(e => e.UserRole.Id).Should().Contain((int)roleId);
    }

    [Test]
    public void should_throw_an_exception_when_justice_user_not_found()
    {
        // Arrange
        var id = Guid.NewGuid();
        var command = new EditJusticeUserCommand(id, "testuser2@hmcts.net", (int)UserRole.VideoHearingsOfficer);

        // Act & Assert
        Assert.ThrowsAsync<JusticeUserNotFoundException>(async () =>
        {
            await _commandHandler.Handle(command);
        }).Message.Should().Be($"Justice user with id {id} not found");
    }
    

    private async Task<VideoHearing> SeedHearing() => await Hooks.SeedVideoHearing();

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
        db.Allocations.Add(new Allocation
        {
            HearingId = allocatedHearingId,
            JusticeUserId = justiceUser.Entity.Id
        });

        await Hooks.SeedJusticeUsersRole(db, justiceUser.Entity, (int)UserRoleId.Vho);

        await db.SaveChangesAsync();
        
        var allocationIds = db.Allocations.Where(x => x.JusticeUserId == justiceUser.Entity.Id).Select(e => e.Id);
        Hooks._seededAllocationIds.AddRange(allocationIds);
        
        return justiceUser.Entity;
    }
}