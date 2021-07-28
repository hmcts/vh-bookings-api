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
        private Person Person1, Person2, Person3;
        private Participant Participant1, Participant2, Participant3;

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
            Person1 = new Person("mr", "luffy", "dragon", "luffy2@strawhat.net") { ContactEmail = "luffy2@strawhat.net" };
            Person2 = new Person("mr", "zoro", "rononora", "zoro@strawhat.net") { ContactEmail = "zoro@strawhat.net" };
            Person3 = new Person("mr", "luffy", "dragon", "luffy@strawhat.net") { ContactEmail = "luffy@strawhat.net" };

            //participants record
            Participant1 = new Individual(Person1, new HearingRole(123, "hearingrole"), new CaseRole(345, "caserole")) { Discriminator = "Individual"};
            Participant2 = new Judge(Person2, new HearingRole(123, "hearingrole"), new CaseRole(345, "caserole")) { Discriminator = "Judge" };
            Participant3 = new JudicialOfficeHolder(Person3, new HearingRole(123, "hearingrole"), new CaseRole(345, "caserole")) { Discriminator = "JudicialOfficeHolder" };
            _context.Persons.AddRange(Person1, Person2, Person3);
            _context.Participants.AddRange(Participant1, Participant2, Participant3);
            _context.SaveChanges();
            _handler = new GetPersonBySearchTermQueryHandler(_context);
        }

        [TearDown]
        public void CleanUp()
        {
            _context.Persons.RemoveRange(Person1, Person2, Person3);
            _context.Participants.RemoveRange(Participant1, Participant2, Participant3);
        }

        [Test]
        public async Task Returns_Persons_Record_By_Search_Term()
        {
            var persons = await _handler.Handle(new GetPersonBySearchTermQuery("luff"));

            Assert.AreEqual(1, persons.Count);
            persons.Select(m => m.Id).Should().Contain(Person1.Id);
            persons.Select(m => m.Id).Should().NotContain(Person2.Id);
            persons.Select(m => m.Id).Should().NotContain(Person3.Id);
        }

        [Test]
        public async Task Filters_Out_Participant_With_Discriminator_Of_Judge_And_JudicialOfficeHolder()
        {
            var _person4 = new Person("mr", "luffy", "dragon", "luffy5@strawhat.net") { ContactEmail = "luffy5@strawhat.net" };
            var _participant4 = new JudicialOfficeHolder(_person4, new HearingRole(123, "hearingrole"), new CaseRole(345, "caserole")) { Discriminator = "Individual" };
            _context.Persons.Add(_person4);
            _context.Participants.Add(_participant4);
            _context.SaveChanges();

            var persons = await _handler.Handle(new GetPersonBySearchTermQuery("luff"));

            Assert.AreEqual(2, persons.Count);
            persons.Select(m => m.Id).Should().Contain(Person1.Id);
            persons.Select(m => m.Id).Should().Contain(_person4.Id);
            persons.Select(m => m.Id).Should().NotContain(Person2.Id);
            persons.Select(m => m.Id).Should().NotContain(Person3.Id);

            _context.Persons.Remove(_person4);
            _context.Participants.Remove(_participant4);
        }

    }
}