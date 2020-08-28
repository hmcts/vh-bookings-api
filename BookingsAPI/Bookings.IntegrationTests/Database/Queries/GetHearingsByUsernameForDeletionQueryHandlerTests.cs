using System.Linq;
using System.Threading.Tasks;
using Bookings.DAL;
using Bookings.DAL.Queries;
using FluentAssertions;
using NUnit.Framework;

namespace Bookings.IntegrationTests.Database.Queries
{
    public class GetHearingsByUsernameForDeletionQueryHandlerTests : DatabaseTestsBase
    {
        private GetHearingsByUsernameForDeletionQueryHandler _handler;

        [SetUp]
        public void Setup()
        {
            var context = new BookingsDbContext(BookingsDbContextOptions);
            _handler = new GetHearingsByUsernameForDeletionQueryHandler(context);
        }

        [Test]
        public async Task Should_return_hearings_for_username()
        {
            var hearing1 = await Hooks.SeedVideoHearing();
            var hearing2 = await Hooks.SeedVideoHearing();
            var hearing3 = await Hooks.SeedVideoHearing();

            var username = hearing2.GetPersons().First().Username;

            var query = new GetHearingsByUsernameForDeletionQuery(username);

            var result = await _handler.Handle(query);
            result.Any().Should().BeTrue();
            result.Any(x => x.Id == hearing2.Id).Should().BeTrue();
            result.Any(x => x.Id == hearing1.Id).Should().BeFalse();
            result.Any(x => x.Id == hearing3.Id).Should().BeFalse();

            result.SelectMany(x => x.GetParticipants()).Any(x => x.HearingRole.UserRole.IsJudge).Should().BeFalse();
            foreach (var hearing in result)
            {
                hearing.GetParticipants().Count.Should().Be(1);
                hearing.GetCases().First().Should().NotBeNull();
            }
        }
        
        [Test]
        public async Task Should_return_no_hearings_for_judge_username()
        {
            var hearing = await Hooks.SeedVideoHearing();
            await Hooks.SeedVideoHearing();
            await Hooks.SeedVideoHearing();

            var username = hearing.GetParticipants().First(x => x.HearingRole.UserRole.IsJudge).Person.Username;

            var query = new GetHearingsByUsernameForDeletionQuery(username);

            var result = await _handler.Handle(query);
            result.Any().Should().BeFalse();
        }
    }
}