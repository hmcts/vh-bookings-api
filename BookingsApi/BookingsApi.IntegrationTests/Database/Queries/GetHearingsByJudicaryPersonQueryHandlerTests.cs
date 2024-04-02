using BookingsApi.DAL.Exceptions;
using BookingsApi.DAL.Queries;
using BookingsApi.Domain.Enumerations;
using BookingsApi.Domain.JudiciaryParticipants;

namespace BookingsApi.IntegrationTests.Database.Queries
{
    public class GetHearingsByJudiciaryPersonQueryHandlerTests : DatabaseTestsBase
    {
        private GetHearingsByJudiciaryPersonQueryHandler _handler;
        private VideoHearing _hearing1;
        private VideoHearing _hearing2;
        private VideoHearing _hearing3;
        private VideoHearing _hearing4;
        private VideoHearing _hearing5;
        private VideoHearing _hearing6;
        private JudiciaryPerson _validJudiciaryPerson;
        private JudiciaryPerson _nonValidJudiciaryPerson;
        
        Action<SeedVideoHearingOptions> _seedOptions = options =>
        {
            options.ScheduledDate = DateTime.Now;
            options.AddJudge = false;
        };


        [SetUp]
        public async Task Setup()
        {
            var context = new BookingsDbContext(BookingsDbContextOptions);
            
            _validJudiciaryPerson = await context.JudiciaryPersons
                .Where(x => x.Email.ToLower().Contains("Automation_".ToLower()) && !x.HasLeft)
                .FirstAsync();
            
            _nonValidJudiciaryPerson = await context.JudiciaryPersons
                .Where(x => x != _validJudiciaryPerson)
                .FirstAsync();
            
            _handler = new GetHearingsByJudiciaryPersonQueryHandler(context);
            //hearing1: Invalid - wrong status
            _hearing1 = await Hooks.SeedVideoHearingV2(_seedOptions); //booked
            //hearing2: Invalid - wrong date
            _hearing2 = await Hooks.SeedVideoHearingV2(options => { options.ScheduledDate = DateTime.Now.AddDays(1); }, BookingStatus.Created);
            //hearing3: valid - (but wont match search query)
            _hearing3 = await Hooks.SeedVideoHearingV2(_seedOptions, BookingStatus.Created);
            //hearing4: valid
            _hearing4 = await Hooks.SeedVideoHearingV2(_seedOptions, BookingStatus.Created);
            //hearing5: Valid - Judiciary Panel Member
            _hearing5 = await Hooks.SeedVideoHearingV2(_seedOptions, BookingStatus.Created);
            //hearing6: Valid - V1 booked hearing
            _hearing6 = await Hooks.SeedVideoHearing(_seedOptions, BookingStatus.Created);
            
            await Hooks.AddJudiciaryJudge(_hearing1, _validJudiciaryPerson, "hearingJudge1");
            await Hooks.AddJudiciaryJudge(_hearing2, _validJudiciaryPerson, "hearingJudge2");
            await Hooks.AddJudiciaryJudge(_hearing4, _validJudiciaryPerson, "hearingJudge4");
            await Hooks.AddJudiciaryJudge(_hearing5, _validJudiciaryPerson, "hearingJudge5");
            await Hooks.AddJudiciaryJudge(_hearing6, _validJudiciaryPerson, "hearingJudge6");
            await context.SaveChangesAsync();
        }

        [Test]
        public async Task Should_return_hearings_for_username()
        {
            var result = await _handler.Handle(new GetHearingsByJudiciaryPersonQuery(_validJudiciaryPerson.Email));
            result.Any().Should().BeTrue();
            result.Exists(x => x.Id == _hearing1.Id).Should().BeFalse();
            result.Exists(x => x.Id == _hearing2.Id).Should().BeFalse();
            result.Exists(x => x.Id == _hearing3.Id).Should().BeFalse();
            result.Exists(x => x.Id == _hearing4.Id).Should().BeTrue();
            result.Exists(x => x.Id == _hearing5.Id).Should().BeTrue();
            result.Exists(x => x.Id == _hearing6.Id).Should().BeTrue();
        }
        
        [Test]
        public async Task Should_not_return_any_hearings()
        {
            var result = await _handler.Handle(new GetHearingsByJudiciaryPersonQuery(_nonValidJudiciaryPerson.Email));
            result.Should().NotBeNull();
            result.Any().Should().BeFalse();
        }
        
        [Test]
        public async Task Should_throw_exception_for_invalid_judiciary_person()
        {
            var query = new GetHearingsByJudiciaryPersonQuery("Random email");
            Func<Task> action = async () => await _handler.Handle(query);
            await action.Should().ThrowAsync<JudiciaryPersonNotFoundException>();
        }
    }
}