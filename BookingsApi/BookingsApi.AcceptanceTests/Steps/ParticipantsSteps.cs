using System;
using System.Collections.Generic;
using System.Linq;
using AcceptanceTests.Common.Api.Helpers;
using BookingsApi.AcceptanceTests.Contexts;
using BookingsApi.AcceptanceTests.Models;
using BookingsApi.Contract.Enums;
using BookingsApi.Contract.Requests;
using BookingsApi.Contract.Responses;
using FluentAssertions;
using TechTalk.SpecFlow;
using Testing.Common.Builders.Api.Request;
using static Testing.Common.Builders.Api.ApiUriFactory.ParticipantsEndpoints;
using UpdateParticipantRequest = BookingsApi.AcceptanceTests.Models.UpdateParticipantRequest;

namespace BookingsApi.AcceptanceTests.Steps
{
    [Binding]
    public sealed class ParticipantsSteps
    {
        private readonly TestContext _context;
        private Guid _removedParticipantId;

        public ParticipantsSteps(TestContext context)
        {
            _context = context;
        }

        [Given(@"I have a get participants in a hearing request with a valid hearing id")]
        public void GivenIHaveAGetAParticipantsInAHearingRequestWithAValidHearingId()
        {
            _context.Request = _context.Get(GetAllParticipantsInHearing(_context.TestData.Hearing.Id));
        }

        [Given(@"I have an add participant to a hearing request with a valid hearing id")]
        public void GivenIHaveAnAddParticipantToAHearingHearingRequestWithAValidHearingId()
        {
            var addParticipantRequest = AddParticipantRequest.BuildRequest();
            _context.Request = _context.Post(AddParticipantsToHearing(_context.TestData.Hearing.Id), addParticipantRequest);
        }

        [Given(@"I have a get a single participant in a hearing request with a valid hearing id")]
        public void GivenIHaveAGetASingleParticipantInAHearingRequestWithAValidHearingId()
        {
            _context.Request = _context.Get(GetParticipantInHearing(_context.TestData.Hearing.Id, _context.TestData.ParticipantsResponses[0].Id));
        }

        [Given(@"I remove a participant with interpreter from a hearing with a valid hearing id")]
        public void GivenIRemoveAParticipantWithInterpreterFromAHearingWithAValidHearingId()
        {            
            _removedParticipantId = _removedParticipantId = _context.TestData.ParticipantsResponses.FirstOrDefault(p => p.HearingRoleName != "Interpreter" && p.LinkedParticipants.Any()).Id;
            _context.Request = _context.Delete(RemoveParticipantFromHearing(_context.TestData.Hearing.Id, _removedParticipantId)); _context.Request = _context.Delete(RemoveParticipantFromHearing(_context.TestData.Hearing.Id, _removedParticipantId));
           
        }

        [Given(@"I remove an interpreter from a hearing with a valid hearing id")]
        public void GivenIRemoveAnInterpreterFromAHearingWithAValidHearingId()
        {
            _removedParticipantId = _context.TestData.ParticipantsResponses.FirstOrDefault(p => p.HearingRoleName == "Interpreter" && p.LinkedParticipants.Any()).Id;
            _context.Request = _context.Delete(RemoveParticipantFromHearing(_context.TestData.Hearing.Id, _removedParticipantId));
        }

        [Given(@"I create a request to link 2 distinct participants in the hearing")]
        public void GivenIHaveAnInterpreterLinkedToAParticipant()
        {
            var participants = _context.TestData.Hearing.Participants
                .Where(x => x.UserRoleName.ToLower() == "individual" || x.UserRoleName.ToLower() == "representative").ToList();
            var interpretee = participants[0];
            var interpreter = participants[1];
            
            var linkedParticipant = new LinkedParticipantRequest
            {
                LinkedParticipantContactEmail = interpreter.ContactEmail, 
                ParticipantContactEmail = interpretee.ContactEmail,
                Type = LinkedParticipantType.Interpreter
            };
            
            var request = new UpdateParticipantRequestBuilder().Build();
            request.LinkedParticipants = new List<LinkedParticipantRequest> {linkedParticipant};

            _context.Request = _context.Put(UpdateParticipantDetails(_context.TestData.Hearing.Id, interpretee.Id), request);
        }

