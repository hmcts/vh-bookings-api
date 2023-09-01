using BookingsApi.DAL.Exceptions;
using BookingsApi.DAL.Queries;

namespace BookingsApi.IntegrationTests.Database.Queries
{
    public class GetVhoNonAvailabilityWorkHoursQueryHandlerDatabaseTests : DatabaseTestsBase
    {
        private GetVhoNonAvailableWorkHoursQueryHandler _handler;
        

        [SetUp]
        public void Setup()
        {
            var context = new BookingsDbContext(BookingsDbContextOptions);
            _handler = new GetVhoNonAvailableWorkHoursQueryHandler(context);
        }

        [Test]
        public async Task Should_throw_exception_when_no_user_is_found()
        {
            // arrange
            var query = new GetVhoNonAvailableWorkHoursQuery("doesnt.existatall@hmcts.net");

            // act / assert
            var action = async() => await _handler.Handle(query);
            await action.Should().ThrowAsync<JusticeUserNotFoundException>()
                .WithMessage($"Justice user with username {query.UserName} not found");
        }
        
        [Test]
        public async Task Should_return_VhoWorkHours_when_that_user_exists()
        {
            // arrange
            var username = "UserWithRecords@test.com";
            var date = DateTime.UtcNow.Date;
            await using var context = new BookingsDbContext(BookingsDbContextOptions);
            var justiceUser = await Hooks.SeedJusticeUser(username, "withrecords", "withrecords", initWorkHours: false);
            context.Attach(justiceUser);
            justiceUser.AddOrUpdateNonAvailability(date, date);
            justiceUser.AddOrUpdateNonAvailability(date.AddDays(1), date.AddDays(1).AddHours(10));
            justiceUser.AddOrUpdateNonAvailability(date.AddDays(2), date.AddDays(2).AddHours(10));

            var nonAvailable1 = justiceUser.VhoNonAvailability[0];
            var nonAvailable2 = justiceUser.VhoNonAvailability[1];
            var nonAvailable3 = justiceUser.VhoNonAvailability[2];
            
            justiceUser.VhoNonAvailability[0].Deleted = true;

            
            await context.SaveChangesAsync();
            
            // act
            var query = new GetVhoNonAvailableWorkHoursQuery(username);
            var vhoWorkHours = await _handler.Handle(query);
            
            // assert
            vhoWorkHours.Should().NotBeNullOrEmpty();
            vhoWorkHours.Count.Should().Be(2);
            
            vhoWorkHours.Should().NotContain(x => x.Id == nonAvailable1.Id);
            
            vhoWorkHours[0].Id.Should().Be(nonAvailable2.Id);
            vhoWorkHours[0].StartTime.Should().Be(nonAvailable2.StartTime);
            vhoWorkHours[0].EndTime.Should().Be(nonAvailable2.EndTime);
            
            vhoWorkHours[1].Id.Should().Be(nonAvailable3.Id);
            vhoWorkHours[1].StartTime.Should().Be(nonAvailable3.StartTime);
            vhoWorkHours[1].EndTime.Should().Be(nonAvailable3.EndTime);
            
            
        }
        
        [Test]
        public async Task Should_return_empty_list_when_user_exists_but_not_work_hours_exist()
        {
            // arrange
            var username = "UserWithoutRecords@test.com";
            var justiceUser = await Hooks.SeedJusticeUser(username, "UserWithoutRecords", "UserWithoutRecords", initWorkHours: false);
            
            // act
            var query = new GetVhoNonAvailableWorkHoursQuery(username);
            var vhoWorkHours = await _handler.Handle(query);
            
            // assert
            vhoWorkHours.Should().BeEmpty();
        }
    }
}