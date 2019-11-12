using Bookings.AcceptanceTests.Contexts;
using Bookings.Api.Contract.Responses;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using TechTalk.SpecFlow;
using Testing.Common.Builders.Api;

namespace Bookings.AcceptanceTests.Steps
{
    [Binding]
    public sealed class PersonsSteps
    {
        private readonly TestContext _context;
        private readonly PersonEndpoints _endpoints = new ApiUriFactory().PersonEndpoints;

        public PersonsSteps(TestContext context)
        {
            _context = context;
        }

        [Given(@"I have a get a person by username request with a valid username")]
        public void GivenIHaveAGetAPersonByUsernameRequestWithAValidUsername()
        {
            _context.Request = _context.Get(_endpoints.GetPersonByUsername(_context.Participants[0].Username));
        }

        [Given(@"I have a get a person by contact email request with a valid email")]
        public void GivenIHaveAGetAPersonByContactEmailRequestWithAValidEmail()
        {
            _context.Request = _context.Get(_endpoints.GetPersonByContactEmail(_context.Participants[0].ContactEmail));
        }

        [Then(@"a person should be retrieved")]
        public void ThenAPersonShouldBeRetrieved()
        {
            var model = ApiRequestHelper.DeserialiseSnakeCaseJsonToResponse<PersonResponse>(_context.Json);
            model.Should().NotBeNull();
            model.ContactEmail.Should().Be(_context.Participants[0].ContactEmail);
            model.FirstName.Should().Be(_context.Participants[0].FirstName);
            model.Id.Should().NotBeEmpty();
            model.LastName.Should().Be(_context.Participants[0].LastName);
            model.MiddleNames.Should().Be(_context.Participants[0].MiddleNames);
            model.TelephoneNumber.Should().Be(_context.Participants[0].TelephoneNumber);
            model.Title.Should().Be(_context.Participants[0].Title);
            model.Username.Should().Be(_context.Participants[0].Username);
        }
        [Given(@"I have a get a person by search term request with a valid search term")]
        public void GivenIHaveAGetAPersonBySearchTermRequestWithAValidSearchTerm()
        {
            var contactEmail = _context.Participants[0].ContactEmail;
            var searchTerm = contactEmail.Substring(0, 3);
            _context.Request = _context.Post(_endpoints.PostPersonBySearchTerm(searchTerm), searchTerm);
        }
        [Then(@"persons details should be retrieved")]
        public void ThenPersonsDetailsShouldBeRetrieved()
        {
            var model = ApiRequestHelper.DeserialiseSnakeCaseJsonToResponse<List<PersonResponse>>(_context.Json);
            var expected = _context.Participants[0];
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
            var username = _context.Participants.FirstOrDefault(x => x.UserRoleName.Equals(participant)).Username;
            var participantId = _context.Participants.FirstOrDefault(x => x.UserRoleName.Equals(participant)).Id;
            _context.Request = _context.Get(_endpoints.GetPersonSuitabilityAnswers(username));
            _context.Response = _context.Client().Execute(_context.Request);

            if (_context.Response.Content != null)
            {
                _context.Json = _context.Response.Content;
            }

            _context.Response.StatusCode.Should().Be(HttpStatusCode.OK);
            var model = ApiRequestHelper.DeserialiseSnakeCaseJsonToResponse<List<PersonSuitabilityAnswerResponse>>(_context.Json);
            var expectedResult = _context.Answers;

            expectedResult[0].Key.Should().Be(model[0].Answers[0].Key);
            expectedResult[0].Answer.Should().Be(model[0].Answers[0].Answer);
            expectedResult[0].ExtendedAnswer.Should().Be(model[0].Answers[0].ExtendedAnswer);
            expectedResult[1].Key.Should().Be(model[0].Answers[1].Key);
            expectedResult[1].Answer.Should().Be(model[0].Answers[1].Answer);
            expectedResult[1].ExtendedAnswer.Should().Be(model[0].Answers[1].ExtendedAnswer);

            model[0].HearingId.Should().Be(_context.HearingId);
            model[0].ParticipantId.Should().Be(participantId);
            model[0].UpdatedAt.Should().BeAfter(DateTime.UtcNow.AddMinutes(-2));
            model[0].QuestionnaireNotRequired.Should().Be(false);
        }
    }
}