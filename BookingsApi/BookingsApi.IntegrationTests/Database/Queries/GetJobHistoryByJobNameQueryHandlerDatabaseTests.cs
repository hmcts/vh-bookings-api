using System.Linq;
using System.Threading.Tasks;
using BookingsApi.DAL;
using BookingsApi.DAL.Queries.V1;
using BookingsApi.Domain;
using FluentAssertions;
using NUnit.Framework;

namespace BookingsApi.IntegrationTests.Database.Queries
{
    public class GetJobHistoryByJobNameQueryHandlerDatabaseTests : DatabaseTestsBase
    {
        private GetJobHistoryByJobNameQueryHandler _handler;
        private string jobName = "unitTestJobName";
        [SetUp]
        public void Setup()
        {
            var context = new BookingsDbContext(BookingsDbContextOptions);
            _handler = new GetJobHistoryByJobNameQueryHandler(context);
            
            if(!context.JobHistory.Any(jh => jh.JobName == jobName)) 
            {
                context.JobHistory.Add(new UpdateJobHistory(jobName, true));
                context.SaveChanges();
            }
        }

        [Test]
        public async Task Should_return_all_case_types_and_their_hearing_types()
        {
            var jobHistory = await _handler.Handle(new GetJobHistoryByJobNameQuery(jobName));
            jobHistory.Should().NotBeNull();
            jobHistory.Count.Should().BePositive();
        }
    }
}