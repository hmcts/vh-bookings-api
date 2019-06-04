using Bookings.AcceptanceTests.Contexts;
using Bookings.Api.Contract.Responses;
using FluentAssertions;
using System.Collections.Generic;
using System.Linq;
using TechTalk.SpecFlow;
using Testing.Common.Builders.Api;

namespace Bookings.AcceptanceTests.Steps
{
    [Binding]
    public sealed class PersonsSteps
    {
        private readonly TestContext _acTestContext;
        private readonly PersonEndpoints _endpoints = new ApiUriFactory().PersonEndpoints;

        public PersonsSteps(TestContext acTestContext)
        {
            _acTestContext = acTestContext;
        }

        [Given(@"I have a get a person by username request with a valid username")]
        public void GivenIHaveAGetAPersonByUsernameRequestWithAValidUsername()
        {
            _acTestContext.Request = _acTestContext.Get(_endpoints.GetPersonByUsername(_acTestContext.Participants[0].Username));
        }
        
        [Given(@"I have a get a person by contact email request with a valid email")]
        public void GivenIHaveAGetAPersonByContactEmailRequestWithAValidEmail()
        {
            _acTestContext.Request = _acTestContext.Get(_endpoints.GetPersonByContactEmail(_acTestContext.Participants[0].ContactEmail));
        }

        [Then(@"a person should be retrieved")]
        public void ThenAPersonShouldBeRetrieved()
        {
            var model = ApiRequestHelper.DeserialiseSnakeCaseJsonToResponse<PersonResponse>(_acTestContext.Json);
            model.Should().NotBeNull();
            model.ContactEmail.Should().Be(_acTestContext.Participants[0].ContactEmail);
            model.FirstName.Should().Be(_acTestContext.Participants[0].FirstName);
            model.Id.Should().NotBeEmpty();
            model.LastName.Should().Be(_acTestContext.Participants[0].LastName);
            model.MiddleNames.Should().Be(_acTestContext.Participants[0].MiddleNames);
            model.TelephoneNumber.Should().Be(_acTestContext.Participants[0].TelephoneNumber);
            model.Title.Should().Be(_acTestContext.Participants[0].Title);
            model.Username.Should().Be(_acTestContext.Participants[0].Username);
        }
        [Given(@"I have a get a person by search term request with a valid search term")]
        public void GivenIHaveAGetAPersonBySearchTermRequestWithAValidSearchTerm()
        {
            var contactEmail = _acTestContext.Participants[0].ContactEmail;
            var searchTerm = contactEmail.Substring(0, 3);
            _acTestContext.Request = _acTestContext.Get(_endpoints.GetPersonBySearchTerm(searchTerm));
        }
        [Then(@"persons details should be retrieved")]
        public void ThenPersonsDetailsShouldBeRetrieved()
        {
            var model = ApiRequestHelper.DeserialiseSnakeCaseJsonToResponse<List<PersonResponse>>(_acTestContext.Json);         
            var expected = _acTestContext.Participants[0];
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
    }
}