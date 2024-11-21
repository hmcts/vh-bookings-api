using BookingsApi.Client;

namespace BookingsApi.IntegrationTests.Api.V1.JobHistory;

public class GetJobHistoryTests : ApiTest
{
    [Test]
    public async Task should_return_job_history()
    {
        // arrange
        var jobName = "should_return_job_history";
        Domain.JobHistory jobHistory = await Hooks.SeedJobHistory(jobName);
        
        // act
        using var client = Application.CreateClient();
        var bookingsApiClient = BookingsApiClient.GetClient(client);
        var response = await bookingsApiClient.GetJobHistoryAsync(jobName);
        
        // assert
        response.Should().NotBeNull();
        response.Should().HaveCount(1);
        response.First().JobName.Should().Be(jobName);
        response.First().IsSuccessful.Should().Be(jobHistory.IsSuccessful);
        response.First().LastRunDate.Should().BeCloseTo(jobHistory.LastRunDate!.Value, TimeSpan.FromSeconds(1));
    }   
}