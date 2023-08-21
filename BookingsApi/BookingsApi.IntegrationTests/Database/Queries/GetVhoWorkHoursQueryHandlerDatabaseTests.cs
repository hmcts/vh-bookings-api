using BookingsApi.DAL.Queries;

namespace BookingsApi.IntegrationTests.Database.Queries
{
    public class GetVhoWorkHoursQueryHandlerDatabaseTests : DatabaseTestsBase
    {
        private GetVhoWorkHoursQueryHandler _handler;
        private const string UserWithRecords = "test.Integration.GetVhoWorkHoursQueryHandlerDatabase1@hearings.reform.hmcts.net";
        private const string UserWithoutRecords = "test.Integration.GetVhoWorkHoursQueryHandlerDatabase2@hearings.reform.hmcts.net";
        private const string DeletedUser = "test.Integration.GetVhoWorkHoursQueryHandlerDatabase3@hearings.reform.hmcts.net";
        private const string UserWithDeletedRecords = "test.Integration.GetVhoWorkHoursQueryHandlerDatabase4@hearings.reform.hmcts.net";

        [SetUp]
        public async Task Setup()
        {
            await Hooks.SeedJusticeUser(UserWithRecords, "withrecords", "withrecords", initWorkHours: true);
            await Hooks.SeedJusticeUser(UserWithoutRecords, "withoutrecords", "withoutrecords", initWorkHours: false);
            await Hooks.SeedJusticeUser(DeletedUser, "deletedWithRecordsUser", "deletedWithRecordsUser", isDeleted: true,
                    initWorkHours: true);
            await Hooks.SeedJusticeUser(UserWithDeletedRecords, "deletedUserWithRecordsThenRestored", "deletedUserWithRecordsThenRestored", isDeleted: true,
                    initWorkHours: true);
            
            var context = new BookingsDbContext(BookingsDbContextOptions);
            _handler = new GetVhoWorkHoursQueryHandler(context);
        }

        [Test]
        public async Task Should_return_empty_list_when_no_user_is_found()
        {
            var query = new GetVhoWorkHoursQuery("doesnt.existatall@hmcts.net");
            var vhoWorkHours = await _handler.Handle(query);
            vhoWorkHours.Should().BeNull();
        }
        
        [Test]
        public async Task Should_return_VhoWorkHours_when_that_user_exists()
        {
            var query = new GetVhoWorkHoursQuery(UserWithRecords);
            var vhoWorkHours = await _handler.Handle(query);

            vhoWorkHours.Should().NotBeNull();
            vhoWorkHours[0].JusticeUser.Username.Should().Be(UserWithRecords);
            vhoWorkHours.Count.Should().Be(7);
        }
        
        [Test]
        public async Task Should_return_empty_list_when_user_exists_but_not_work_hours_exist()
        {
            var query        = new GetVhoWorkHoursQuery(UserWithoutRecords);
            var vhoWorkHours = await _handler.Handle(query);
            vhoWorkHours.Should().BeEmpty();
        }

        [Test]
        public async Task Should_return_null_when_user_is_deleted()
        {
            var query = new GetVhoWorkHoursQuery(DeletedUser);
            var vhoWorkHours = await _handler.Handle(query);
            vhoWorkHours.Should().BeNull();
        }

        [Test]
        public async Task Should_not_return_deleted_work_hours()
        {
            await using var context = new BookingsDbContext(BookingsDbContextOptions);
            var justiceUser = await context.JusticeUsers.IgnoreQueryFilters().FirstAsync(x => x.Username == UserWithDeletedRecords);
            justiceUser.Restore();
            await context.SaveChangesAsync();
            
            var query = new GetVhoWorkHoursQuery(UserWithDeletedRecords);
            var vhoWorkHours = await _handler.Handle(query);
            vhoWorkHours.Should().BeEmpty();
        }
    }
}