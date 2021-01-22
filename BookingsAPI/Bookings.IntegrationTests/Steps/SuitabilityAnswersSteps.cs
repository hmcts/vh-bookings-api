﻿using Bookings.Api.Contract.Responses;
using FluentAssertions;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using AcceptanceTests.Common.Api.Helpers;
using TechTalk.SpecFlow;
using static Testing.Common.Builders.Api.ApiUriFactory.SuitabilityAnswerEndpoints;

namespace Bookings.IntegrationTests.Steps
{
    [Binding]
    public sealed class SuitabilityAnswersBaseSteps : BaseSteps
    {
        public SuitabilityAnswersBaseSteps(Contexts.TestContext apiTestContext) : base(apiTestContext)
        {
        }

        [Given(@"I have the suitable answers for participants")]
        public async Task GivenIHaveTheSuitableAnswersForParticipants()
        {
            var seededHearing = await Context.TestDataManager.SeedVideoHearing(true);
            Context.TestData.NewHearingId = seededHearing.Id;
            Context.Uri = GetSuitabilityAnswers("");
            Context.HttpMethod = HttpMethod.Get;
        }

        [Then(@"suitable answers should be retrieved")]
        public async Task ThenSuitableAnswersShouldBeRetrieved()
        {
            var json = await Context.Response.Content.ReadAsStringAsync();
            var model = RequestHelper.DeserialiseSnakeCaseJsonToResponse<SuitabilityAnswersResponse>(json);
            model.Should().NotBeNull();
            model.PrevPageUrl.Should().NotBe(model.NextPageUrl);
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
            var firstHearing =  await Context.TestDataManager.SeedVideoHearing(true);
            var secondHearing = await Context.TestDataManager.SeedVideoHearing(true);

            Context.TestData.OldHearingId = firstHearing.Id;
            Context.TestData.NewHearingId = secondHearing.Id;
            
            Context.Uri = GetSuitabilityAnswerWithLimit("", 1);
            Context.HttpMethod = HttpMethod.Get;
            var response = await SendGetRequestAsync(Context);
            var json = await response.Content.ReadAsStringAsync();
            var bookings = RequestHelper.DeserialiseSnakeCaseJsonToResponse<SuitabilityAnswersResponse>(json);

            Context.Uri = GetSuitabilityAnswerWithLimit(bookings.NextCursor, 1);
        }
    }
}
