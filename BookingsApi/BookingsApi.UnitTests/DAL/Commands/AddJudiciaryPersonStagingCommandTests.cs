using System;
using System.Threading.Tasks;
using BookingsApi.DAL;
using BookingsApi.DAL.Commands;
using Microsoft.EntityFrameworkCore;

namespace BookingsApi.UnitTests.DAL.Commands
{
    public class AddJudiciaryPersonStagingCommandTests
    {
        private BookingsDbContext _context;
        private AddJudiciaryPersonStagingCommandHandler _handler;
        
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
                ExternalRefId = Faker.Name.First(),
                PersonalCode = Faker.Name.First(),
                Title = Faker.Name.First(),
                KnownAs = Faker.Name.First(),
                Surname = Faker.Name.First(),
                Fullname = Faker.Name.First(),
                PostNominals = Faker.Name.First(),
                Email = Faker.Name.First(),
                Leaver = Faker.Name.First(),
                LeftOn = Faker.Name.First()
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
            newEntry.Leaver.Should().Be(command.Leaver);
            newEntry.LeftOn.Should().Be(command.LeftOn);
            newEntry.CreatedDate.Should().BeAfter(DateTime.Now.AddDays(-1));
            newEntry.UpdatedDate.Should().BeAfter(DateTime.Now.AddDays(-1));
        }
    }
}