using System.Threading.Tasks;
using BookingsApi.DAL;
using BookingsApi.DAL.Commands;
using BookingsApi.Domain;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;

namespace BookingsApi.IntegrationTests.Database.Commands
{
    public class RemoveAllJudiciaryPersonStagingCommandTests : DatabaseTestsBase
    {
        private BookingsDbContext _context;
        private RemoveAllJudiciaryPersonStagingCommandHandler _command;

        private JudiciaryPersonStaging _person1, _person2;

        [OneTimeSetUp]
        public void InitialSetup()
        {
            _context = new BookingsDbContext(BookingsDbContextOptions);
        }

        [OneTimeTearDown]
        public void FinalCleanUp()
        {
            _context.Database.EnsureDeleted();
        }
        
        [SetUp]
        public async Task SetUp()
        {
            await Hooks.AddJudiciaryPersonStaging();
            
            _command = new RemoveAllJudiciaryPersonStagingCommandHandler(_context);
        }

        [Test]
        public async Task RemoveAllJudiciaryPersonStagingCommand_Removes_All_Records()
        {
            await _command.Handle(new RemoveAllJudiciaryPersonStagingCommand());

            (await _context.JudiciaryPersonsStaging.CountAsync()).Should().Be(0);
        }
    }
}