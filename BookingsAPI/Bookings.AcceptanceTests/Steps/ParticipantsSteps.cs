using System;
using System.Collections.Generic;
using System.Linq;
using Bookings.AcceptanceTests.Contexts;
using Bookings.AcceptanceTests.Models;
using Bookings.Api.Contract.Responses;
using FluentAssertions;
using TechTalk.SpecFlow;
using Testing.Common.Builders.Api;

namespace Bookings.AcceptanceTests.Steps
{
    [Binding]
    public sealed class ParticipantsSteps : BaseSteps
    {
        private readonly TestContext _acTestContext;
        private readonly ParticipantsEndpoints _endpoints = new ApiUriFactory().ParticipantsEndpoints;
        private Guid _removedParticipantId;

        public ParticipantsSteps(TestContext acTestContext)
        {
            _acTestContext = acTestContext;
        }

        [Given(@"I have a get participants in a hearing request with a valid hearing id")]
        public void GivenIHaveAGetAParticipantsInAHearingRequestWithAValidHearingId()
        {
            _acTestContext.Request = _acTestContext.Get(_endpoints.GetAllParticipantsInHearing(_acTestContext.HearingId));
        }

        [Given(@"I have an add participant to a hearing request with a valid hearing id")]
        public void GivenIHaveAnAddParticipantToAHearingHearingRequestWithAValidHearingId()
        {
            var addParticipantRequest = AddParticipantRequest.BuildRequest();
            _acTestContext.Request = _acTestContext.Post(_endpoints.AddParticipantsToHearing(_acTestContext.HearingId), addParticipantRequest);
        }

        [Given(@"I have a get a single participant in a hearing request with a valid hearing id")]
        public void GivenIHaveAGetASingleParticipantInAHearingRequestWithAValidHearingId()
        {
            _acTestContext.Request = _acTestContext.Get(_endpoints.GetParticipantInHearing(_acTestContext.HearingId, _acTestContext.Participants[0].Id));
        }

        [Given(@"I have a remove participant from a hearing with a valid hearing id")]
        public void GivenIHaveARemoveParticipantFromAHearingWithAValidHearingId()
        {
            _removedParticipantId = _acTestContext.Participants[_acTestContext.Participants.Count - 1].Id;
            _acTestContext.Request = _acTestContext.Delete(_endpoints.RemoveParticipantFromHearing(_acTestContext.HearingId, _removedParticipantId));
        }

        [Then(@"a list of hearing participants should be retrieved")]
        public void ThenAListOfHearingParticipantsShouldBeRetrieved()
        {
            var model = ApiRequestHelper.DeserialiseSnakeCaseJsonToResponse<List<ParticipantResponse>>(_acTestContext.Json);
            CheckParticipantDetailsAreReturned(model);
        }        

        [Then(@"a hearing participant should be retrieved")]
        public void ThenAHearingParticipantShouldBeRetrieved()
        {
            var model = new List<ParticipantResponse>{ApiRequestHelper.DeserialiseSnakeCaseJsonToResponse<ParticipantResponse>(_acTestContext.Json)};
            CheckParticipantDetailsAreReturned(model);
        }

        [Then(@"the participant should be (.*)")]
        public void ThenTheParticipantShouldBeAddedOrRemoved(string state)
        {
            _acTestContext.Request = _acTestContext.Get(_endpoints.GetAllParticipantsInHearing(_acTestContext.HearingId));
            _acTestContext.Response = _acTestContext.Client().Execute(_acTestContext.Request);
            _acTestContext.Json = _acTestContext.Response.Content;
            var model = ApiRequestHelper.DeserialiseSnakeCaseJsonToResponse<List<ParticipantResponse>>(_acTestContext.Json);
            if (state.Equals("added"))
            {
                var exists = model.Exists(x => x.FirstName == "Added Participant");
                exists.Should().BeTrue();
            }
            if (!state.Equals("removed")) return;
            {
                var exists = model.Exists(x => x.Id == _removedParticipantId);
                exists.Should().BeFalse();
            }
        }

        private static void CheckParticipantDetailsAreReturned(IReadOnlyCollection<ParticipantResponse> model)
        {
            model.Should().NotBeNull();
            foreach (var participant in model)
            {
                participant.CaseRoleName.Should().NotBeNullOrEmpty();
                participant.ContactEmail.Should().NotBeNullOrEmpty();
                participant.DisplayName.Should().NotBeNullOrEmpty();
                participant.FirstName.Should().NotBeNullOrEmpty();
                participant.HearingRoleName.Should().NotBeNullOrEmpty();
                participant.Id.Should().NotBeEmpty();
                participant.LastName.Should().NotBeNullOrEmpty();
                participant.MiddleNames.Should().NotBeNullOrEmpty();
                participant.TelephoneNumber.Should().NotBeNullOrEmpty();
                participant.Title.Should().NotBeNullOrEmpty();
                participant.UserRoleName.Should().NotBeNullOrEmpty();
                participant.Username.Should().NotBeNullOrEmpty();
            }
        }
      
        [Given(@"I have an update participant details request with a valid user (.*)")]
        public void GivenIHaveAnUpdateParticipantDetailsRequestWithAValidUserRole(string role)
        {
            var participantId = _acTestContext.Participants.FirstOrDefault(x=>x.UserRoleName.Equals(role)).Id;
            var updateParticipantRequest = UpdateParticipantRequest.BuildRequest();
            _acTestContext.Request = _acTestContext.Put(_endpoints.UpdateParticipantDetails(_acTestContext.HearingId,participantId), updateParticipantRequest);
        }
        
        [Then(@"'(.*)' details should be updated")]
        public void ThenIndividualDetailsShouldBeUpdated(string participant)
        {
            var model = ApiRequestHelper.DeserialiseSnakeCaseJsonToResponse<ParticipantResponse>(_acTestContext.Json);
            var updateParticipantRequest = UpdateParticipantRequest.BuildRequest();
            model.Should().NotBeNull();
            model.Title.Should().Be(updateParticipantRequest.Title);
            model.DisplayName.Should().Be(updateParticipantRequest.DisplayName);
            model.TelephoneNumber.Should().Be(updateParticipantRequest.TelephoneNumber);
            if (participant.Equals("Representative"))
            {
                model.HouseNumber.Should().BeNull();
                model.Street.Should().BeNull();
                model.City.Should().BeNull();
                model.County.Should().BeNull();
                model.Postcode.Should().BeNull();
            }
        }

        [Given(@"I have an update participant suitability answers with a valid user '(.*)'")]
        public void GivenIHaveAnUpdateParticipantSuitabilityAnswersWithAValidUser(string role)
        {
            var participantId = _acTestContext.Participants.FirstOrDefault(x => x.UserRoleName.Equals(role)).Id;
            var updateParticipantRequest = UpdateSuitabilityAnswersRequest.BuildRequest();
            _acTestContext.Answers = updateParticipantRequest;
            _acTestContext.Request = _acTestContext.Put(_endpoints.UpdateSuitabilityAnswers(_acTestContext.HearingId, participantId), updateParticipantRequest);
        }
    }
}