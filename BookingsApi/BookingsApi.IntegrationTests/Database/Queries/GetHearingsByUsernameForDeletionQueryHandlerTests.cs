using BookingsApi.DAL.Exceptions;
using BookingsApi.DAL.Queries;
using BookingsApi.Domain.Enumerations;
using BookingsApi.Domain.Participants;

namespace BookingsApi.IntegrationTests.Database.Queries
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
            var hearing1 = await Hooks.SeedVideoHearingV2();
            var hearing2 = await Hooks.SeedVideoHearingV2();
            var hearing3 = await Hooks.SeedVideoHearingV2();

            var individual = hearing2.GetParticipants().First(x => x.HearingRole.UserRole.IsIndividual);
            var username = individual.Person.Username;

            var query = new GetHearingsByUsernameForDeletionQuery(username);

            var result = await _handler.Handle(query);
            result.Any().Should().BeTrue();
            result.Exists(x => x.Id == hearing2.Id).Should().BeTrue();
            result.Exists(x => x.Id == hearing1.Id).Should().BeFalse();
            result.Exists(x => x.Id == hearing3.Id).Should().BeFalse();
        }
        

        [Test]
        public void should_throw_person_not_found_exception_if_username_not_found()
        {
            var username = "do_not_exist@hmcts.net";
            var query = new GetHearingsByUsernameForDeletionQuery(username);
            Assert.ThrowsAsync<PersonNotFoundException>(() => _handler.Handle(query));
        }
    }
}