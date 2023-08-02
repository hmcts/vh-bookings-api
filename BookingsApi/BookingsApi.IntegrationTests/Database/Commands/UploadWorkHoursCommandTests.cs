using BookingsApi.DAL;
using BookingsApi.DAL.Commands;
using FluentAssertions;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BookingsApi.Contract.V1.Requests;
using BookingsApi.DAL.Queries;
using BookingsApi.Domain;

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
            var requests = new List<UploadWorkHoursRequest> {
                new UploadWorkHoursRequest
                {
                    Username = username,
                }
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
            var oldWorkCount = _context.VhoWorkHours.Count();
            var oldWorkCount2 = _context.JusticeUsers;

            var justiceUserOne = await Hooks
                .SeedJusticeUser("team.lead.1@hearings.reform.hmcts.net", "firstName", "secondname", true);
            var justiceUserTwo = await Hooks
                .SeedJusticeUser("team.lead.2@hearings.reform.hmcts.net", "firstName2", "secondname2", true);

            var requests = new List<UploadWorkHoursRequest> {
                new UploadWorkHoursRequest
                {
                    Username = justiceUserOne.Username,
                    WorkingHours = new List<WorkingHours>
                    {
                        new WorkingHours(1, 9, 0, 17, 0)
                    }
                },
                new UploadWorkHoursRequest
                {
                    Username = justiceUserTwo.Username,
                    WorkingHours = new List<WorkingHours>
                    {
                        new WorkingHours(2, 9, 30, 17, 30)
                    }
                }
            };

            var command = new UploadWorkHoursCommand(requests);
            await _commandHandler.Handle(command);

            // Test user with existing work hours  gets updated
            requests[0].WorkingHours[0].EndTimeHour = 18;

            command = new UploadWorkHoursCommand(requests);

            // Act
            await _commandHandler.Handle(command);

            var workHours = _context.VhoWorkHours;
            var justiceUserOneWorkHours = workHours.SingleOrDefault(x => x.JusticeUserId == justiceUserOne.Id);
            var justiceUserTwoWorkHours = workHours.SingleOrDefault(x => x.JusticeUserId == justiceUserTwo.Id);

            // Assert
            workHours.Count().Should().Be(oldWorkCount + 2);
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

            var requests = new List<UploadWorkHoursRequest> {
                new()
                {
                    Username = allocatedUser1.Username,
                    WorkingHours = new List<WorkingHours>
                    {
                        new WorkingHours(1, 22, 0, 23, 0)
                    }
                },
                new()
                {
                    Username = allocatedUser2.Username,
                    WorkingHours = new List<WorkingHours>
                    {
                        new WorkingHours(1, 22, 0, 23, 0)
                    }
                }
            };
            
            var command = new UploadWorkHoursCommand(requests);
            
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