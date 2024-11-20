using BookingsApi.DAL;
using BookingsApi.DAL.Helper;
using BookingsApi.DAL.Queries;
using BookingsApi.Domain;
using BookingsApi.Domain.RefData;
using Microsoft.EntityFrameworkCore;

namespace BookingsApi.UnitTests.DAL.Queries
{
    public class GetAnonymisationDataQueryHandlerTests
    {
        private BookingsDbContext _context;
        private GetAnonymisationDataQueryHandler _handler;
        private UserRole _userRole;
        private HearingRole _hearingRole;
        private Person _person1, _person2, _person3, _person4, _anonymisedPerson;
        private JudiciaryPerson _judiciaryJudge, _judiciaryJoh;
        private CaseType _caseType1;

        [SetUp]
        public async Task Setup()
        {
            var contextOptions = new DbContextOptionsBuilder<BookingsDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;
            _context = new BookingsDbContext(contextOptions);

            _userRole = new UserRole(2, "user role 2");

            _hearingRole = new HearingRole(2, "hearing role 2") {UserRole = _userRole};

            _person1 = new Person(Faker.Name.Suffix(), Faker.Name.First(), Faker.Name.Last(), Faker.Internet.Email(), FakeValidUserName());
            _person2 = new Person(Faker.Name.Suffix(), Faker.Name.First(), Faker.Name.Last(), Faker.Internet.Email(), FakeValidUserName());
            _person3 = new Person(Faker.Name.Suffix(), Faker.Name.First(), Faker.Name.Last(), Faker.Internet.Email(), FakeValidUserName());
            _person4 = new Person(Faker.Name.Suffix(), Faker.Name.First(), Faker.Name.Last(), Faker.Internet.Email(), FakeValidUserName());
            
            _judiciaryJudge = new JudiciaryPersonBuilder("Judge123").Build();
            _judiciaryJoh = new JudiciaryPersonBuilder("Joh123").Build();
            
            _anonymisedPerson =
                new Person(Faker.Name.Suffix(), Faker.Name.First(), Faker.Name.Last(), Faker.Internet.Email(), $"{Faker.Internet.Email()}@email.net");

            _caseType1 = new CaseType(12, "case 1")
            {
                HearingTypes = [new HearingType("Hearing type 1")],
                CaseRoles = []
            };

            await _context.CaseTypes.AddAsync(_caseType1);
            await _context.SaveChangesAsync();

            _handler = new GetAnonymisationDataQueryHandler(_context);
        }

        [TearDown]
        public void TearDown()
        {
            _context.CaseTypes.Remove(_caseType1);
        }

        [Test]
        public async Task GetAnonymisationDataQuery_Returns_List_Of_Usernames_And_Hearing_Ids_For_Hearings_Over_3_Months_Old()
        {
            var venue = new HearingVenue(1, "venue 1");
            await _context.Venues.AddAsync(venue);
            
            var hearing1 = new VideoHearing(_caseType1, DateTime.Today, 40,
                venue, "Room 1", null, Faker.Name.First(), false);

            var hearing2 = new VideoHearing(_caseType1, DateTime.Today, 40,
                venue, "Room 1", null, Faker.Name.First(), false);
            
            hearing1.AddIndividual(Guid.NewGuid().ToString(), _person1, _hearingRole, "Individual 1");
            hearing1.AddIndividual(Guid.NewGuid().ToString(), _person2, _hearingRole, "Individual 123");

            hearing2.AddIndividual(Guid.NewGuid().ToString(), _person3, _hearingRole, "Individual 1");
            hearing2.AddIndividual(Guid.NewGuid().ToString(), _person4, _hearingRole, "Individual 123");

            var videoHearingType = typeof(VideoHearing);
            videoHearingType.GetProperty("ScheduledDateTime").SetValue(hearing1, DateTime.UtcNow.AddMonths(-3));

            await _context.VideoHearings.AddRangeAsync(hearing1, hearing2);
            await _context.SaveChangesAsync();

            var result = await _handler.Handle(new GetAnonymisationDataQuery());

            result.Usernames.Count.Should().Be(2, "hearing 1 has expired and both partcicipants do not have upcoming hearings");
            result.HearingIds.Count.Should().Be(1);

            result.Usernames.Should().Contain(x => x == _person1.Username);
            result.Usernames.Should().Contain(x => x == _person2.Username);
            result.Usernames.Should().NotContain(x => x == _person3.Username);
            result.Usernames.Should().NotContain(x => x == _person4.Username);

            result.HearingIds.Should().Contain(x => x == hearing1.Id);
            result.HearingIds.Should().NotContain(x => x == hearing2.Id);
        }

