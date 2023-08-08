using BookingsApi.DAL.Commands;
using BookingsApi.Contract.V1.Requests;
using BookingsApi.DAL.Queries;

namespace BookingsApi.IntegrationTests.Database.Commands
{
    public class UploadNonWorkingHoursCommandDatabaseTests : AllocationDatabaseTestsBase
    {
        private UploadNonWorkingHoursCommandHandler _commandHandler;
        private GetHearingByIdQueryHandler _getHearingByIdQueryHandler;
        private BookingsDbContext _context;

        [SetUp]
        public void Setup()
        {
            _context = Context;
            _commandHandler = new UploadNonWorkingHoursCommandHandler(_context, HearingAllocationService);
            _getHearingByIdQueryHandler = new GetHearingByIdQueryHandler(_context);
        }

        [Test]
        public async Task Should_return_username_if_it_doesnt_exist()
        {
            // Arrange
            var username = "dontexist@test.com";
            var requests = new List<UploadNonWorkingHoursRequest> {
                new UploadNonWorkingHoursRequest(
                    username,
                    DateTime.Now,
                    DateTime.Now.AddDays(2)
                )
            };

            var command = new UploadNonWorkingHoursCommand(requests);

            // Act
            await _commandHandler.Handle(command);

            // Assert
            command.FailedUploadUsernames.Count.Should().Be(1);
            command.FailedUploadUsernames[0].Should().Be(username);
        }

        [Test]
        public async Task Should_save_non_working_hours_to_database()
        {
            // Arrange
            var oldNonAvailabilitiesCount = _context.VhoNonAvailabilities.Count();

            var justiceUserOne = await Hooks
                .SeedJusticeUser("team.lead.1@hearings.reform.hmcts.net", "firstName", "secondname", true);
            var justiceUserTwo = await Hooks
                .SeedJusticeUser("team.lead.2@hearings.reform.hmcts.net", "firstName2", "secondname2", true);

            var justiceUserOneNonWorkingHoursStartTime = new DateTime(2022, 2, 1);
            var justiceUserOneNonWorkingHoursEndTime = new DateTime(2022, 1, 1);
            var justiceUserTwoNonWorkingHoursStartTime = new DateTime(2022, 2, 10, 9, 0, 0);
            var justiceUserTwoNonWorkingHoursEndTime = new DateTime(2022, 2, 11, 16, 30, 0);

            var requests = new List<UploadNonWorkingHoursRequest> {
                new UploadNonWorkingHoursRequest(
                    justiceUserOne.Username,
                    justiceUserOneNonWorkingHoursStartTime,
                    justiceUserOneNonWorkingHoursEndTime
                ),
                new UploadNonWorkingHoursRequest(
                    justiceUserTwo.Username,
                    justiceUserTwoNonWorkingHoursStartTime,
                    justiceUserTwoNonWorkingHoursEndTime
                )
            };

            var command = new UploadNonWorkingHoursCommand(requests);
            await _commandHandler.Handle(command);

            // Test user with existing non-working hours gets updated
            requests[1].EndTime = requests[1].EndTime.AddHours(1);

            command = new UploadNonWorkingHoursCommand(requests);

            // Act
            await _commandHandler.Handle(command);

            var nonAvailabilities = _context.VhoNonAvailabilities;
            var justiceUserOneNonWorkHours = nonAvailabilities.SingleOrDefault(x => x.JusticeUserId == justiceUserOne.Id);
            var justiceUserTwoNonWorkHours = nonAvailabilities.SingleOrDefault(x => x.JusticeUserId == justiceUserTwo.Id);

            // Assert
            nonAvailabilities.Count().Should().Be(oldNonAvailabilitiesCount + 2);
            justiceUserOneNonWorkHours.StartTime.Should().Be(justiceUserOneNonWorkingHoursStartTime);
            justiceUserOneNonWorkHours.EndTime.Should().Be(justiceUserOneNonWorkingHoursEndTime);
            justiceUserTwoNonWorkHours.StartTime.Should().Be(justiceUserTwoNonWorkingHoursStartTime);
            justiceUserTwoNonWorkHours.EndTime.Should().Be(new DateTime(2022, 2, 11, 17, 30, 0));
        }

