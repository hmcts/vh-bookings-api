using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Bookings.Domain;
using BookingsApi.DAL;
using BookingsApi.DAL.Commands;
using BookingsApi.DAL.Exceptions;
using BookingsApi.DAL.Queries;
using FluentAssertions;
using NUnit.Framework;

namespace Bookings.IntegrationTests.Database.Commands
{
    public class AddCasesToHearingCommandTests : DatabaseTestsBase
    {
        private AddCasesToHearingCommandHandler _commandHandler;
        private GetHearingByIdQueryHandler _getHearingByIdQueryHandler;
        
        [SetUp]
        public void Setup()
        {
            var context = new BookingsDbContext(BookingsDbContextOptions);
            _commandHandler = new AddCasesToHearingCommandHandler(context);
            _getHearingByIdQueryHandler = new GetHearingByIdQueryHandler(context);
        }
        
        [Test]
        public void Should_throw_exception_when_hearing_does_not_exist()
        {
            var hearingId = Guid.NewGuid();

            Assert.ThrowsAsync<HearingNotFoundException>(() => _commandHandler.Handle(
                new AddCasesToHearingCommand(hearingId, new List<Case>())));
        }
        
        [Test]
        public async Task Should_add_cases_to_video_hearing()
        {
            var seededHearing = await Hooks.SeedVideoHearing();
            TestContext.WriteLine($"New seeded video hearing id: {seededHearing.Id}");

            var beforeCount = seededHearing.GetCases().Count;

            var cases = new List<Case> {new Case("01234567890", "Test Add")};
            await _commandHandler.Handle(new AddCasesToHearingCommand(seededHearing.Id, cases));

            var returnedVideoHearing =
                await _getHearingByIdQueryHandler.Handle(new GetHearingByIdQuery(seededHearing.Id));
            var afterCount = returnedVideoHearing.GetCases().Count;

            afterCount.Should().BeGreaterThan(beforeCount);
        }

        [TearDown]
        public new async Task TearDown()
        {
            await Hooks.ClearSeededHearings();
        }
    }
}