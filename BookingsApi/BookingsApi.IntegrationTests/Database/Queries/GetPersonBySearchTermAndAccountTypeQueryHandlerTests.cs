using BookingsApi.DAL;
using BookingsApi.DAL.Queries;
using BookingsApi.Domain;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookingsApi.IntegrationTests.Database.Queries
{
    [TestFixture]
    public class GetPersonBySearchTermAndAccountTypeQueryHandlerTests
    {
        private GetPersonBySearchTermAndAccountTypeQueryHandler _handler;
        private BookingsDbContext _context;
        private Person Person1, Person2, Person3;

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
            Person1 = new Person("mr", "luffy", "dragon", "luffy2@strawhat.net") { ContactEmail = "luffy2@strawhat.net" };
            Person2 = new Person("mr", "zoro", "rononora", "zoro@strawhat.net") { ContactEmail = "zoro@strawhat.net" };
            Person3 = new Person("mr", "luffy", "dragon", "luffy@strawhat.net") { ContactEmail = "luffy@strawhat.net" };
            _context.Persons.AddRange(Person1, Person2, Person3);
            _context.SaveChanges();
            _handler = new GetPersonBySearchTermAndAccountTypeQueryHandler(_context);
        }

        [TearDown]
        public void CleanUp()
        {
            _context.Persons.RemoveRange(Person1, Person2, Person3);
        }

        [Test]
        public async Task Returns_Persons_Record_By_Search_Term()
        {
            var persons = await _handler.Handle(new GetPersonBySearchTermAndAccountTypeQuery("luff"));

            Assert.AreEqual(2, persons.Count);
            persons.Select(m => m.Id).Should().Contain(Person1.Id);
            persons.Select(m => m.Id).Should().NotContain(Person2.Id);

        }

        [Test]
        public async Task Returns_Persons_With_Empty_Account_Types_When_Unspecified()
        {
            Person1.AccountType = "SomeAccount";
            _context.Persons.Update(Person1);
            _context.SaveChanges();
            _handler = new GetPersonBySearchTermAndAccountTypeQueryHandler(_context);

            var persons = await _handler.Handle(new GetPersonBySearchTermAndAccountTypeQuery("luff"));

            Assert.AreEqual(1, persons.Count);
            persons.Select(m => m.Id).Should().NotContain(Person1.Id);
        }

        [Test]
        public async Task Returns_Persons_With_Specified_Account_Type()
        {
            var specifiedAccountType = "SomeAccount";
            Person1.AccountType = specifiedAccountType;
            _context.Persons.Update(Person1);
            _context.SaveChanges();
            _handler = new GetPersonBySearchTermAndAccountTypeQueryHandler(_context);

            var persons = await _handler.Handle(new GetPersonBySearchTermAndAccountTypeQuery("luff", new List<string> { specifiedAccountType }));

            Assert.AreEqual(1, persons.Count);
            persons.Select(m => m.Id).Should().Contain(Person1.Id);
            persons.Select(m => m.Id).Should().NotContain(Person2.Id);
            persons.Select(m => m.Id).Should().NotContain(Person3.Id);
        }

        [Test]
        public async Task Returns_Persons_With_Multiple_Specified_Account_Types()
        {
            var accountType1 = "SomeAccount1";
            var accountType2 = "SomeAccount2";
            Person1.AccountType = accountType1;
            Person3.AccountType = accountType2;
            _context.Persons.Update(Person1);
            _context.SaveChanges();
            _handler = new GetPersonBySearchTermAndAccountTypeQueryHandler(_context);

            var persons = await _handler.Handle(new GetPersonBySearchTermAndAccountTypeQuery("luff", new List<string> { accountType1, accountType2 }));

            Assert.AreEqual(2, persons.Count);
            persons.Select(m => m.Id).Should().Contain(Person1.Id);
            persons.Select(m => m.Id).Should().NotContain(Person2.Id);
            persons.Select(m => m.Id).Should().Contain(Person3.Id);
        }
    }
}
