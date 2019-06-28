using Bookings.AcceptanceTests.Contexts;
using Bookings.AcceptanceTests.Models;
using Bookings.Api.Contract.Responses;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using TechTalk.SpecFlow;
using Testing.Common.Builders.Api;

namespace Bookings.AcceptanceTests.Steps
{
    [Binding]
    public sealed class SuitabilityAnswersSteps
    {
        private readonly TestContext _acTestContext;
        private readonly ParticipantsEndpoints _endpoints = new ApiUriFactory().ParticipantsEndpoints;
        private readonly SuitabilityAnswerEndpoints _suitabilityEndpoints = new ApiUriFactory().SuitabilityAnswerEndpoints;
        private readonly HearingsEndpoints _hearingEndpoints = new ApiUriFactory().HearingsEndpoints;

        public SuitabilityAnswersSteps(TestContext acTestContext)
        {
            _acTestContext = acTestContext;
        }

        [Given(@"I have the suitable answers submitted")]
        public void GivenIHaveTheSuitableAnswersForParticipants()
        {
            AddSuitabilityAnswersToParticipants(_acTestContext.HearingId, _acTestContext.Participants);
        }

        [Given(@"I have a request to get the suitable answers")]
        public void GivenIHaveARequestToGetTheSuitableAnswers()
        {
            _acTestContext.Request = _acTestContext.Get(_suitabilityEndpoints.GetSuitabilityAnswers(""));
        }

        [Given(@"I have a request to the second set of suitable answers")]
        public void GivenIHaveARequestToTheSecondSetOfSuitableAnswers()
        {
            //Create hearing and submit answers to participants
            AddSuitabilityAnswersToHearing();
            AddSuitabilityAnswersToHearing();
            
            //Get first set of the suitability answers
            var suitabilityGetRequest = _acTestContext.Get(_suitabilityEndpoints.GetSuitabilityAnswerWithLimit("", 1));
            var response = _acTestContext.Client().Execute(suitabilityGetRequest);
            var firstSetOfRecords = ApiRequestHelper.DeserialiseSnakeCaseJsonToResponse<SuitabilityAnswersResponse>(response.Content);

            //Get 2nd set of the suitability answers
            _acTestContext.Request = _acTestContext.Get(_suitabilityEndpoints.GetSuitabilityAnswerWithLimit(firstSetOfRecords.NextCursor, 1));
        }


        [Then(@"suitable answers should be retrieved")]
        public void ThenSuitableAnswersShouldBeRetrieved()
        {
            var model = ApiRequestHelper.DeserialiseSnakeCaseJsonToResponse<SuitabilityAnswersResponse>(_acTestContext.Json);
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

        private void AddSuitabilityAnswersToParticipantsOfAHearing()
        {
            var request = _acTestContext.Post(_hearingEndpoints.BookNewHearing(), CreateHearingRequest.BuildRequest());
            var response = _acTestContext.Client().Execute(request);
            var model = ApiRequestHelper.DeserialiseSnakeCaseJsonToResponse<HearingDetailsResponse>(response.Content);
            AddSuitabilityAnswersToParticipants( model.Id, model.Participants);
        }

        private void AddSuitabilityAnswersToParticipants(Guid hearingId, List<ParticipantResponse> participants)
        {
            //Add suitability answers for all the participants
            foreach (var participant in participants)
            {
                var updateParticipantRequest = UpdateSuitabilityAnswersRequest.BuildRequest();
                _acTestContext.Answers = updateParticipantRequest;
                _acTestContext.Request = _acTestContext.Put(_endpoints.UpdateSuitabilityAnswers(hearingId, participant.Id), updateParticipantRequest);
                _acTestContext.Response = _acTestContext.Client().Execute(_acTestContext.Request);
                _acTestContext.Json = _acTestContext.Response.Content;
            }
        }
    }
}