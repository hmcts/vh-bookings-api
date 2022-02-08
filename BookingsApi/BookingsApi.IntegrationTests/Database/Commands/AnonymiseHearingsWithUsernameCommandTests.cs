using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BookingsApi.DAL;
using BookingsApi.DAL.Commands;
using BookingsApi.Domain;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;

namespace BookingsApi.IntegrationTests.Database.Commands
{
    public class AnonymiseHearingsWithUsernameCommandTests
    {
        private BookingsDbContext _context;
        private AnonymiseHearingsWithUsernameCommandHandler _command;
        private Person _person1, _person2;

        [OneTimeSetUp]
        public async Task InitialSetup()
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
        public async Task SetUp()
        {
            _person1 = new Person(Faker.Name.Suffix(), Faker.Name.First(), Faker.Name.Last(), Faker.Internet.Email()) { ContactEmail = Faker.Internet.Email() };
            _person2 = new Person(Faker.Name.Suffix(), Faker.Name.First(), Faker.Name.Last(), Faker.Internet.Email()) { ContactEmail = Faker.Internet.Email() };
            
            await _context.Persons.AddRangeAsync(_person1, _person2);
            
            await _context.SaveChangesAsync();
            
            _command = new AnonymiseHearingsWithUsernameCommandHandler(_context);
        }

        [Test]
        public async Task AnonymiseHearingsWithUsernameCommand_Anonymises_Only_Specified_Username_In_Person_Table()
        {
            var personEntryBeforeAnonymisation = new Person(_person1.Title, _person1.FirstName, _person1.LastName, _person1.Username);
            var query = new AnonymiseHearingsWithUsernameCommand {Username = _person1.Username};
            
            await _command.Handle(query);

            var anonymisedPerson = _context.Persons.First(p => p.Id == _person1.Id);
            var unAnonymisedPerson = _context.Persons.First(p => p.Id == _person2.Id);

            anonymisedPerson.FirstName.Should().NotBe(personEntryBeforeAnonymisation.FirstName);
            anonymisedPerson.LastName.Should().NotBe(personEntryBeforeAnonymisation.LastName);
            anonymisedPerson.TelephoneNumber.Should().Be("00000000000");
            anonymisedPerson.ContactEmail.Should().Contain("@hmcts.net");
            anonymisedPerson.Username.Should().Contain("@email.net");
            
            unAnonymisedPerson.FirstName.Should().Be(_person2.FirstName);
            unAnonymisedPerson.LastName.Should().Be(_person2.LastName);
            unAnonymisedPerson.TelephoneNumber.Should().Be(_person2.TelephoneNumber);
            unAnonymisedPerson.ContactEmail.Should().Be(_person2.ContactEmail);
            unAnonymisedPerson.Username.Should().Be(_person2.Username);
        }

        [Test]
        public async Task AnonymiseHearingsWithUsernameCommand_Anonymises_Organisation_When_OrganisationId_Is_Not_Null()
        {
            var organisation = new Organisation(Faker.Company.Suffix());

            _person1.UpdateOrganisation(organisation);

            var organisationNameBeforeAnonymisation = organisation.Name;
            
            await _command.Handle(new AnonymiseHearingsWithUsernameCommand {Username = _person1.Username});

            var anonymisedOrganisation = _context.Persons.Select(x => x.Organisation).FirstOrDefault(x => x.Id == organisation.Id);
            anonymisedOrganisation.Name.Should().NotBe(organisationNameBeforeAnonymisation);
        }
    }
}