        [Test]
        public async Task GetAnonymisationDataQuery_Filters_Out_Username_Included_In_Future_Hearing()
        {
            var venue = new HearingVenue(1, "venue 1");
            await _context.Venues.AddAsync(venue);
            
            var hearing1 = new VideoHearing(_caseType1, DateTime.Today, 40,
                venue, "Room 1", null, Faker.Name.First(), false);

            var hearing2 = new VideoHearing(_caseType1, DateTime.Today, 40,
                venue, "Room 1", null, Faker.Name.First(), false);
            
            hearing1.AddJudiciaryJudge(_judiciaryJudge, "Judge 123");
            hearing1.AddIndividual(Guid.NewGuid().ToString(), _person2, _hearingRole, "Individual 123");

            hearing2.AddJudiciaryJudge(_judiciaryJudge, "Judge 123");
            hearing2.AddJudiciaryPanelMember(_judiciaryJoh, "PM 123");
            hearing2.AddIndividual(Guid.NewGuid().ToString(), _person4, _hearingRole, "Individual 123");

            var videoHearingType = typeof(VideoHearing);
            videoHearingType.GetProperty("ScheduledDateTime").SetValue(hearing1, DateTime.UtcNow.AddMonths(-3));

            await _context.VideoHearings.AddRangeAsync(hearing1, hearing2);
            await _context.SaveChangesAsync();

            var result = await _handler.Handle(new GetAnonymisationDataQuery());

            result.Usernames.Count.Should().Be(1);

            result.Usernames.Should().NotContain(x => x == _person1.Username);
            result.Usernames.Should().Contain(x => x == _person2.Username);
            result.Usernames.Should().NotContain(x => x == _person3.Username);
            result.Usernames.Should().NotContain(x => x == _person4.Username);
        }

        [Test]
        public async Task GetAnonymisationDataQuery_Filters_Out_Usernames_And_Hearing_Ids_Already_Processed_By_LastRunDate_Specified_In_JobHistory()
        {
            var jobHistory = new JobHistoryForTest();
            jobHistory.SetLastRunDate(DateTime.Now.AddMonths(-5));
            await _context.JobHistory.AddAsync(jobHistory);

            var venue = new HearingVenue(1, "venue 1");
            await _context.Venues.AddAsync(venue);
            
            var hearing1 = new VideoHearing(_caseType1, DateTime.Today, 40,
                venue, "Room 1", null, Faker.Name.First(), false);

            var hearing2 = new VideoHearing(_caseType1, DateTime.Today, 40,
                venue, "Room 1", null, Faker.Name.First(), false);

            hearing1.AddJudiciaryJudge(_judiciaryJudge, "Judge 123");
            hearing1.AddIndividual(Guid.NewGuid().ToString(), _person2, _hearingRole, "Individual 123");

            hearing2.AddJudiciaryJudge(_judiciaryJudge, "Judge 123");
            hearing2.AddIndividual(Guid.NewGuid().ToString(), _person4, _hearingRole, "Individual 123");

            //setting the scheduled time for a hearing that does not fall in between cut off date and last run date 
            typeof(VideoHearing).GetProperty("ScheduledDateTime")?.SetValue(hearing1, DateTime.UtcNow.AddMonths(-10)); 
            await _context.VideoHearings.AddRangeAsync(hearing1, hearing2);
            await _context.SaveChangesAsync();

            var result = await _handler.Handle(new GetAnonymisationDataQuery());

            result.Usernames.Count.Should().Be(0);
            result.HearingIds.Count.Should().Be(0);

            _context.JobHistory.Remove(jobHistory);
        }
        
