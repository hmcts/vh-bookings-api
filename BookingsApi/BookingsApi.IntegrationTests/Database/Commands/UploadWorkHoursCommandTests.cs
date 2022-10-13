using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BookingsApi.Contract.Requests;
using BookingsApi.DAL;
using BookingsApi.DAL.Commands;
using BookingsApi.DAL.Dtos;
using BookingsApi.DAL.Queries;
using BookingsApi.DAL.Services;
using BookingsApi.Domain;
using BookingsApi.Domain.Enumerations;
using BookingsApi.Domain.RefData;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using Testing.Common.Builders.Domain;

namespace BookingsApi.IntegrationTests.Database.Commands
{
    public class UploadWorkHoursCommandTests : DatabaseTestsBase
    {
        private UploadWorkHoursCommandHandler _commandHandler;
        private BookingsDbContext _context;

        [SetUp]
        public void Setup()
        {
            _context = new BookingsDbContext(BookingsDbContextOptions);
            _commandHandler = new UploadWorkHoursCommandHandler(_context);
        }

        [Test]
        public async Task Should_return_username_if_it_doesnt_exist()
        {
            // Arrange
            var username = "dontexist@test.com";
            var requests = new List<UploadWorkAllocationRequest> {
                new UploadWorkAllocationRequest
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

            var requests = new List<UploadWorkAllocationRequest> {
                new UploadWorkAllocationRequest
                {
                    Username = justiceUserOne.Username,
                    WorkingHours = new List<WorkingHours>
                    {
                        new WorkingHours(1, 9, 0, 17, 0)
                    }
                },
                new UploadWorkAllocationRequest
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
    }
}