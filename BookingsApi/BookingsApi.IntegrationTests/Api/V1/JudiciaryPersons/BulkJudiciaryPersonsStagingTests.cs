using BookingsApi.Contract.V1.Requests;

namespace BookingsApi.IntegrationTests.Api.V1.JudiciaryPersons
{
    public class BulkJudiciaryPersonsStagingTests : ApiTest
    {
        [Test]
        public async Task Should_process_deleted_judiciary_person()
        {
            // Arrange
            var personalCode = Guid.NewGuid().ToString();
            var request = new List<JudiciaryPersonStagingRequest>
            {
                new()
                {
                    PersonalCode = personalCode,
                    Id = null,
                    Deleted = true,
                    DeletedOn = "2023-01-01"
                }
            };
            
            // Act
            using var client = Application.CreateClient();
            var result = await client.PostAsync(
                ApiUriFactory.JudiciaryPersonsStagingEndpoints.BulkJudiciaryPersonsStaging(), 
                RequestBody.Set(request));
            
            // Assert
            result.IsSuccessStatusCode.Should().BeTrue();
            result.StatusCode.Should().Be(HttpStatusCode.OK);
            
            await using var db = new BookingsDbContext(BookingsDbContextOptions);
            var judiciaryPersonStaging = await db.JudiciaryPersonsStaging.SingleOrDefaultAsync(jps => jps.PersonalCode == personalCode);
            judiciaryPersonStaging.Should().NotBeNull();
            judiciaryPersonStaging.Deleted.Should().BeTrue();
            judiciaryPersonStaging.DeletedOn.Should().Be(request[0].DeletedOn);
        }
    }
}