        [Test]
        public async Task GetAnonymisationDataQuery_since_the_begining_of_time_when_no_successful_JobHistory_dates_to_run_from()
        {
            var jobHistory = new JobHistoryForTest();
            jobHistory.SetLastRunDateFalse();
            await _context.JobHistory.AddAsync(jobHistory);


            var venue = new HearingVenue(1, "venue 1");
            await _context.Venues.AddAsync(venue);
            
            var hearing1 = new VideoHearing(_caseType1, DateTime.Today, 40,
                venue, "Room 1", null, Faker.Name.First(), false);

            var hearing2 = new VideoHearing(_caseType1, DateTime.Today, 40,
                venue, "Room 1", null, Faker.Name.First(), false);

            hearing1.AddIndividual(Guid.NewGuid().ToString(), _person1, _hearingRole, "Individual 1");
            hearing1.AddIndividual(Guid.NewGuid().ToString(), _person2, _hearingRole, "Individual 123");

            hearing2.AddIndividual(Guid.NewGuid().ToString(), _person3, _hearingRole, "Individual 3");
            hearing2.AddIndividual(Guid.NewGuid().ToString(), _person4, _hearingRole, "Individual 123");

            typeof(VideoHearing).GetProperty("ScheduledDateTime")?.SetValue(hearing1, DateTime.UtcNow.AddMonths(-24)); 
            //Not valid because still within 3 month cut off period
            typeof(VideoHearing).GetProperty("ScheduledDateTime")?.SetValue(hearing2, DateTime.UtcNow.AddMonths(-1)); 
            await _context.VideoHearings.AddRangeAsync(hearing1, hearing2);
            await _context.SaveChangesAsync();

            var result = await _handler.Handle(new GetAnonymisationDataQuery());

            result.Usernames.Count.Should().Be(2);
            result.HearingIds.Count.Should().Be(1);

            _context.JobHistory.Remove(jobHistory);
        }

        [Test]
        public async Task GetAnonymisationDataQuery_Returns_List_Of_Usernames_And_Hearing_Ids_For_Hearings_Over_3_Months_Old_And_Are_Unique()
        {
            var venue = new HearingVenue(1, "venue 1");
            await _context.Venues.AddAsync(venue);
            
            var hearing1 = new VideoHearing(_caseType1, DateTime.Today, 40,
                venue, "Room 1", null, Faker.Name.First(), false);

            var hearing2 = new VideoHearing(_caseType1, DateTime.Today, 40,
                venue, "Room 1", null, Faker.Name.First(), false);
            
            var hearing3 = new VideoHearing(_caseType1, DateTime.Today, 40,
                venue, "Room 1", null, Faker.Name.First(), false);

            
            hearing1.AddIndividual(Guid.NewGuid().ToString(), _person1, _hearingRole, "Individual 1");
            hearing1.AddIndividual(Guid.NewGuid().ToString(), _person2, _hearingRole, "Individual 123");

            hearing2.AddIndividual(Guid.NewGuid().ToString(), _person3, _hearingRole, "Individual 3");
            hearing2.AddIndividual(Guid.NewGuid().ToString(), _person4, _hearingRole, "Individual 123");
            
            hearing3.AddIndividual(Guid.NewGuid().ToString(), _person1, _hearingRole, "Individual 1");
            hearing3.AddIndividual(Guid.NewGuid().ToString(), _person2, _hearingRole, "Individual 123");
            
            var videoHearingType = typeof(VideoHearing);
            videoHearingType.GetProperty("ScheduledDateTime").SetValue(hearing1, DateTime.UtcNow.AddMonths(-3));
            videoHearingType.GetProperty("ScheduledDateTime").SetValue(hearing3, DateTime.UtcNow.AddMonths(-3));

            await _context.VideoHearings.AddRangeAsync(hearing1, hearing2, hearing3);
            await _context.SaveChangesAsync();

            var result = await _handler.Handle(new GetAnonymisationDataQuery());

            result.Usernames.Count.Should().Be(2);
            result.HearingIds.Count.Should().Be(2);

            result.Usernames.Should().Contain(x => x == _person1.Username);
            result.Usernames.Should().Contain(x => x == _person2.Username);
            result.Usernames.Should().NotContain(x => x == _person3.Username);
            result.Usernames.Should().NotContain(x => x == _person4.Username);

            result.HearingIds.Should().Contain(x => x == hearing1.Id);
            result.HearingIds.Should().NotContain(x => x == hearing2.Id);
            result.HearingIds.Should().Contain(x => x == hearing3.Id);
        }

