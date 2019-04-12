using Bookings.AcceptanceTests.Contexts;
using Bookings.Api.Contract.Responses;
using FluentAssertions;
using System.Collections.Generic;
using TechTalk.SpecFlow;
using Testing.Common.Builders.Api;

namespace Bookings.AcceptanceTests.Steps
{
    [Binding]
    public sealed class PersonsSteps : BaseSteps
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
           _acTestContext.Request = _acTestContext.Get(_endpoints.GetPersonBySearchTerm(_acTestContext.Participants[0].ContactEmail));
        }
        [Then(@"persons details should be retrieved")]
        public void ThenPersonsDetailsShouldBeRetrieved()
        {
            var model = ApiRequestHelper.DeserialiseSnakeCaseJsonToResponse<List<PersonResponse>>(_acTestContext.Json);
            model[0].Should().NotBeNull();
            model[0].ContactEmail.Should().Be(_acTestContext.Participants[0].ContactEmail);
            model[0].FirstName.Should().Be(_acTestContext.Participants[0].FirstName);
            model[0].Id.Should().NotBeEmpty();
            model[0].LastName.Should().Be(_acTestContext.Participants[0].LastName);
            model[0].MiddleNames.Should().Be(_acTestContext.Participants[0].MiddleNames);
            model[0].TelephoneNumber.Should().Be(_acTestContext.Participants[0].TelephoneNumber);
            model[0].Title.Should().Be(_acTestContext.Participants[0].Title);
            model[0].Username.Should().Be(_acTestContext.Participants[0].Username);
        }
    }
}