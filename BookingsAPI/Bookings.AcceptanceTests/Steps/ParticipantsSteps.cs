using System;
using System.Collections.Generic;
using System.Linq;
using AcceptanceTests.Common.Api.Helpers;
using Bookings.AcceptanceTests.Contexts;
using Bookings.AcceptanceTests.Models;
using Bookings.Api.Contract.Requests;
using Bookings.Api.Contract.Responses;
using Bookings.Domain;
using Bookings.Domain.Enumerations;
using FluentAssertions;
using TechTalk.SpecFlow;
using Testing.Common.Builders.Api.Request;
using static Testing.Common.Builders.Api.ApiUriFactory.ParticipantsEndpoints;
using UpdateParticipantRequest = Bookings.AcceptanceTests.Models.UpdateParticipantRequest;

namespace Bookings.AcceptanceTests.Steps
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

        [Given(@"I have a remove participant from a hearing with a valid hearing id")]
        public void GivenIHaveARemoveParticipantFromAHearingWithAValidHearingId()
        {
            _removedParticipantId = _context.TestData.ParticipantsResponses[^1].Id;
            _context.Request = _context.Delete(RemoveParticipantFromHearing(_context.TestData.Hearing.Id, _removedParticipantId));
        }
        
        [Given(@"I have an interpreter linked to a participant")]
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
            model.Exists(x => x.FirstName == "Automation_Added Participant").Should().BeTrue();
        }

        [Then(@"the participant should be removed")]
        public void ThenTheParticipantShouldBeRemoved()
        {
            _context.Request = _context.Get(GetAllParticipantsInHearing(_context.TestData.Hearing.Id));
            _context.Response = _context.Client().Execute(_context.Request);
            var model = RequestHelper.Deserialise<List<ParticipantResponse>>(_context.Response.Content);
            model.Exists(x => x.Id == _removedParticipantId).Should().BeFalse();
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
            var participantId = _context.TestData.ParticipantsResponses.FirstOrDefault(x=>x.UserRoleName.Equals(role)).Id;
            var updateParticipantRequest = UpdateParticipantRequest.BuildRequest();
            _context.Request = _context.Put(UpdateParticipantDetails(_context.TestData.Hearing.Id,participantId), updateParticipantRequest);
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