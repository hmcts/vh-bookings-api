using System.Linq;
using BookingsApi.DAL;
using BookingsApi.DAL.Commands;

namespace BookingsApi.IntegrationTests.Database.Commands
{
    public class AddJobHistoryCommandTests: DatabaseTestsBase
    {
        private AddJobHistoryCommandHandler _commandHandler;
        private BookingsDbContext _context;

        private string job = "unitTestJobName";
        private bool success = true;
        
        [SetUp]
        public void Setup()
        {
            _context = new BookingsDbContext(BookingsDbContextOptions);
            _commandHandler = new AddJobHistoryCommandHandler(_context);
        }
        
        [Test]
        public async Task Should_add_record_to_job_history()
        {
            var beforeCount = _context.JobHistory.Count();
            var command = new AddJobHistoryCommand{JobName = job, IsSuccessful = success};
            
            await _commandHandler.Handle(command);
            
            var afterCount = _context.JobHistory.Count();
            afterCount.Should().BeGreaterThan(beforeCount);
            
            var newRecord = _context.JobHistory.First(e => e.JobName == job);
            newRecord.JobName.Should().Be(job);
            newRecord.IsSuccessful.Should().Be(success);
        }
        [TearDown]
        public void RemoveTestRecords()
        {

            var testRecords = _context.JobHistory.Where(e => e.JobName == job);
            _context.JobHistory.RemoveRange(testRecords);
            _context.SaveChanges();
        }
    }
}