using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Bookings.Api.Contract.Responses;
using Bookings.IntegrationTests.Contexts;
using Bookings.IntegrationTests.Helper;
using Faker;
using FluentAssertions;
using NUnit.Framework;
using TechTalk.SpecFlow;
using Testing.Common.Builders.Api;

namespace Bookings.IntegrationTests.Steps
{
    [Binding]
    public class PersonSteps : StepsBase
    {
        private readonly ApiTestContext _apiTestContext;
        private readonly PersonEndpoints _endpoints = new ApiUriFactory().PersonEndpoints;
        private string _username;
        
        public PersonSteps(ApiTestContext apiTestContext)
        {
            _apiTestContext = apiTestContext;
            _username = string.Empty;
        }
        
        [Given(@"I have a get person by username request with a (.*) username")]
        [Given(@"I have a get person by username request with an (.*) username")]
        public async Task GivenIHaveAGetPersonByUsernameRequest(Scenario scenario)
        {
            switch (scenario)
            {
                case Scenario.Valid:
                {
                    var seededHearing = await _apiTestContext.TestDataManager.SeedVideoHearing();
                    _apiTestContext.NewHearingId = seededHearing.Id;
                    TestContext.WriteLine($"New seeded video hearing id: {seededHearing.Id}");
                    _username = seededHearing.GetParticipants().First().Person.Username;
                    break;
                }
                case Scenario.Nonexistent:
                    _username = Internet.Email();
                    break;
                case Scenario.Invalid:
                    _username = "blfdshnon";
                    break;
                default: throw new ArgumentOutOfRangeException(nameof(scenario), scenario, null);
            }
            _apiTestContext.Uri = _endpoints.GetPersonByUsername(_username);
            _apiTestContext.HttpMethod = HttpMethod.Get;
        }
        
        [Given(@"I have a get person by username request with a (.*) contact email")]
        [Given(@"I have a get person by username request with an (.*) contact email")]
        public async Task GivenIHaveAGetPersonByContactEmailRequest(Scenario scenario)
        {
            switch (scenario)
            {
                case Scenario.Valid:
                {
                    var seededHearing = await _apiTestContext.TestDataManager.SeedVideoHearing();
                    _apiTestContext.NewHearingId = seededHearing.Id;
                    TestContext.WriteLine($"New seeded video hearing id: {seededHearing.Id}");
                    _username = seededHearing.GetParticipants().First().Person.ContactEmail;
                    break;
                }
                case Scenario.Nonexistent:
                    _username = Internet.Email();
                    break;
                case Scenario.Invalid:
                    _username = "blfdshnon";
                    break;
                default: throw new ArgumentOutOfRangeException(nameof(scenario), scenario, null);
            }
            _apiTestContext.Uri = _endpoints.GetPersonByContactEmail(_username);
            _apiTestContext.HttpMethod = HttpMethod.Get;
        }
        
        [Then(@"person details should be retrieved")]
        public async Task ThenThePersonDetailsShouldBeRetrieved()
        {
            var json = await _apiTestContext.ResponseMessage.Content.ReadAsStringAsync();
            var model = ApiRequestHelper.DeserialiseSnakeCaseJsonToResponse<PersonResponse>(json);

            model.Should().NotBeNull();
        }

    }
}