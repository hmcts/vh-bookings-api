using BookingsApi.Contract.Configuration;
using BookingsApi.DAL;
using BookingsApi.DAL.Queries;
using BookingsApi.Domain;
using BookingsApi.Domain.Participants;
using BookingsApi.Domain.RefData;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Moq;
using NUnit.Framework;
using System.Linq;
using System.Threading.Tasks;

namespace BookingsApi.IntegrationTests.Database.Queries
{
    public class GetPersonBySearchTermQueryHandlerEFCoreTests
    {
        private GetPersonBySearchTermQueryHandler _handler;
        private BookingsDbContext _context;
        private Person IndividualPerson, JudgePerson, JudicialOfficeHolderPerson, StaffMemberPerson;
        private Participant IndividualParticipant, JudgeParticipant, JudicialOfficeHolderParticipant, StaffMemberParticipant;
        private Organisation organisation;
        private Mock<IOptions<FeatureFlagConfiguration>> _configOptions;

        [OneTimeSetUp]
        public void InitialSetup()
        {
            var contextOptions = new DbContextOptionsBuilder<BookingsDbContext>().UseInMemoryDatabase(databaseName: "VhBookings").Options;
            _context = new BookingsDbContext(contextOptions);
            _configOptions = new Mock<IOptions<FeatureFlagConfiguration>>();
        }

        [OneTimeTearDown]
        public void FinalCleanUp()
        {
            _context.Database.EnsureDeleted();
        }

        [SetUp]
        public void Setup()
        {
            //orgranisation table setup
            organisation = new Organisation("strawhat");

            //persons record
            IndividualPerson = new Person("mr", "luffy", "dragon", "luffy2@strawhat.net", "luffy2@strawhat.net") { Organisation = organisation };
            JudgePerson = new Person("mr", "zoro", "rononora", "zoro@strawhat.net", "zoro@strawhat.net") { Organisation = organisation };
            JudicialOfficeHolderPerson = new Person("mr", "luffy", "dragon", "luffy@strawhat.net", "luffy@strawhat.net") { Organisation = organisation };
            StaffMemberPerson = new Person("mr", "luffy", "duffy", "luffy@staff.net", "luffy@staffmember.net") {  Organisation = organisation };

            //participants record
            IndividualParticipant = new Individual(IndividualPerson, new HearingRole(123, "hearingrole"), new CaseRole(345, "caserole")) { Discriminator = "Individual" };
            var IndividualParticipant2 = new Individual(IndividualPerson, new HearingRole(123, "hearingrole"), new CaseRole(345, "caserole")) { Discriminator = "Individual" };
            JudgeParticipant = new Judge(JudgePerson, new HearingRole(123, "hearingrole"), new CaseRole(345, "caserole")) { Discriminator = "Judge" };
            JudicialOfficeHolderParticipant = new JudicialOfficeHolder(JudicialOfficeHolderPerson, new HearingRole(123, "hearingrole"), new CaseRole(345, "caserole")) { Discriminator = "JudicialOfficeHolder" };
            StaffMemberParticipant = new JudicialOfficeHolder(StaffMemberPerson, new HearingRole(719, "hearingrole"), new CaseRole(345, "caserole")) { Discriminator = "StaffMember" };
            
            _context.Persons.AddRange(IndividualPerson, JudgePerson, JudicialOfficeHolderPerson, StaffMemberPerson);
            _context.Participants.AddRange(IndividualParticipant, IndividualParticipant2, JudgeParticipant, JudicialOfficeHolderParticipant, StaffMemberParticipant);
            _context.SaveChanges();
            _configOptions.Setup(opt => opt.Value).Returns(new FeatureFlagConfiguration()
            {
                EJudFeature = true
            });

            _handler = new GetPersonBySearchTermQueryHandler(_context, _configOptions.Object);
        }

        [TearDown]
        public void CleanUp()
        {
            _context.Persons.RemoveRange(IndividualPerson, JudgePerson, JudicialOfficeHolderPerson, StaffMemberPerson);
            _context.Participants.RemoveRange(IndividualParticipant, JudgeParticipant, JudicialOfficeHolderParticipant,StaffMemberParticipant);
        }

        [Test]
        public async Task Returns_Persons_Record_By_Search_Term_EJjud_ON()
        {
            var persons = await _handler.Handle(new GetPersonBySearchTermQuery("luff"));

            Assert.AreEqual(1, persons.Count);
            persons.Select(m => m.Id).Should().Contain(IndividualPerson.Id);
            persons.Select(m => m.Id).Should().NotContain(JudgePerson.Id);
            persons.Select(m => m.Id).Should().NotContain(JudicialOfficeHolderPerson.Id);
        }

        [Test]
        public async Task Returns_Persons_Record_By_Search_Term_Ejud_OFF()
        {
            _configOptions.Setup(opt => opt.Value).Returns(new FeatureFlagConfiguration()
            {
                EJudFeature = false
            });
            _handler = new GetPersonBySearchTermQueryHandler(_context, _configOptions.Object);
            var persons = await _handler.Handle(new GetPersonBySearchTermQuery("luff"));

            Assert.AreEqual(3, persons.Count);
            persons.Select(m => m.Id).Should().Contain(IndividualPerson.Id);
            persons.Select(m => m.Id).Should().Contain(JudicialOfficeHolderPerson.Id);
            persons.Select(m => m.Id).Should().NotContain(JudgePerson.Id);
        }

        [Test]
        public async Task Handle_Should_Not_Filters_Out_Participant_With_Discriminator_Of_Judge_And_JudicialOfficeHolder()
        {
            _configOptions.Setup(opt => opt.Value).Returns(new FeatureFlagConfiguration()
            {
                EJudFeature = false
            });
            _handler = new GetPersonBySearchTermQueryHandler(_context, _configOptions.Object);
            var persons = await _handler.Handle(new GetPersonBySearchTermQuery("luff"));

            Assert.AreEqual(3, persons.Count);
            persons.Select(m => m.Id).Should().Contain(IndividualPerson.Id);
            persons.Select(m => m.Id).Should().NotContain(JudgePerson.Id);
            persons.Select(m => m.Id).Should().Contain(JudicialOfficeHolderPerson.Id);
        }

        [Test]
        public async Task Includes_Organisation()
        {
            var persons = await _handler.Handle(new GetPersonBySearchTermQuery("luff"));

            var personToQuery = persons.FirstOrDefault(m => m.Id == IndividualPerson.Id);
            personToQuery.Id.Should().Be(IndividualPerson.Id);
            personToQuery.Organisation.Should().NotBeNull();
            personToQuery.Organisation.Name.Should().Be("strawhat");
        }

    }
}