using BookingsApi.DAL.Commands;
using Microsoft.EntityFrameworkCore;

namespace BookingsApi.IntegrationTests.Database.Commands
{
    public class RemoveAllJudiciaryPersonStagingCommandTests : DatabaseTestsBase
    {
        private BookingsDbContext _context;
        private RemoveAllJudiciaryPersonStagingCommandHandler _command;
        private JudiciaryPersonStaging _addedPerson;

        [OneTimeSetUp]
        public void InitialSetup()
        {
            _context = new BookingsDbContext(BookingsDbContextOptions);
        }

        [OneTimeTearDown]
        public async Task FinalCleanUp()
        {
            if (_addedPerson == null) return;
            var dbResult = await _context.JudiciaryPersonsStaging.FirstOrDefaultAsync(x=> x.Email == _addedPerson.Email);
            if (dbResult != null)
            {
                _context.Remove(dbResult);
                await _context.SaveChangesAsync();
            }
        }
        
        [SetUp]
        public async Task SetUp()
        {
            _addedPerson = await Hooks.AddJudiciaryPersonStaging();
            
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