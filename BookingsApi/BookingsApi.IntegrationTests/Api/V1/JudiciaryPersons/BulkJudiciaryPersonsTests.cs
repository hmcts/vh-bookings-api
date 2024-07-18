using BookingsApi.Contract.V1.Requests;
using BookingsApi.Contract.V1.Responses;
using BookingsApi.DAL.Queries;

namespace BookingsApi.IntegrationTests.Api.V1.JudiciaryPersons
{
    public class BulkJudiciaryPersonsTests : ApiTest
    {
        [TestCase(true)]
        [TestCase(false)]
        public async Task Should_process_deleted_judiciary_person(bool alreadyExists)
        {
            // Arrange
            var personalCode = Guid.NewGuid().ToString();
            if (alreadyExists)
                await Hooks.AddJudiciaryPerson(personalCode);
            var request = new List<JudiciaryPersonRequest>
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
                ApiUriFactory.JudiciaryPersonsEndpoints.BulkJudiciaryPersons(), 
                RequestBody.Set(request));
            
            // Assert
            result.IsSuccessStatusCode.Should().BeTrue();
            result.StatusCode.Should().Be(HttpStatusCode.OK);
            var response = await ApiClientResponse.GetResponses<BulkJudiciaryPersonResponse>(result.Content);
            response.ErroredRequests.Should().BeEmpty();
            
            await using var db = new BookingsDbContext(BookingsDbContextOptions);
            var judiciaryPerson = await new GetJudiciaryPersonByPersonalCodeQueryHandler(db).Handle(new GetJudiciaryPersonByPersonalCodeQuery(personalCode));
            judiciaryPerson.Should().NotBeNull();
            judiciaryPerson.Deleted.Should().BeTrue();
            judiciaryPerson.DeletedOn.Should().Be(request[0].DeletedOn);
        }
    }
}
