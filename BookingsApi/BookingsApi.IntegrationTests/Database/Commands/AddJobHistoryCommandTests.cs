using BookingsApi.DAL.Commands;

namespace BookingsApi.IntegrationTests.Database.Commands
{
    public class AddJobHistoryCommandTests: DatabaseTestsBase
    {
        private AddJobHistoryCommandHandler _commandHandler;
        private BookingsDbContext _context;

        private readonly string job = "unitTestJobName";
        private readonly bool success = true;
        
        [SetUp]
        public void Setup()
        {
            _context = new BookingsDbContext(BookingsDbContextOptions);
            _commandHandler = new AddJobHistoryCommandHandler(_context);
        }
        
        [Test]
        public async Task Should_add_record_to_job_history()
        {
            var beforeCount = await _context.JobHistory.CountAsync();
            var command = new AddJobHistoryCommand{JobName = job, IsSuccessful = success};
            
            await _commandHandler.Handle(command);
            
            var afterCount = await _context.JobHistory.CountAsync();
            afterCount.Should().BeGreaterThan(beforeCount);
            
            var newRecord = await _context.JobHistory.FirstAsync(e => e.JobName == job);
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