using BookingsApi.Contract.Requests;
using BookingsApi.DAL;
using BookingsApi.DAL.Commands;
using FluentAssertions;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookingsApi.IntegrationTests.Database.Commands
{
    public class UploadNonWorkingHoursCommandTests : DatabaseTestsBase
    {
        private UploadNonWorkingHoursCommandHandler _commandHandler;
        private BookingsDbContext _context;

        [SetUp]
        public void Setup()
        {
            _context = new BookingsDbContext(BookingsDbContextOptions);
            _commandHandler = new UploadNonWorkingHoursCommandHandler(_context);
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

            var justiceUserOneNonWorkingHours = 
                new NonWorkingHours(new DateTime(2022, 2, 1), new DateTime(2022, 1, 1));
            var justiceUserTwoNonWorkingHours = 
                new NonWorkingHours(new DateTime(2022, 2, 10, 9, 0, 0), new DateTime(2022, 2, 11, 16, 30, 0));

            var requests = new List<UploadNonWorkingHoursRequest> {
                new UploadNonWorkingHoursRequest(
                    justiceUserOne.Username,
                    justiceUserOneNonWorkingHours.StartTime,
                    justiceUserOneNonWorkingHours.EndTime
                ),
                new UploadNonWorkingHoursRequest(
                    justiceUserTwo.Username,
                    justiceUserTwoNonWorkingHours.StartTime,
                    justiceUserTwoNonWorkingHours.EndTime
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
            justiceUserOneNonWorkHours.StartTime.Should().Be(justiceUserOneNonWorkingHours.StartTime);
            justiceUserOneNonWorkHours.EndTime.Should().Be(justiceUserOneNonWorkingHours.EndTime);
            justiceUserTwoNonWorkHours.StartTime.Should().Be(justiceUserTwoNonWorkingHours.StartTime);
            justiceUserTwoNonWorkHours.EndTime.Should().Be(new DateTime(2022, 2, 11, 17, 30, 0));
        }
    }
}