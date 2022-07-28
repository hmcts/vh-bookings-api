using System.Linq;
using System.Threading.Tasks;
using BookingsApi.DAL;
using BookingsApi.DAL.Commands;
using FluentAssertions;
using NUnit.Framework;

namespace BookingsApi.IntegrationTests.Database.Commands
{
    public class AddJobHistoryCommandTests: DatabaseTestsBase
    {
        private AddJobHistoryCommandHandler _commandHandler;
        private BookingsDbContext _context;

        [SetUp]
        public void Setup()
        {
            _context = new BookingsDbContext(BookingsDbContextOptions);
            _commandHandler = new AddJobHistoryCommandHandler(_context);
        }
        
        [Test]
        public async Task Should_add_record_to_job_history()
        {
            var job = "TestJobName";
            var success = true;
            var beforeCount = _context.JobHistory.Count();
            var command = new AddJobHistoryCommand{JobName = job, IsSuccessful = success};
            
            await _commandHandler.Handle(command);
            
            var afterCount = _context.JobHistory.Count();
            afterCount.Should().BeGreaterThan(beforeCount);
            
            var newRecord = _context.JobHistory.First();
            newRecord.JobName.Should().Be(job);
            newRecord.IsSuccessful.Should().Be(success);
        }
    }
}