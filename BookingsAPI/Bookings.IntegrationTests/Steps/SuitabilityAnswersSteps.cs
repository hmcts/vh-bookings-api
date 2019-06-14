using Bookings.Api.Contract.Responses;
using FluentAssertions;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using TechTalk.SpecFlow;
using Testing.Common.Builders.Api;

namespace Bookings.IntegrationTests.Steps
{
    [Binding]
    public sealed class SuitabilityAnswersSteps : StepsBase
    {
        private readonly SuitabilityAnswerEndpoints _endpoints = new ApiUriFactory().SuitabilityAnswerEndpoints;

        public SuitabilityAnswersSteps(Contexts.TestContext apiTestContext) : base(apiTestContext)
        {
        }

        [Given(@"I have the suitable answers for participants")]
        public async Task GivenIHaveTheSuitableAnswersForParticipants()
        {
            await ApiTestContext.TestDataManager.SeedVideoHearing(true);
            ApiTestContext.Uri = _endpoints.GetSuitabilityAnswers("");
            ApiTestContext.HttpMethod = HttpMethod.Get;
        }


        [Then(@"suitable answers should be retrieved")]
        public async Task ThenSuitableAnswersShouldBeRetrieved()
        {
            var json = await ApiTestContext.ResponseMessage.Content.ReadAsStringAsync();
            var model = ApiRequestHelper.DeserialiseSnakeCaseJsonToResponse<SuitabilityAnswersResponse>(json);
            model.Should().NotBeNull();
            model.ParticipantSuitabilityAnswerResponse.Should().NotBeNull();
            model.ParticipantSuitabilityAnswerResponse.Count.Should().BeGreaterThan(0);
            var participantAnswer = model.ParticipantSuitabilityAnswerResponse[0];
            participantAnswer.FirstName.Should().NotBeEmpty();
            participantAnswer.LastName.Should().NotBeEmpty();
            participantAnswer.HearingRole.Should().NotBeEmpty();
            participantAnswer.Answers.Should().NotBeNull();
            participantAnswer.Answers.Count.Should().BeGreaterThan(0);
            participantAnswer.UpdatedAt.Should().Be(model.ParticipantSuitabilityAnswerResponse.Max(s => s.UpdatedAt));
        }

        [Given(@"I have a request to the second set of suitable answers")]
        public async Task GivenIHaveARequestToTheSecondSetOfSuitableAnswers()
        {
            await ApiTestContext.TestDataManager.SeedVideoHearing(true);
            await ApiTestContext.TestDataManager.SeedVideoHearing(true);

            ApiTestContext.Uri = _endpoints.GetSuitabilityAnswerWithLimit("", 1);
            ApiTestContext.HttpMethod = HttpMethod.Get;
            var response = await SendGetRequestAsync(ApiTestContext);
            var json = await response.Content.ReadAsStringAsync();
            var bookings = ApiRequestHelper.DeserialiseSnakeCaseJsonToResponse<SuitabilityAnswersResponse>(json);

            ApiTestContext.Uri = _endpoints.GetSuitabilityAnswerWithLimit(bookings.NextCursor, 1);
        }
    }
}
