using BookingsApi.Contract.V1.Requests;
using BookingsApi.Contract.V1.Responses;
using BookingsApi.Mappings.V1;
using Faker;

namespace BookingsApi.IntegrationTests.Api.V1.JudiciaryPersons
{
    public class PostJudiciaryPersonBySearchTermTests : ApiTest
    {
        private readonly JudiciaryPersonToResponseMapper _mapper = new();
        private const string EmailPrefix = "PostJudiciaryPersonBySearchTermTests_";
        
        [Test]
        public async Task Should_filter_out_inactive_persons()
        {
            // Arrange
            var activePerson = await Hooks.AddJudiciaryPerson(Guid.NewGuid().ToString(), email: GenerateEmail());
            // Inactive persons
            await Hooks.AddLeaverJudiciaryPerson(Guid.NewGuid().ToString(), "2023-01-01", email: GenerateEmail());
            await Hooks.AddDeletedJudiciaryPerson(Guid.NewGuid().ToString(), "2023-02-01", email: GenerateEmail());
            var request = new SearchTermRequest(EmailPrefix);
            
            // Act
            using var client = Application.CreateClient();
            var result = await client.PostAsync(
                ApiUriFactory.JudiciaryPersonsEndpoints.PostJudiciaryPersonBySearchTerm(), 
                RequestBody.Set(request));
            
            // Assert
            result.IsSuccessStatusCode.Should().BeTrue();
            result.StatusCode.Should().Be(HttpStatusCode.OK);
            var response = await ApiClientResponse.GetResponses<List<JudiciaryPersonResponse>>(result.Content);

            response.Count.Should().Be(1);
            response.Should().ContainEquivalentOf(_mapper.MapJudiciaryPersonToResponse(activePerson));
        }
        
        private static string GenerateEmail() => $"{EmailPrefix}_${RandomNumber.Next()}@email.com";
    }
}