        [Test]
        public async Task Should_not_duplicate_overlapping_non_working_hours_to_database()
        {
            // Arrange
            var oldNonAvailabilitiesCount = _context.VhoNonAvailabilities.Count();

            var justiceUser = await Hooks
                .SeedJusticeUser("team.lead.1@hearings.reform.hmcts.net", "firstName", "secondname", true);

            var justiceUserNonWorkingHoursStartTime1 = new DateTime(2022, 2, 1);
            var justiceUserNonWorkingHoursEndTime1 = new DateTime(2022, 1, 1);
            var justiceUserNonWorkingHoursStartTime2 = new DateTime(2022, 2, 1);
            var justiceUserNonWorkingHoursEndTime2 = new DateTime(2022, 2, 11, 16, 30, 0);

            var requests = new List<UploadNonWorkingHoursRequest> {
                new UploadNonWorkingHoursRequest(
                    justiceUser.Username,
                    justiceUserNonWorkingHoursStartTime1,
                    justiceUserNonWorkingHoursEndTime1
                ),
                new UploadNonWorkingHoursRequest(
                    justiceUser.Username,
                    justiceUserNonWorkingHoursStartTime2,
                    justiceUserNonWorkingHoursEndTime2
                )
            };

            var command = new UploadNonWorkingHoursCommand(requests);
            await _commandHandler.Handle(command);

            command = new UploadNonWorkingHoursCommand(requests);

            // Act
            await _commandHandler.Handle(command);

            var nonAvailabilities = _context.VhoNonAvailabilities;
            var justiceUserNonWorkHours = nonAvailabilities.SingleOrDefault(x => x.JusticeUserId == justiceUser.Id);

            // Assert
            nonAvailabilities.Count().Should().Be(oldNonAvailabilitiesCount + 1);
            justiceUserNonWorkHours.StartTime.Should().Be(justiceUserNonWorkingHoursStartTime2);
            justiceUserNonWorkHours.EndTime.Should().Be(justiceUserNonWorkingHoursEndTime2);
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
            db.Allocations.Add(new Allocation
            {
                HearingId = seededHearing1.Id,
                JusticeUserId = allocatedUser1.Id
            });
            db.Allocations.Add(new Allocation
            {
                HearingId = seededHearing2.Id,
                JusticeUserId = allocatedUser2.Id
            });
            await db.SaveChangesAsync();
            var hearing1 = await _getHearingByIdQueryHandler.Handle(new GetHearingByIdQuery(seededHearing1.Id));
            hearing1.AllocatedTo.Should().NotBeNull();
            hearing1.AllocatedTo.Id.Should().Be(allocatedUser1.Id);
            var hearing2 = await _getHearingByIdQueryHandler.Handle(new GetHearingByIdQuery(seededHearing2.Id));
            hearing2.AllocatedTo.Should().NotBeNull();
            hearing2.AllocatedTo.Id.Should().Be(allocatedUser2.Id);

            var requests = new List<UploadNonWorkingHoursRequest> {
                new UploadNonWorkingHoursRequest(
                    allocatedUser1.Username,
                    DateTime.Today.AddDays(1).AddHours(0).AddMinutes(0),
                    DateTime.Today.AddDays(1).AddHours(23).AddMinutes(0)
                ),
                new UploadNonWorkingHoursRequest(
                    allocatedUser2.Username,
                    DateTime.Today.AddDays(1).AddHours(0).AddMinutes(0),
                    DateTime.Today.AddDays(1).AddHours(23).AddMinutes(0)
                )
            };
            
            var command = new UploadNonWorkingHoursCommand(requests);
            
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