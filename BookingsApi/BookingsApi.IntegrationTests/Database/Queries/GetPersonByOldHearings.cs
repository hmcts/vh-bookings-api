﻿using System;
using System.Threading.Tasks;
using BookingsApi.DAL;
using BookingsApi.DAL.Queries;
using FluentAssertions;
using NUnit.Framework;

namespace BookingsApi.IntegrationTests.Database.Queries
{
    public class GetPersonByOldHearings : DatabaseTestsBase
    {
        private GetPersonsByClosedHearingsQueryHandler _handler;

        [SetUp]
        public void Setup()
        {
            var context = new BookingsDbContext(BookingsDbContextOptions);
            _handler = new GetPersonsByClosedHearingsQueryHandler(context);
        }

        [Test]
        public async Task Should_return_list_of_user_names_for_old_hearings_over_3_months_old()
        {
            
            var lastRunDate = Hooks.GetJobLastRunDateTime();
            var cutOffDate = DateTime.UtcNow.AddMonths(-3);
            var scheduledDate = lastRunDate.HasValue 
                             ? cutOffDate.AddDays((lastRunDate.Value - DateTime.UtcNow).Days - 1).AddMinutes(10) 
                             : cutOffDate.AddDays(-5);
            var oldHearings = await Hooks.SeedPastHearings(scheduledDate);
            TestContext.WriteLine($"New seeded video hearing id: { oldHearings.Id }");

            var usernamesList = await _handler.Handle(new GetPersonsByClosedHearingsQuery());
            usernamesList.Should().NotBeNull();
            usernamesList.Count.Should().Be(4); // Individual & Representative participants + JOH participants
        }
    }
}
