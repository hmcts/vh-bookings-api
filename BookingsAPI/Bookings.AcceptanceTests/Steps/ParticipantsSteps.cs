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
    public sealed class ParticipantsSteps
    {
        private readonly TestContext _context;
        private readonly ParticipantsEndpoints _endpoints = new ApiUriFactory().ParticipantsEndpoints;
        private Guid _removedParticipantId;

        public ParticipantsSteps(TestContext context)
        {
            _context = context;
        }

        [Given(@"I have a get participants in a hearing request with a valid hearing id")]
        public void GivenIHaveAGetAParticipantsInAHearingRequestWithAValidHearingId()
        {
            _context.Request = _context.Get(_endpoints.GetAllParticipantsInHearing(_context.HearingId));
        }

        [Given(@"I have an add participant to a hearing request with a valid hearing id")]
        public void GivenIHaveAnAddParticipantToAHearingHearingRequestWithAValidHearingId()
        {
            var addParticipantRequest = AddParticipantRequest.BuildRequest();
            _context.Request = _context.Post(_endpoints.AddParticipantsToHearing(_context.HearingId), addParticipantRequest);
        }

        [Given(@"I have a get a single participant in a hearing request with a valid hearing id")]
        public void GivenIHaveAGetASingleParticipantInAHearingRequestWithAValidHearingId()
        {
            _context.Request = _context.Get(_endpoints.GetParticipantInHearing(_context.HearingId, _context.Participants[0].Id));
        }

        [Given(@"I have a remove participant from a hearing with a valid hearing id")]
        public void GivenIHaveARemoveParticipantFromAHearingWithAValidHearingId()
        {
            _removedParticipantId = _context.Participants[_context.Participants.Count - 1].Id;
            _context.Request = _context.Delete(_endpoints.RemoveParticipantFromHearing(_context.HearingId, _removedParticipantId));
        }

        [Then(@"a list of hearing participants should be retrieved")]
        public void ThenAListOfHearingParticipantsShouldBeRetrieved()
        {
            var model = ApiRequestHelper.DeserialiseSnakeCaseJsonToResponse<List<ParticipantResponse>>(_context.Json);
            CheckParticipantDetailsAreReturned(model);
        }        

        [Then(@"a hearing participant should be retrieved")]
        public void ThenAHearingParticipantShouldBeRetrieved()
        {
            var model = new List<ParticipantResponse>{ApiRequestHelper.DeserialiseSnakeCaseJsonToResponse<ParticipantResponse>(_context.Json)};
            CheckParticipantDetailsAreReturned(model);
        }

        [Then(@"the participant should be (.*)")]
        public void ThenTheParticipantShouldBeAddedOrRemoved(string state)
        {
            _context.Request = _context.Get(_endpoints.GetAllParticipantsInHearing(_context.HearingId));
            _context.Response = _context.Client().Execute(_context.Request);
            _context.Json = _context.Response.Content;
            var model = ApiRequestHelper.DeserialiseSnakeCaseJsonToResponse<List<ParticipantResponse>>(_context.Json);
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
            var participantId = _context.Participants.FirstOrDefault(x=>x.UserRoleName.Equals(role)).Id;
            var updateParticipantRequest = UpdateParticipantRequest.BuildRequest();
            _context.Request = _context.Put(_endpoints.UpdateParticipantDetails(_context.HearingId,participantId), updateParticipantRequest);
        }
        
        [Then(@"'(.*)' details should be updated")]
        public void ThenIndividualDetailsShouldBeUpdated(string participant)
        {
            var model = ApiRequestHelper.DeserialiseSnakeCaseJsonToResponse<ParticipantResponse>(_context.Json);
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
            var participantId = _context.Participants.FirstOrDefault(x => x.UserRoleName.Equals(role)).Id;
            var updateParticipantRequest = UpdateSuitabilityAnswersRequest.BuildRequest();
            _context.Answers = updateParticipantRequest;
            _context.Request = _context.Put(_endpoints.UpdateSuitabilityAnswers(_context.HearingId, participantId), updateParticipantRequest);
        }
    }
}