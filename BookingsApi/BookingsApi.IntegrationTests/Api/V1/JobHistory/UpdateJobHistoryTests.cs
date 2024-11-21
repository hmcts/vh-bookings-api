using BookingsApi.Client;

namespace BookingsApi.IntegrationTests.Api.V1.JobHistory;

public class UpdateJobHistoryTests : ApiTest
{
    [Test]
    public async Task should_update_history_to_successful()
    {
        // arrange
        var jobName = "should_update_history_to_successful";
        await Hooks.SeedJobHistory(jobName, false);
        
        // act
        using var client = Application.CreateClient();
        var bookingsApiClient = BookingsApiClient.GetClient(client);
        await bookingsApiClient.UpdateJobHistoryAsync(jobName, true);
        
        // assert
        await using var db = new BookingsDbContext(BookingsDbContextOptions);
        var updatedJobHistories = await db.JobHistory.Where(x=> x.JobName == jobName).ToListAsync();
        var updatedJobHistory = updatedJobHistories[^1];
        
        Hooks.AddJobHistoryToBeDeleted(updatedJobHistory.Id);

        updatedJobHistory!.JobName.Should().Be(jobName);
        updatedJobHistory.IsSuccessful.Should().BeTrue();
        updatedJobHistory.LastRunDate.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }   
}