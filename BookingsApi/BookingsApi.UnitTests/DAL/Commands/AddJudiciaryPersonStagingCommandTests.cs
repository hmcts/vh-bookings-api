using Bogus;
using BookingsApi.DAL;
using BookingsApi.DAL.Commands;
using Microsoft.EntityFrameworkCore;

namespace BookingsApi.UnitTests.DAL.Commands
{
    public class AddJudiciaryPersonStagingCommandTests
    {
        private BookingsDbContext _context;
        private AddJudiciaryPersonStagingCommandHandler _handler;
        private static readonly Faker Faker = new();
        
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
        public void Setup()
        {
            _handler = new AddJudiciaryPersonStagingCommandHandler(_context);
        }

        [Test]
        public async Task Adds_Entry_To_Table()
        {
            var command = new AddJudiciaryPersonStagingCommand
            {
                ExternalRefId = Faker.Name.FirstName(),
                PersonalCode = Faker.Name.FirstName(),
                Title = Faker.Name.FirstName(),
                KnownAs = Faker.Name.FirstName(),
                Surname = Faker.Name.FirstName(),
                Fullname = Faker.Name.FirstName(),
                PostNominals = Faker.Name.FirstName(),
                Email = Faker.Name.FirstName(),
                WorkPhone = Faker.Phone.PhoneNumber(),
                Leaver = Faker.Name.FirstName(),
                LeftOn = Faker.Name.FirstName(),
                Deleted = true,
                DeletedOn = "2023-01-01"
            };

            await _handler.Handle(command);

            var newEntry =
                await _context.JudiciaryPersonsStaging.FirstOrDefaultAsync(x => x.ExternalRefId == command.ExternalRefId);

            newEntry.Should().NotBeNull();
            newEntry.Id.Should().NotBeEmpty();
            newEntry.PersonalCode.Should().Be(command.PersonalCode);
            newEntry.Title.Should().Be(command.Title);
            newEntry.KnownAs.Should().Be(command.KnownAs);
            newEntry.Surname.Should().Be(command.Surname);
            newEntry.Fullname.Should().Be(command.Fullname);
            newEntry.PostNominals.Should().Be(command.PostNominals);
            newEntry.Email.Should().Be(command.Email);
            newEntry.WorkPhone.Should().Be(command.WorkPhone);
            newEntry.Leaver.Should().Be(command.Leaver);
            newEntry.LeftOn.Should().Be(command.LeftOn);
            newEntry.CreatedDate.Should().BeAfter(DateTime.Now.AddDays(-1));
            newEntry.UpdatedDate.Should().BeAfter(DateTime.Now.AddDays(-1));
            newEntry.Deleted.Should().Be(newEntry.Deleted);
            newEntry.DeletedOn.Should().Be(newEntry.DeletedOn);
        }
    }
}