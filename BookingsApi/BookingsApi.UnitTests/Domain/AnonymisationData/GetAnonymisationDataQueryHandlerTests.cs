using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BookingsApi.DAL;
using BookingsApi.DAL.Queries;
using BookingsApi.Domain;
using BookingsApi.Domain.RefData;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;

namespace BookingsApi.UnitTests.Domain.AnonymisationData
{
    public class GetAnonymisationDataQueryHandlerTests
    {
        private BookingsDbContext _context;
        private GetAnonymisationDataQueryHandler _handler;
        private UserRole _userRole1, _userRole2;
        private HearingRole _hearingRole1, _hearingRole2;
        private Person _person1, _person2, _person3, _person4, _anonymisedPerson;
        private CaseType _caseType1;

        [OneTimeSetUp]
        public void InitialSetup()
        {
            var contextOptions = new DbContextOptionsBuilder<BookingsDbContext>()
                .UseInMemoryDatabase("VhBookingsInMemory").Options;
            _context = new BookingsDbContext(contextOptions);
        }

        [OneTimeTearDown]
        public void FinalCleanUp()
        {
            _context.Database.EnsureDeleted();
        }

        [SetUp]
        public async Task Setup()
        {
            _userRole1 = new UserRole(1, "user role 1");
            _userRole2 = new UserRole(2, "user role 2");

            _hearingRole1 = new HearingRole(1, "hearing role 1") { UserRole = _userRole1 };
            _hearingRole2 = new HearingRole(2, "hearing role 2") { UserRole = _userRole2 };

            _person1 = new Person(Faker.Name.Suffix(), Faker.Name.First(), Faker.Name.Last(), Faker.Internet.Email(), Faker.Internet.Email());
            _person2 = new Person(Faker.Name.Suffix(), Faker.Name.First(), Faker.Name.Last(), Faker.Internet.Email(), Faker.Internet.Email());
            _person3 = new Person(Faker.Name.Suffix(), Faker.Name.First(), Faker.Name.Last(), Faker.Internet.Email(), Faker.Internet.Email());
            _person4 = new Person(Faker.Name.Suffix(), Faker.Name.First(), Faker.Name.Last(), Faker.Internet.Email(), Faker.Internet.Email());
            _anonymisedPerson =
                new Person(Faker.Name.Suffix(), Faker.Name.First(), Faker.Name.Last(), Faker.Internet.Email(),
                    $"{Faker.Internet.Email()}@email.net");

            _caseType1 = new CaseType(12, "case 1")
                { HearingTypes = new List<HearingType> { new HearingType("Hearing type 1") } };

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
        public async Task
            GetAnonymisationDataQuery_Returns_List_Of_Usernames_And_Hearing_Ids_For_Hearings_Over_3_Months_Old()
        {
            var hearingType = _caseType1.HearingTypes.First();

            var hearing1 = new VideoHearing(_caseType1,
                hearingType, DateTime.Today, 40,
                new HearingVenue(1, "venue 1"),
                Faker.Name.First(), Faker.Name.First(), null, true, false, null);


            var hearing2 = new VideoHearing(_caseType1,
                hearingType, DateTime.Today, 40,
                new HearingVenue(1, "venue 1"),
                Faker.Name.First(), Faker.Name.First(), null, true, false, null);

            hearing1.AddJudge(_person1, _hearingRole1, new CaseRole(1, "judge"), "Judge 123");
            hearing1.AddIndividual(_person2, _hearingRole2, new CaseRole(2, "individual"), "Individual 123");

            hearing2.AddJudge(_person3, _hearingRole1, new CaseRole(1, "judge"), "Judge 123");
            hearing2.AddIndividual(_person4, _hearingRole2, new CaseRole(2, "individual"), "Individual 123");

            var videoHearingType = typeof(VideoHearing);
            videoHearingType.GetProperty("ScheduledDateTime").SetValue(hearing1, DateTime.UtcNow.AddMonths(-3));

            await _context.VideoHearings.AddRangeAsync(hearing1, hearing2);
            await _context.SaveChangesAsync();

            var result = await _handler.Handle(new GetAnonymisationDataQuery());

            result.Usernames.Count.Should().Be(2);
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
            var hearingType = _caseType1.HearingTypes.First();

            var hearing1 = new VideoHearing(_caseType1,
                hearingType, DateTime.Today, 40,
                new HearingVenue(1, "venue 1"),
                Faker.Name.First(), Faker.Name.First(), null, true, false, null);


            var hearing2 = new VideoHearing(_caseType1,
                hearingType, DateTime.Today, 40,
                new HearingVenue(1, "venue 1"),
                Faker.Name.First(), Faker.Name.First(), null, true, false, null);

            hearing1.AddJudge(_person1, _hearingRole1, new CaseRole(1, "judge"), "Judge 123");
            hearing1.AddIndividual(_person2, _hearingRole2, new CaseRole(2, "individual"), "Individual 123");

            hearing2.AddJudge(_person3, _hearingRole1, new CaseRole(1, "judge"), "Judge 123");
            hearing2.AddIndividual(_person1, _hearingRole2, new CaseRole(4, "pm"), "PM 123");
            hearing2.AddIndividual(_person4, _hearingRole2, new CaseRole(2, "individual"), "Individual 123");

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
        public async Task
            GetAnonymisationDataQuery_Filters_Out_Usernames_And_Hearing_Ids_Already_Processed_By_LastRunDate_Specified_In_JobHistory()
        {
            var jobHistory = new JobHistoryForTest();
            jobHistory.setLastRunDate(DateTime.Now.AddMonths(-5));
            _context.JobHistory.Add(jobHistory);

            var hearingType = _caseType1.HearingTypes.First();

            var hearing1 = new VideoHearing(_caseType1,
                hearingType, DateTime.Today, 40,
                new HearingVenue(1, "venue 1"),
                Faker.Name.First(), Faker.Name.First(), null, true, false, null);


            var hearing2 = new VideoHearing(_caseType1,
                hearingType, DateTime.Today, 40,
                new HearingVenue(1, "venue 1"),
                Faker.Name.First(), Faker.Name.First(), null, true, false, null);

            hearing1.AddJudge(_person1, _hearingRole1, new CaseRole(1, "judge"), "Judge 123");
            hearing1.AddIndividual(_person2, _hearingRole2, new CaseRole(2, "individual"), "Individual 123");

            hearing2.AddJudge(_person3, _hearingRole1, new CaseRole(1, "judge"), "Judge 123");
            hearing2.AddIndividual(_person4, _hearingRole2, new CaseRole(2, "individual"), "Individual 123");

            var videoHearingType = typeof(VideoHearing);
            videoHearingType.GetProperty("ScheduledDateTime")
                .SetValue(hearing1,
                    DateTime.UtcNow
                        .AddMonths(-10)); //setting the scheduled time for a hearing that does not fall in between cut off date and last run date 

            await _context.VideoHearings.AddRangeAsync(hearing1, hearing2);
            await _context.SaveChangesAsync();

            var result = await _handler.Handle(new GetAnonymisationDataQuery());

            result.Usernames.Count.Should().Be(0);
            result.HearingIds.Count.Should().Be(0);

            _context.JobHistory.Remove(jobHistory);
        }

        [Test]
        public async Task
            GetAnonymisationDataQuery_Returns_List_Of_Usernames_And_Hearing_Ids_For_Hearings_Over_3_Months_Old_And_Are_Unique()
        {
            var hearingType = _caseType1.HearingTypes.First();

            var hearing1 = new VideoHearing(_caseType1,
                hearingType, DateTime.Today, 40,
                new HearingVenue(1, "venue 1"),
                Faker.Name.First(), Faker.Name.First(), null, true, false, null);


            var hearing2 = new VideoHearing(_caseType1,
                hearingType, DateTime.Today, 40,
                new HearingVenue(1, "venue 1"),
                Faker.Name.First(), Faker.Name.First(), null, true, false, null);


            var hearing3 = new VideoHearing(_caseType1,
                hearingType, DateTime.Today, 40,
                new HearingVenue(1, "venue 1"),
                Faker.Name.First(), Faker.Name.First(), null, true, false, null);

            hearing1.AddJudge(_person1, _hearingRole1, new CaseRole(1, "judge"), "Judge 123");
            hearing1.AddIndividual(_person2, _hearingRole2, new CaseRole(2, "individual"), "Individual 123");

            hearing2.AddJudge(_person3, _hearingRole1, new CaseRole(1, "judge"), "Judge 123");
            hearing2.AddIndividual(_person4, _hearingRole2, new CaseRole(2, "individual"), "Individual 123");

            hearing3.AddJudge(_person1, _hearingRole1, new CaseRole(1, "judge"), "Judge 123");
            hearing3.AddIndividual(_person2, _hearingRole2, new CaseRole(2, "individual"), "Individual 123");

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
            var hearingType = _caseType1.HearingTypes.First();

            var hearing1 = new VideoHearing(_caseType1,
                hearingType, DateTime.Today, 40,
                new HearingVenue(1, "venue 1"),
                Faker.Name.First(), Faker.Name.First(), null, true, false, null);


            var hearing2 = new VideoHearing(_caseType1,
                hearingType, DateTime.Today, 40,
                new HearingVenue(1, "venue 1"),
                Faker.Name.First(), Faker.Name.First(), null, true, false, null);

            hearing1.AddJudge(_person1, _hearingRole1, new CaseRole(1, "judge"), "Judge 123");
            hearing1.AddIndividual(_person2, _hearingRole2, new CaseRole(2, "individual"), "Individual 123");
            hearing1.AddIndividual(_anonymisedPerson, _hearingRole2, new CaseRole(2, "individual"), "Individual 123");

            hearing2.AddJudge(_person3, _hearingRole1, new CaseRole(1, "judge"), "Judge 123");
            hearing2.AddIndividual(_person4, _hearingRole2, new CaseRole(2, "individual"), "Individual 123");

            var videoHearingType = typeof(VideoHearing);
            videoHearingType.GetProperty("ScheduledDateTime").SetValue(hearing1, DateTime.UtcNow.AddMonths(-3));

            await _context.VideoHearings.AddRangeAsync(hearing1, hearing2);
            await _context.SaveChangesAsync();

            var result = await _handler.Handle(new GetAnonymisationDataQuery());

            result.Usernames.Count.Should().Be(2);

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
            var hearingType = _caseType1.HearingTypes.First();

            var hearing1 = new VideoHearing(_caseType1,
                hearingType, DateTime.Today, 40,
                new HearingVenue(1, "venue 1"),
                Faker.Name.First(), Faker.Name.First(), null, true, false, null);


            var hearing2 = new VideoHearing(_caseType1,
                hearingType, DateTime.Today, 40,
                new HearingVenue(1, "venue 1"),
                Faker.Name.First(), Faker.Name.First(), null, true, false, null);

            var automationTestAccountPersonEntry =
                new Person(Faker.Name.Suffix(), Faker.Name.First(), Faker.Name.Last(), Faker.Internet.Email(), username);

            hearing1.AddJudge(_person1, _hearingRole1, new CaseRole(1, "judge"), "Judge 123");
            hearing1.AddIndividual(_person2, _hearingRole2, new CaseRole(2, "individual"), "Individual 123");
            hearing1.AddIndividual(automationTestAccountPersonEntry, _hearingRole2, new CaseRole(2, "individual"),
                "Individual 123");

            hearing2.AddJudge(_person3, _hearingRole1, new CaseRole(1, "judge"), "Judge 123");
            hearing2.AddIndividual(_person4, _hearingRole2, new CaseRole(2, "individual"), "Individual 123");

            var videoHearingType = typeof(VideoHearing);
            videoHearingType.GetProperty("ScheduledDateTime").SetValue(hearing1, DateTime.UtcNow.AddMonths(-3));

            await _context.VideoHearings.AddRangeAsync(hearing1, hearing2);
            await _context.SaveChangesAsync();

            var result = await _handler.Handle(new GetAnonymisationDataQuery());

            result.Usernames.Count.Should().Be(2);

            result.Usernames.Should().Contain(x => x == _person1.Username);
            result.Usernames.Should().Contain(x => x == _person2.Username);
            result.Usernames.Should().NotContain(x => x == _person3.Username);
            result.Usernames.Should().NotContain(x => x == _person4.Username);
            result.Usernames.Should().NotContain(x => x == automationTestAccountPersonEntry.Username);
        }

        private class JobHistoryForTest : JobHistory
        {
            public void setLastRunDate(DateTime lastRunDate)
            {
                LastRunDate = lastRunDate;
            }
        }
    }
}