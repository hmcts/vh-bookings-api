using System.Linq;
using System.Threading.Tasks;
using BookingsApi.DAL;
using BookingsApi.DAL.Queries;
using BookingsApi.Domain;
using BookingsApi.Domain.Participants;
using BookingsApi.Domain.RefData;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;

namespace BookingsApi.IntegrationTests.Database.Queries
{
    public class GetPersonBySearchTermQueryHandlerEFCoreTests
    {
        private GetPersonBySearchTermQueryHandler _handler;
        private BookingsDbContext _context;
        private Person IndividualPerson, JudgePerson, JudicialOfficeHolderPerson;
        private Participant IndividualParticipant, JudgeParticipant, JudicialOfficeHolderParticipant;

        [OneTimeSetUp]
        public void InitialSetup()
        {
            var contextOptions = new DbContextOptionsBuilder<BookingsDbContext>().UseInMemoryDatabase(databaseName: "VhBookings").Options;
            _context = new BookingsDbContext(contextOptions);
        }

        [OneTimeTearDown]
        public void FinalCleanUp()
        {
            _context.Database.EnsureDeleted();
        }

        [SetUp]
        public void Setup()
        {
            //persons record
            IndividualPerson = new Person("mr", "luffy", "dragon", "luffy2@strawhat.net") { ContactEmail = "luffy2@strawhat.net" };
            JudgePerson = new Person("mr", "zoro", "rononora", "zoro@strawhat.net") { ContactEmail = "zoro@strawhat.net" };
            JudicialOfficeHolderPerson = new Person("mr", "luffy", "dragon", "luffy@strawhat.net") { ContactEmail = "luffy@strawhat.net" };

            //participants record
            IndividualParticipant = new Individual(IndividualPerson, new HearingRole(123, "hearingrole"), new CaseRole(345, "caserole")) { Discriminator = "Individual"};
            JudgeParticipant = new Judge(JudgePerson, new HearingRole(123, "hearingrole"), new CaseRole(345, "caserole")) { Discriminator = "Judge" };
            JudicialOfficeHolderParticipant = new JudicialOfficeHolder(JudicialOfficeHolderPerson, new HearingRole(123, "hearingrole"), new CaseRole(345, "caserole")) { Discriminator = "JudicialOfficeHolder" };
            _context.Persons.AddRange(IndividualPerson, JudgePerson, JudicialOfficeHolderPerson);
            _context.Participants.AddRange(IndividualParticipant, JudgeParticipant, JudicialOfficeHolderParticipant);
            _context.SaveChanges();
            _handler = new GetPersonBySearchTermQueryHandler(_context);
        }

        [TearDown]
        public void CleanUp()
        {
            _context.Persons.RemoveRange(IndividualPerson, JudgePerson, JudicialOfficeHolderPerson);
            _context.Participants.RemoveRange(IndividualParticipant, JudgeParticipant, JudicialOfficeHolderParticipant);
        }

        [Test]
        public async Task Returns_Persons_Record_By_Search_Term()
        {
            var persons = await _handler.Handle(new GetPersonBySearchTermQuery("luff"));

            Assert.AreEqual(1, persons.Count);
            persons.Select(m => m.Id).Should().Contain(IndividualPerson.Id);
            persons.Select(m => m.Id).Should().NotContain(JudgePerson.Id);
            persons.Select(m => m.Id).Should().NotContain(JudicialOfficeHolderPerson.Id);
        }

        [Test]
        public async Task Filters_Out_Participant_With_Discriminator_Of_Judge_And_JudicialOfficeHolder()
        {
            var additionalIndividualPerson = new Person("mr", "luffy", "dragon", "luffy5@strawhat.net") { ContactEmail = "luffy5@strawhat.net" };
            var additionalIndividualParticipant = new JudicialOfficeHolder(additionalIndividualPerson, new HearingRole(123, "hearingrole"), new CaseRole(345, "caserole")) { Discriminator = "Individual" };
            _context.Persons.Add(additionalIndividualPerson);
            _context.Participants.Add(additionalIndividualParticipant);
            _context.SaveChanges();

            var persons = await _handler.Handle(new GetPersonBySearchTermQuery("luff"));

            Assert.AreEqual(2, persons.Count);
            persons.Select(m => m.Id).Should().Contain(IndividualPerson.Id);
            persons.Select(m => m.Id).Should().Contain(additionalIndividualPerson.Id);
            persons.Select(m => m.Id).Should().NotContain(JudgePerson.Id);
            persons.Select(m => m.Id).Should().NotContain(JudicialOfficeHolderPerson.Id);

            _context.Persons.Remove(additionalIndividualPerson);
            _context.Participants.Remove(additionalIndividualParticipant);
        }

    }
}