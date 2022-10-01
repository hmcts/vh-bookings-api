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
    public class GetStaffMemberBySearchTermQueryHandlerDatabaseTests : DatabaseTestsBase
    {
        private BookingsDbContext _context;
        private GetStaffMemberBySearchTermQueryHandler _handler;
        private Person _individualPerson, _judgePerson, _judicialOfficeHolderPerson, _staffMemberPerson, _repPerson;
        private Participant _individualParticipant, _judgeParticipant, _judicialOfficeHolderParticipant, _staffMemberParticipant, _repParticipant;
        private Organisation _organisation;

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
            _organisation = new Organisation(Faker.Company.Name());

            _individualPerson = new Person(Faker.Name.Suffix(), Faker.Name.First(), Faker.Name.Last(), Faker.Internet.Email(), Faker.Internet.Email()) { Organisation = _organisation };
            _judgePerson = new Person(Faker.Name.Suffix(), Faker.Name.First(), Faker.Name.Last(), Faker.Internet.Email(), Faker.Internet.Email()) { Organisation = _organisation };
            _judicialOfficeHolderPerson = new Person(Faker.Name.Suffix(), Faker.Name.First(), Faker.Name.Last(), Faker.Internet.Email(), Faker.Internet.Email()) { Organisation = _organisation };
            _staffMemberPerson = new Person(Faker.Name.Suffix(), Faker.Name.First(), Faker.Name.Last(), Faker.Internet.Email(), Faker.Internet.Email()) {  Organisation = _organisation };
            _repPerson = new Person(Faker.Name.Suffix(), Faker.Name.First(), Faker.Name.Last(), Faker.Internet.Email(), Faker.Internet.Email()) {  Organisation = _organisation };

            _judgeParticipant = new Judge(_judgePerson, new HearingRole(123, "hearingrole"), new CaseRole(345, "caserole")) { Discriminator = "Judge" };
            _individualParticipant = new Individual(_individualPerson, new HearingRole(123, "hearingrole"), new CaseRole(345, "caserole")) { Discriminator = "Individual" };
            _judicialOfficeHolderParticipant = new JudicialOfficeHolder(_judicialOfficeHolderPerson, new HearingRole(123, "hearingrole"), new CaseRole(345, "caserole")) { Discriminator = "JudicialOfficeHolder" };
            _staffMemberParticipant = new JudicialOfficeHolder(_staffMemberPerson, new HearingRole(719, "hearingrole"), new CaseRole(345, "caserole")) { Discriminator = "StaffMember" };
            _repParticipant = new Representative(_repPerson, new HearingRole(719, "hearingrole"), new CaseRole(345, "caserole")) { Discriminator = "Representative" };

            _context.Persons.AddRange(_individualPerson, _judgePerson, _judicialOfficeHolderPerson, _staffMemberPerson, _repPerson);
            _context.Participants.AddRange(_individualParticipant, _judgeParticipant, _judicialOfficeHolderParticipant, _staffMemberParticipant, _repParticipant);

            _context.SaveChanges();

            _handler = new GetStaffMemberBySearchTermQueryHandler(_context);
        }

        [TearDown]
        public void CleanUp()
        {
            _context.Persons.RemoveRange(_individualPerson, _judgePerson, _judicialOfficeHolderPerson, _staffMemberPerson, _repPerson);
            _context.Participants.RemoveRange(_individualParticipant, _judgeParticipant, _judicialOfficeHolderParticipant, _staffMemberParticipant, _repParticipant);
        }

        [Test]
        public async Task GetStaffMemberBySearchTermQuery_ShouldFindTheStaffMemberOnly_CaseInsensitive()
        {
            var staffMember = await _handler.Handle(new GetStaffMemberBySearchTermQuery(_staffMemberPerson.ContactEmail));
            staffMember.Count.Should().Be(1);
            staffMember.First().ContactEmail.Should().Be(_staffMemberPerson.ContactEmail);
            staffMember.Select(m => m.Id).Should().Contain(_staffMemberPerson.Id);
            staffMember.Select(m => m.Id).Should().NotContain(_judgePerson.Id);
            staffMember.Select(m => m.Id).Should().NotContain(_individualPerson.Id);
            staffMember.Select(m => m.Id).Should().NotContain(_judicialOfficeHolderPerson.Id);
            staffMember.Select(m => m.Id).Should().NotContain(_repPerson.Id);
        }

        [Test]
        public async Task GetStaffMemberBySearchTermQuery_ShouldNot_Retun_Judge_PersonDetails()
        {
            var staffMember = await _handler.Handle(new GetStaffMemberBySearchTermQuery(_judgePerson.ContactEmail));
            staffMember.Count.Should().Be(0);
        }

        [Test]
        public async Task GetStaffMemberBySearchTermQuery_ShouldNot_Retun_Joh_PersonDetails()
        {
            var staffMember = await _handler.Handle(new GetStaffMemberBySearchTermQuery(_judicialOfficeHolderPerson.ContactEmail));
            staffMember.Count.Should().Be(0);
        }

        [Test]
        public async Task GetStaffMemberBySearchTermQuery_ShouldNot_Retun_individual_PersonDetails()
        {
            var staffMember = await _handler.Handle(new GetStaffMemberBySearchTermQuery(_individualPerson.ContactEmail));
            staffMember.Count.Should().Be(0);
        }
        
        [Test]
        public async Task GetStaffMemberBySearchTermQuery_ShouldNot_Retun_representative_PersonDetails()
        {
            var staffMember = await _handler.Handle(new GetStaffMemberBySearchTermQuery(_repPerson.ContactEmail));
            staffMember.Count.Should().Be(0);
        }
    }
}