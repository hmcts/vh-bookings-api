using BookingsApi.DAL.Commands;
using BookingsApi.DAL.Dtos;
using BookingsApi.DAL.Queries;
using Testing.Common.Builders.Domain;

namespace BookingsApi.IntegrationTests.Database.Commands
{
    public class UploadWorkHoursCommandDatabaseTests : AllocationDatabaseTestsBase
    {
        private UploadWorkHoursCommandHandler _commandHandler;
        private GetHearingByIdQueryHandler _getHearingByIdQueryHandler;
        private BookingsDbContext _context;

        [SetUp]
        public void Setup()
        {
            _context = Context;
            _commandHandler = new UploadWorkHoursCommandHandler(_context, HearingAllocationService);
            _getHearingByIdQueryHandler = new GetHearingByIdQueryHandler(_context);
        }

        [Test]
        public async Task Should_return_username_if_it_doesnt_exist()
        {
            // Arrange
            var username = "dontexist@test.com";
            var requests = new List<UploadWorkHoursDto> {
                new (username, new List<WorkHoursDto>())
            };

            var command = new UploadWorkHoursCommand(requests);

            // Act
            await _commandHandler.Handle(command);

            // Assert
            command.FailedUploadUsernames.Count.Should().Be(1);
            command.FailedUploadUsernames[0].Should().Be(username);
        }

        [Test]
        public async Task Should_save_work_hours_to_database()
        {
            // Arrange
            var justiceUserOne = await Hooks
                .SeedJusticeUser("team.lead.1@hearings.reform.hmcts.net", "firstName", "secondname", true, initWorkHours:false);
            var justiceUserTwo = await Hooks
                .SeedJusticeUser("team.lead.2@hearings.reform.hmcts.net", "firstName2", "secondname2", true, initWorkHours:false);

            var requests = new List<UploadWorkHoursDto> {
                new (justiceUserOne.Username, new List<WorkHoursDto>
                {
                    new (1, 9, 0, 17, 0)
                }),
                new (justiceUserTwo.Username, new List<WorkHoursDto>
                {
                    new (2, 9, 30, 17, 30)
                })
            };

            var command = new UploadWorkHoursCommand(requests);
            await _commandHandler.Handle(command);

            // Test user with existing work hours  gets updated
            requests[0].WorkingHours[0].SetProtected(nameof(WorkHoursDto.EndTimeHour), 18);

            command = new UploadWorkHoursCommand(requests);

            // Act
            await _commandHandler.Handle(command);

            
            var justiceUserOneDb = await _context.JusticeUsers.Include(x => x.VhoWorkHours)
                .FirstAsync(x => x.Id == justiceUserOne.Id);
            var justiceUserTwoDb = await _context.JusticeUsers.Include(x => x.VhoWorkHours)
                .FirstAsync(x => x.Id == justiceUserTwo.Id);
            var justiceUserOneWorkHours = justiceUserOneDb.VhoWorkHours[0];
            var justiceUserTwoWorkHours = justiceUserTwoDb.VhoWorkHours[0];

            // Assert
            justiceUserOneWorkHours.DayOfWeekId.Should().Be(1);
            justiceUserOneWorkHours.StartTime.Should().Be(new TimeSpan(9, 0, 0));
            justiceUserOneWorkHours.EndTime.Should().Be(new TimeSpan(18, 0, 0));
            
            justiceUserTwoWorkHours.DayOfWeekId.Should().Be(2);
            justiceUserTwoWorkHours.StartTime.Should().Be(new TimeSpan(9, 30, 0));
            justiceUserTwoWorkHours.EndTime.Should().Be(new TimeSpan(17, 30, 0));
        }

        [Test]
        public async Task Should_deallocate_hearings_when_users_no_longer_available()
        {
            // Arrange
            var seededHearing1 = await Hooks.SeedVideoHearing();
            var seededHearing2 = await Hooks.SeedVideoHearing();
            var allocatedUser1 = await Hooks.SeedJusticeUser("cso1@email.com", "Cso1", "Test");
            var allocatedUser2 = await Hooks.SeedJusticeUser("cso2@email.com", "Cso2", "Test");
            await using var db = new BookingsDbContext(BookingsDbContextOptions);
            var daysOfWeek = await db.DaysOfWeek.ToListAsync();
            await Hooks.AddAllocation(seededHearing1, allocatedUser1);
            await Hooks.AddAllocation(seededHearing2, allocatedUser2);
            
            var hearing1 = await _getHearingByIdQueryHandler.Handle(new GetHearingByIdQuery(seededHearing1.Id));
            hearing1.AllocatedTo.Should().NotBeNull();
            hearing1.AllocatedTo.Id.Should().Be(allocatedUser1.Id);
            var hearing2 = await _getHearingByIdQueryHandler.Handle(new GetHearingByIdQuery(seededHearing2.Id));
            hearing2.AllocatedTo.Should().NotBeNull();
            hearing2.AllocatedTo.Id.Should().Be(allocatedUser2.Id);

            var dayOfWeek1 = daysOfWeek.First(x => x.Day == hearing1.ScheduledDateTime.DayOfWeek.ToString());
            var dayOfWeek2 = daysOfWeek.First(x => x.Day == hearing1.ScheduledDateTime.DayOfWeek.ToString());
            var dto = new List<UploadWorkHoursDto>
            {
                new(allocatedUser1.Username, new List<WorkHoursDto>
                {
                    new(dayOfWeek1.Id, 22, 0, 23, 0)
                }),
                new(allocatedUser2.Username, new List<WorkHoursDto>
                {
                    new(dayOfWeek2.Id, 22, 0, 23, 0)
                })
            };
            
            var command = new UploadWorkHoursCommand(dto);
            
            // Act
            await _commandHandler.Handle(command);
            
            // Assert
            hearing1 = await _getHearingByIdQueryHandler.Handle(new GetHearingByIdQuery(seededHearing1.Id));
            hearing1.AllocatedTo.Should().BeNull();
            hearing2 = await _getHearingByIdQueryHandler.Handle(new GetHearingByIdQuery(seededHearing2.Id));
            hearing2.AllocatedTo.Should().BeNull();
        }
    }
}