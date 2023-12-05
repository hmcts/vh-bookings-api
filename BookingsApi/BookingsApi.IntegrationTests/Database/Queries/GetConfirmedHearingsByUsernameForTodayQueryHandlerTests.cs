using BookingsApi.DAL.Queries;
using BookingsApi.Domain.Enumerations;
using BookingsApi.Domain.Participants;

namespace BookingsApi.IntegrationTests.Database.Queries
{
    public class GetConfirmedHearingsByUsernameForTodayQueryHandlerTests : DatabaseTestsBase
    {
        private GetConfirmedHearingsByUsernameForTodayQueryHandler _handler;
        
        [SetUp]
        public void Setup()
        {
            var context = new BookingsDbContext(BookingsDbContextOptions);
            _handler = new GetConfirmedHearingsByUsernameForTodayQueryHandler(context);
        }

        [Test]
        public async Task Should_return_all_confirmed_hearings_for_username_for_today()
        {
            var hearing1 = await Hooks.SeedVideoHearing(status: BookingStatus.Created,
                    configureOptions: options => { options.ScheduledDate = System.DateTime.UtcNow; });
            await Hooks.CloneVideoHearing(hearing1.Id, new List<System.DateTime> { System.DateTime.UtcNow}, BookingStatus.Created);
            await Hooks.CloneVideoHearing(hearing1.Id, new List<System.DateTime> { System.DateTime.UtcNow }, BookingStatus.Created);
            await Hooks.CloneVideoHearing(hearing1.Id, new List<System.DateTime> { System.DateTime.UtcNow.AddDays(1) }, BookingStatus.Created);
            var username = hearing1.Participants.First().Person.Username;

            var query = new GetConfirmedHearingsByUsernameForTodayQuery(username);

            var result = await _handler.Handle(query);
            result.Count.Should().Be(3);
        }

        [Test]
        public async Task Should_not_return_not_confirmed_hearings_for_username_for_today()
        {
            var hearing1 = await Hooks.SeedVideoHearing(status: BookingStatus.Booked);
            await Hooks.CloneVideoHearing(hearing1.Id, new List<System.DateTime> { System.DateTime.UtcNow });

            var username = hearing1.GetPersons().First().Username;

            var query = new GetConfirmedHearingsByUsernameForTodayQuery(username);

            var result = await _handler.Handle(query);
            result.Any().Should().BeFalse();
            result.Count.Should().Be(0);
        }

        [Test]
        public async Task Should_return_confirmed_hearings_for_judiciary_participant_judge_username_for_today()
        {
            var hearing = await Hooks.SeedVideoHearingV2(
                status: BookingStatus.Created,
                configureOptions: options =>
                {
                    options.AddJudge = true;
                    options.ScheduledDate = DateTime.UtcNow;
                });
            
            var username = hearing.GetJudiciaryParticipants()
                .First(p => p.HearingRoleCode == JudiciaryParticipantHearingRoleCode.Judge)
                .JudiciaryPerson
                .Email;

            var query = new GetConfirmedHearingsByUsernameForTodayQuery(username);
            
            var result = await _handler.Handle(query);
            result.Count.Should().Be(1);
        }
        
        [Test]
        public async Task Should_return_confirmed_hearings_for_judiciary_participant_panel_member_username_for_today()
        {
            var hearing = await Hooks.SeedVideoHearingV2(
                status: BookingStatus.Created,
                configureOptions: options =>
                {
                    options.AddJudge = true;
                    options.AddPanelMember = true;
                    options.ScheduledDate = DateTime.UtcNow;
                });
            
            var username = hearing.GetJudiciaryParticipants()
                .First(p => p.HearingRoleCode == JudiciaryParticipantHearingRoleCode.PanelMember)
                .JudiciaryPerson
                .Email;

            var query = new GetConfirmedHearingsByUsernameForTodayQuery(username);
            
            var result = await _handler.Handle(query);
            result.Count.Should().Be(1);
        }
    }
}