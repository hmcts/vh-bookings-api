using Bookings.Api.Contract.Responses;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
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

        [Given(@"I have a get suitable answers for participants")]
        public async Task GivenIHaveAGetSuitableAnswersForParticipants()
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
    }
}
