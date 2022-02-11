using System;
using System.Linq;
using System.Threading.Tasks;
using BookingsApi.DAL;
using BookingsApi.DAL.Queries;
using FluentAssertions;
using NUnit.Framework;

namespace BookingsApi.IntegrationTests.Database.Queries
{
    public class GetAnonymisationDataQueryHandlerTests : DatabaseTestsBase
    {
        private GetAnonymisationDataQueryHandler _handler;

        [SetUp]
        public void Setup()
        {
            var context = new BookingsDbContext(BookingsDbContextOptions);
            _handler = new GetAnonymisationDataQueryHandler(context);
        }

        [Test]
        public async Task Returns_Anonymisation_Data_For_3_Months_Old_Hearings()
        {
            var lastRunDate = Hooks.GetJobLastRunDateTime();
            var cutOffDate = DateTime.UtcNow.AddMonths(-3);
            var scheduledDate = lastRunDate.HasValue
                ? cutOffDate.AddDays((lastRunDate.Value - DateTime.UtcNow).Days - 1).AddMinutes(10)
                : cutOffDate.AddDays(-5);
            var oldHearings = await Hooks.SeedPastHearings(scheduledDate);

            TestContext.WriteLine($"New seeded video hearing id: {oldHearings.Id}");

            var listOfData = await _handler.Handle(new GetAnonymisationDataQuery());

            listOfData.Usernames.Count.Should().Be(oldHearings.Participants.Count);
            listOfData.HearingIds.Count.Should().Be(1);
            listOfData.HearingIds.FirstOrDefault().Should().Be(oldHearings.Id);
        }

        [Test]
        [TestCase(1)]
        [TestCase(2)]
        [TestCase(-1)]
        [TestCase(-2)]
        public async Task Returns_Empty_List_Of_Anonymisation_Data(int cutOffInMonths)
        {
            var lastRunDate = Hooks.GetJobLastRunDateTime();
            var cutOffDate = DateTime.UtcNow.AddMonths(cutOffInMonths);
            var scheduledDate = lastRunDate.HasValue
                ? cutOffDate.AddDays((lastRunDate.Value - DateTime.UtcNow).Days - 1).AddMinutes(10)
                : cutOffDate.AddDays(-5);
            var oldHearings = await Hooks.SeedPastHearings(scheduledDate);

            TestContext.WriteLine($"New seeded video hearing id: {oldHearings.Id}");
            
            var listofData = await _handler.Handle(new GetAnonymisationDataQuery());

            listofData.Usernames.Count.Should().Be(0);
            listofData.HearingIds.Count.Should().Be(0);
        }
    }
}