        [Test]
        public async Task GetAnonymisationDataQuery_Filters_Out_Anonymised_Usernames()
        {
            var venue = new HearingVenue(1, "venue 1");
            await _context.Venues.AddAsync(venue);
            
            var hearing1 = new VideoHearing(_caseType1, DateTime.Today, 40,
                venue, "Room 1", null, Faker.Name.First(), false);

            var hearing2 = new VideoHearing(_caseType1, DateTime.Today, 40,
                venue, "Room 1", null, Faker.Name.First(), false);
            
            hearing1.AddIndividual(Guid.NewGuid().ToString(), _person1, _hearingRole, "Individual 1");
            hearing1.AddIndividual(Guid.NewGuid().ToString(), _person2, _hearingRole, "Individual 123");
            hearing1.AddIndividual(Guid.NewGuid().ToString(), _anonymisedPerson, _hearingRole, "Individual 123");

            hearing2.AddJudiciaryJudge(_judiciaryJudge, "Judge 123");
            hearing2.AddIndividual(Guid.NewGuid().ToString(), _person4, _hearingRole, "Individual 123");

            var videoHearingType = typeof(VideoHearing);
            videoHearingType.GetProperty("ScheduledDateTime").SetValue(hearing1, DateTime.UtcNow.AddMonths(-3));

            await _context.VideoHearings.AddRangeAsync(hearing1, hearing2);
            await _context.SaveChangesAsync();

            var result = await _handler.Handle(new GetAnonymisationDataQuery());

            result.Usernames.Count.Should().Be(2, "because hearing 1 has expired and 2 users are not anonymised");

            result.Usernames.Should().Contain(x => x == _person1.Username);
            result.Usernames.Should().Contain(x => x == _person2.Username);
            result.Usernames.Should().NotContain(x => x == _person3.Username);
            result.Usernames.Should().NotContain(x => x == _person4.Username);
            result.Usernames.Should().NotContain(x => x == _anonymisedPerson.Username);
        }

        [Test]
        [TestCase("auto_vw.individual_16@hearings.reform.hmcts.net")]
        [TestCase("auto_aw.judge_03@hearings.reform.hmcts.net")]
        [TestCase("auto_tw.individual_07@hearings.reform.hmcts.net")]
        [TestCase("auto_sw.individual_02@hearings.reform.hmcts.net")]
        [TestCase("auto_sw.judge_04@hearings.reform.hmcts.net")]
        public async Task Filters_Out_Automation_Test_Account_Usernames(string username)
        {
            var venue = new HearingVenue(1, "venue 1");
            await _context.Venues.AddAsync(venue);
            
            var hearing1 = new VideoHearing(_caseType1, DateTime.Today, 40,
                venue, "Room 1", null, Faker.Name.First(), false);

            var hearing2 = new VideoHearing(_caseType1, DateTime.Today, 40,
                venue, "Room 1", null, Faker.Name.First(), false);
            var automationTestAccountPersonEntry = new Person(Faker.Name.Suffix(), Faker.Name.First(), Faker.Name.Last(), Faker.Internet.Email(), username);

            hearing1.AddIndividual(Guid.NewGuid().ToString(), _person1, _hearingRole, "Individual 1");
            hearing1.AddIndividual(Guid.NewGuid().ToString(), _person2, _hearingRole, "Individual 123");
            hearing1.AddIndividual(Guid.NewGuid().ToString(), automationTestAccountPersonEntry, _hearingRole, "Individual 123");

            hearing2.AddJudiciaryJudge(_judiciaryJudge, "Judge 123");
            hearing2.AddIndividual(Guid.NewGuid().ToString(), _person4, _hearingRole, "Individual 123");

            typeof(VideoHearing).GetProperty("ScheduledDateTime")?.SetValue(hearing1, DateTime.UtcNow.AddMonths(-3));

            await _context.VideoHearings.AddRangeAsync(hearing1, hearing2);
            await _context.SaveChangesAsync();

            var result = await _handler.Handle(new GetAnonymisationDataQuery());

            result.Usernames.Count.Should().Be(2, "because hearing 1 has expired and 2 users are not automation test accounts");

            result.Usernames.Should().Contain(x => x == _person1.Username);
            result.Usernames.Should().Contain(x => x == _person2.Username);
            result.Usernames.Should().NotContain(x => x == _person3.Username);
            result.Usernames.Should().NotContain(x => x == _person4.Username);
            result.Usernames.Should().NotContain(x => x == automationTestAccountPersonEntry.Username);
        }

        private class JobHistoryForTest : JobHistory
        {
            public void SetLastRunDate(DateTime lastRunDate)
            {
                LastRunDate = lastRunDate;
                IsSuccessful = true;
                JobName = SchedulerJobsNames.AnonymiseHearings;
            }

            public void SetLastRunDateFalse()
            {
                LastRunDate = DateTime.Now;
                IsSuccessful = false;
                JobName = SchedulerJobsNames.AnonymiseHearings;
            }
        }
        
        private static string FakeValidUserName() => $"{Faker.Internet.UserName()}@hearings.reform.hmcts.net";
    }
}