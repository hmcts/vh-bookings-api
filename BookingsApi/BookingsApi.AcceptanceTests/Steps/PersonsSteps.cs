using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using AcceptanceTests.Common.Api.Helpers;
using BookingsApi.AcceptanceTests.Contexts;
using BookingsApi.Contract.V1.Requests;
using BookingsApi.Contract.V1.Responses;
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

        [Then(@"a person should be retrieved")]
        public void ThenAPersonShouldBeRetrieved()
        {
            var model = RequestHelper.Deserialise<PersonResponse>(_context.Response.Content);
            model.Should().NotBeNull();
            model.ContactEmail.Should().BeEquivalentTo(_context.TestData.ParticipantsResponses[0].ContactEmail);
            model.FirstName.Should().Be(_context.TestData.ParticipantsResponses[0].FirstName);
            model.Id.Should().NotBeEmpty();
            model.LastName.Should().Be(_context.TestData.ParticipantsResponses[0].LastName);
            model.MiddleNames.Should().Be(_context.TestData.ParticipantsResponses[0].MiddleNames);
            model.TelephoneNumber.Should().Be(_context.TestData.ParticipantsResponses[0].TelephoneNumber);
            model.Title.Should().Be(_context.TestData.ParticipantsResponses[0].Title);
            model.Username.Should().BeEquivalentTo(_context.TestData.ParticipantsResponses[0].Username);
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