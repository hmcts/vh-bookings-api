using BookingsApi.DAL;
using BookingsApi.DAL.Commands;
using BookingsApi.DAL.Exceptions;
using BookingsApi.Domain;
using BookingsApi.Domain.RefData;
using FizzWare.NBuilder;
using Microsoft.EntityFrameworkCore;

namespace BookingsApi.UnitTests.DAL.Commands
{
    public class DeleteTestPersonCommandTests
    {
        private VideoHearing _hearing;
        private BookingsDbContext _context;

        [SetUp]
        public void Setup()
        {
            var contextOptions = new DbContextOptionsBuilder<BookingsDbContext>().UseInMemoryDatabase("VhBookingsInMemory").Options;
            _context = new BookingsDbContext(contextOptions);
            var refDataBuilder = new RefDataBuilder();
            var venue = refDataBuilder.HearingVenues[0];
            var caseType = new CaseType(1, "Generic");
            var hearingType = Builder<HearingType>.CreateNew().WithFactory(() => new HearingType("Generic")).Build();
            var scheduledDateTime = DateTime.Today.AddDays(1).AddHours(11).AddMinutes(45);
            var person = new Person("Mr", "test", "one", "ContactEmail", "UserName");
            _context.Persons.Add(person);
            _hearing = new VideoHearing(caseType, hearingType, scheduledDateTime, 100, venue, "", "", "", false, "");
            _hearing.AddIndividual(person, new HearingRole(1, "any"), new CaseRole(1, "any"), "DN");
            _context.VideoHearings.Add(_hearing);
            _context.SaveChangesAsync();
        }
        
        [TearDown]
        public void TearDown()
        {
            _context.Database.EnsureDeleted();
        }
        
        [Test]
        public async Task should_throw_an_exception_when_person_is_not_a_test_user()
        {
            // Arrange
            var commandHandler = new DeleteTestPersonCommandHandler(_context);
            var hearing = _context.VideoHearings.First();
            var newCaseType = new CaseType(2, "NotGeneric");
            _context.CaseTypes.Add(newCaseType);
            hearing.CaseType = newCaseType;
            await _context.SaveChangesAsync();
            
            var personToDelete = _hearing.Participants[0];
            var command = new DeleteTestPersonCommand(personToDelete.Person.Username);
            
            // Act & Assert
            Assert.ThrowsAsync<ArgumentException>(async () =>
            {
                await commandHandler.Handle(command);
            });
        }

        [Test]
        public async Task should_delete_person()
        {
            // Arrange
            var commandHandler = new DeleteTestPersonCommandHandler(_context);
            var personToDelete = _hearing.Participants[0];
            var command = new DeleteTestPersonCommand(personToDelete.Person.Username);
            
            // Act
            await commandHandler.Handle(command);
            
            // Assert
            var participant = await _context.VideoHearings
                .Include(u => u.Participants).ThenInclude(p => p.Person)
                .FirstAsync(x => x.Id == _hearing.Id);
            var person = await _context.Persons.FindAsync(personToDelete.PersonId);
            
            person.Should().BeNull();
            participant.Participants.Should().BeEmpty();
        }
        
        [Test]
        public void should_throw_an_exception_when_person_does_not_exist()
        {
            // Arrange
            var commandHandler = new DeleteTestPersonCommandHandler(_context);
            var username = "invalid-username";
            
            var command = new DeleteTestPersonCommand(username);

            // Act & Assert
            Assert.ThrowsAsync<PersonNotFoundException>(async () =>
            {
                await commandHandler.Handle(command);
            });
        }
    }
}