        [Then(@"a list of hearing participants should be retrieved")]
        public void ThenAListOfHearingParticipantsShouldBeRetrieved()
        {
            var model = RequestHelper.Deserialise<List<ParticipantResponse>>(_context.Response.Content);
            CheckParticipantDetailsAreReturned(model);
        }        

        [Then(@"a hearing participant should be retrieved")]
        public void ThenAHearingParticipantShouldBeRetrieved()
        {
            var model = new List<ParticipantResponse>{RequestHelper.Deserialise<ParticipantResponse>(_context.Response.Content)};
            CheckParticipantDetailsAreReturned(model);
        }

        [Then(@"the participant should be added")]
        public void ThenTheParticipantShouldBeAdded()
        {
            _context.Request = _context.Get(GetAllParticipantsInHearing(_context.TestData.Hearing.Id));
            _context.Response = _context.Client().Execute(_context.Request);
            var model = RequestHelper.Deserialise<List<ParticipantResponse>>(_context.Response.Content);
            model.Exists(x => x.FirstName == AddParticipantRequest.ParticipantRequestFirstName).Should().BeTrue();
        }

        [Then(@"the participant should be removed")]
        [Then(@"the participant and interpter should be removed")]
        public void ThenTheParticipantShouldBeRemoved()
        {
            _context.Request = _context.Get(GetAllParticipantsInHearing(_context.TestData.Hearing.Id)); _context.Request = _context.Get(GetAllParticipantsInHearing(_context.TestData.Hearing.Id));
            _context.Response = _context.Client().Execute(_context.Request); _context.Response = _context.Client().Execute(_context.Request);
            var model = RequestHelper.Deserialise<List<ParticipantResponse>>(_context.Response.Content); 
            model.Exists(x => x.Id == _removedParticipantId).Should().BeFalse(); model.Exists(x => x.Id == _removedParticipantId).Should().BeFalse();
            model.Exists(x => x.LinkedParticipants.Any()).Should().BeFalse();
        }

        private static void CheckParticipantDetailsAreReturned(IReadOnlyCollection<ParticipantResponse> participantResponses)
        {
            participantResponses.Should().NotBeNull();
            foreach (var participant in participantResponses)
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
            var participantResponses = _context.TestData.ParticipantsResponses;
            var isIndividual = role == "Individual";
            var participant = isIndividual ? participantResponses.FirstOrDefault(x => x.UserRoleName.Equals(role) && !x.LinkedParticipants.Any())
                                    : participantResponses.FirstOrDefault(x => x.UserRoleName.Equals(role));
            var updateParticipantRequest = UpdateParticipantRequest.BuildRequest(); 
            _context.Request = _context.Put(UpdateParticipantDetails(_context.TestData.Hearing.Id, participant.Id), updateParticipantRequest); 
            if (isIndividual)
            {
                var interpreter = participantResponses.FirstOrDefault(x => x.HearingRoleName.Equals("Interpreter"));
                updateParticipantRequest.WithLinkedParticipants(participant.ContactEmail, interpreter.ContactEmail);
            }
            _context.Request = _context.Put(UpdateParticipantDetails(_context.TestData.Hearing.Id, participant.Id), updateParticipantRequest);
        }
        
        [Then(@"'(.*)' details should be updated")]
        public void ThenIndividualDetailsShouldBeUpdated(string participant)
        {
            var model = RequestHelper.Deserialise<ParticipantResponse>(_context.Response.Content);
            var updateParticipantRequest = UpdateParticipantRequest.BuildRequest();
            model.Should().NotBeNull();
            model.Title.Should().Be(updateParticipantRequest.Title);
            model.DisplayName.Should().Be(updateParticipantRequest.DisplayName);
            model.TelephoneNumber.Should().Be(updateParticipantRequest.TelephoneNumber);
            model.LinkedParticipants.Count.Should().Be(participant == "Individual" ? 1 : 0);
        }

        [Given(@"I have an update participant suitability answers with a valid user '(.*)'")]
        public void GivenIHaveAnUpdateParticipantSuitabilityAnswersWithAValidUser(string role)
        {
            var participantId = _context.TestData.ParticipantsResponses.FirstOrDefault(x => x.UserRoleName.Equals(role)).Id;
            var updateParticipantRequest = UpdateSuitabilityAnswersRequest.BuildRequest();
            _context.TestData.Answers = updateParticipantRequest;
            _context.Request = _context.Put(UpdateSuitabilityAnswers(_context.TestData.Hearing.Id, participantId), updateParticipantRequest);
        }
    }
}