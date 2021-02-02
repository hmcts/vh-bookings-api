using BookingsApi.Contract.Requests;
using BookingsApi.Contract.Responses;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using AcceptanceTests.Common.Api.Helpers;
using BookingsApi.AcceptanceTests.Contexts;
using TechTalk.SpecFlow;
using static Testing.Common.Builders.Api.ApiUriFactory.PersonEndpoints;

namespace BookingsApi.AcceptanceTests.Steps
{
    [Binding]
    public sealed class PersonsSteps
    {
        private readonly TestContext _context;

        public PersonsSteps(TestContext context)
        {
            _context = context;
        }

        [Given(@"I have a get a person by username request with a valid username")]
        public void GivenIHaveAGetAPersonByUsernameRequestWithAValidUsername()
        {
            _context.Request = _context.Get(GetPersonByUsername(_context.TestData.ParticipantsResponses[0].Username));
        }

        [Given(@"I have a get a person by contact email request with a valid email")]
        public void GivenIHaveAGetAPersonByContactEmailRequestWithAValidEmail()
        {
            _context.Request =
                _context.Get(GetPersonByContactEmail(_context.TestData.ParticipantsResponses[0].ContactEmail));
        }
        
        [Given(@"I have a search for hearings by username for removal request")]
        public void GivenIHaveASearchForHearingsByUsernameForRemovalRequest()
        {
            var participant = _context.TestData.ParticipantsResponses.First(x => x.UserRoleName.ToLower() != "judge");
            _context.Request =
                _context.Get(GetHearingsByUsernameForDeletion(participant.Username));
        }

        [Then(@"a person should be retrieved")]
        public void ThenAPersonShouldBeRetrieved()
        {
            var model = RequestHelper.Deserialise<PersonResponse>(_context.Response.Content);
            model.Should().NotBeNull();
            model.ContactEmail.Should().Be(_context.TestData.ParticipantsResponses[0].ContactEmail);
            model.FirstName.Should().Be(_context.TestData.ParticipantsResponses[0].FirstName);
            model.Id.Should().NotBeEmpty();
            model.LastName.Should().Be(_context.TestData.ParticipantsResponses[0].LastName);
            model.MiddleNames.Should().Be(_context.TestData.ParticipantsResponses[0].MiddleNames);
            model.TelephoneNumber.Should().Be(_context.TestData.ParticipantsResponses[0].TelephoneNumber);
            model.Title.Should().Be(_context.TestData.ParticipantsResponses[0].Title);
            model.Username.Should().Be(_context.TestData.ParticipantsResponses[0].Username);
        }

        [Given(@"I have a get a person by search term request with a valid search term")]
        public void GivenIHaveAGetAPersonBySearchTermRequestWithAValidSearchTerm()
        {
            var contactEmail = _context.TestData.ParticipantsResponses[0].ContactEmail;
            var searchTerm = new SearchTermRequest(contactEmail.Substring(0, 3));
            _context.Request = _context.Post(PostPersonBySearchTerm, searchTerm);
        }
        
        [Given(@"I have a search for an individual request for an individual")]
        public void GivenIHaveASearchForAIndividualRequestForAnIndividual()
        {
            var participant = _context.TestData.ParticipantsResponses.First(x=> x.UserRoleName == "Individual");
            _context.Request =
                _context.Get(SearchForNonJudicialPersonsByContactEmail(participant.ContactEmail));
        }
        
        [Given(@"I have a search for an individual request for a judge")]
        public void GivenIHaveASearchForAIndividualRequestForAJudge()
        {
            var participant = _context.TestData.ParticipantsResponses.First(x=> x.UserRoleName == "Judge");
            _context.Request =
                _context.Get(SearchForNonJudicialPersonsByContactEmail(participant.ContactEmail));
        }

        [Then(@"persons details should be retrieved")]
        public void ThenPersonsDetailsShouldBeRetrieved()
        {
            var model =
                RequestHelper.Deserialise<List<PersonResponse>>(_context.Response.Content);
            var expected = _context.TestData.ParticipantsResponses[0];
            var actual = model.Single(u => u.ContactEmail == expected.ContactEmail);
            actual.Should().NotBeNull();
            actual.ContactEmail.Should().Be(expected.ContactEmail);
            actual.FirstName.Should().Be(expected.FirstName);
            actual.Id.Should().NotBeEmpty();
            actual.LastName.Should().Be(expected.LastName);
            actual.MiddleNames.Should().Be(expected.MiddleNames);
            actual.TelephoneNumber.Should().Be(expected.TelephoneNumber);
            actual.Title.Should().Be(expected.Title);
            actual.Username.Should().Be(expected.Username);
        }

        [Then(@"suitability answers for '(.*)' should be updated")]
        public void ThenSuitabilityAnswersForShouldBeUpdated(string participant)
        {
            var username = _context.TestData.ParticipantsResponses
                .FirstOrDefault(x => x.UserRoleName.Equals(participant)).Username;
            var participantId = _context.TestData.ParticipantsResponses
                .FirstOrDefault(x => x.UserRoleName.Equals(participant)).Id;
            _context.Request = _context.Get(GetPersonSuitabilityAnswers(username));
            _context.Response = _context.Client().Execute(_context.Request);
            _context.Response.StatusCode.Should().Be(HttpStatusCode.OK);
            var model =
                RequestHelper.Deserialise<List<PersonSuitabilityAnswerResponse>>(
                    _context.Response.Content);
            var expectedResult = _context.TestData.Answers;

            expectedResult[0].Key.Should().Be(model[0].Answers[0].Key);
            expectedResult[0].Answer.Should().Be(model[0].Answers[0].Answer);
            expectedResult[0].ExtendedAnswer.Should().Be(model[0].Answers[0].ExtendedAnswer);
            expectedResult[1].Key.Should().Be(model[0].Answers[1].Key);
            expectedResult[1].Answer.Should().Be(model[0].Answers[1].Answer);
            expectedResult[1].ExtendedAnswer.Should().Be(model[0].Answers[1].ExtendedAnswer);

            model[0].HearingId.Should().Be(_context.TestData.Hearing.Id);
            model[0].ParticipantId.Should().Be(participantId);
            model[0].UpdatedAt.Should().BeAfter(DateTime.UtcNow.AddMinutes(-2));
            model[0].QuestionnaireNotRequired.Should().BeFalse();
        }

        [Then(@"a list of hearings for deletion is (.*)")]
        public void ThenAListOfHearingsForDeletionIs(int expectedNumOfHearings)
        {
            var response =
                RequestHelper.Deserialise<List<HearingsByUsernameForDeletionResponse>>(
                    _context.Response.Content);

            response.Count.Should().Be(expectedNumOfHearings);

            if (expectedNumOfHearings == 1)
            {
                var result = response[0];
                var hearing = _context.TestData.Hearing;
                var leadCase = hearing.Cases.FirstOrDefault(x => x.IsLeadCase) ?? hearing.Cases.First();
                result.HearingId.Should().Be(hearing.Id);
                result.Venue.Should().Be(hearing.HearingVenueName);
                result.ScheduledDateTime.Should().Be(hearing.ScheduledDateTime);
                result.CaseName.Should().Be(leadCase.Name);
                result.CaseNumber.Should().Be(leadCase.Number);
            }
        }
    }
}