using Bookings.AcceptanceTests.Contexts;
using Bookings.Api.Contract.Responses;
using FluentAssertions;
using TechTalk.SpecFlow;
using Testing.Common.Builders.Api;

namespace Bookings.AcceptanceTests.Steps
{
    [Binding]
    public sealed class PersonsSteps : StepsBase
    {
        private readonly AcTestContext _acTestContext;
        private readonly PersonEndpoints _endpoints = new ApiUriFactory().PersonEndpoints;

        public PersonsSteps(AcTestContext acTestContext)
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
    }
}
