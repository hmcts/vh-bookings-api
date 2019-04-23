using System;
using System.Collections.Generic;
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
    public class Personsteps : StepsBase
    {
        private readonly Contexts.TestContext _apiTestContext;
        private readonly PersonEndpoints _endpoints = new ApiUriFactory().PersonEndpoints;
        private string _username;

        public Personsteps(Contexts.TestContext apiTestContext)
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
                        NUnit.Framework.TestContext.WriteLine($"New seeded video hearing id: {seededHearing.Id}");
                        _username = seededHearing.GetParticipants().First().Person.Username;
                        break;
                    }
                case Scenario.Nonexistent:
                    _username = Internet.Email();
                    break;
                case Scenario.Invalid:
                    _username = "invalid username";
                    break;
                default: throw new ArgumentOutOfRangeException(nameof(scenario), scenario, null);
            }
            _apiTestContext.Uri = _endpoints.GetPersonByUsername(_username);
            _apiTestContext.HttpMethod = HttpMethod.Get;
        }

        [Given(@"I have a get (.*) by contact email request with a (.*) contact email")]
        [Given(@"I have a get (.*) by contact email request with an (.*) contact email")]
        public async Task GivenIHaveAGetPersonByContactEmailRequest(string personType, Scenario scenario)
        {
            switch (scenario)
            {
                case Scenario.Valid:
                    {
                        var seededHearing = await _apiTestContext.TestDataManager.SeedVideoHearing();
                        _apiTestContext.NewHearingId = seededHearing.Id;
                        NUnit.Framework.TestContext.WriteLine($"New seeded video hearing id: {seededHearing.Id}");
                        if(personType.Equals("individual"))
                        {
                            _username = seededHearing.GetParticipants().First(p => p.HearingRole.UserRole.IsIndividual).Person.ContactEmail;
                        }
                        else
                        {
                            _username = seededHearing.GetParticipants().First().Person.ContactEmail;
                        }
                        break;
                    }
                case Scenario.Nonexistent:
                    _username = Internet.Email();
                    break;
                case Scenario.Invalid:
                    _username = "invalid contact email";
                    break;
                default: throw new ArgumentOutOfRangeException(nameof(scenario), scenario, null);
            }
            _apiTestContext.Uri = _endpoints.GetPersonByContactEmail(_username);
            _apiTestContext.HttpMethod = HttpMethod.Get;
        }


        [Given(@"I have a get person by contact email search term request")]
        public async Task GivenIHaveAGetPersonByContactEmailSearchTermRequest()
        {
            var seededHearing = await _apiTestContext.TestDataManager.SeedVideoHearing();
            _apiTestContext.NewHearingId = seededHearing.Id;
            NUnit.Framework.TestContext.WriteLine($"New seeded video hearing id: {seededHearing.Id}");
            var email = seededHearing.GetParticipants().First().Person.ContactEmail;
            var searchTerm = email.Substring(0, 3);
            _apiTestContext.Uri = _endpoints.GetPersonBySearchTerm(searchTerm);
            _apiTestContext.HttpMethod = HttpMethod.Get;
        }

        [Given(@"I have a get person by contact email search term request that case insensitive")]
        public async Task GivenIHaveAGetPersonByContactEmailSearchTermRequestThatCaseInsensitive()
        {
            var seededHearing = await _apiTestContext.TestDataManager.SeedVideoHearing();
            _apiTestContext.NewHearingId = seededHearing.Id;
            NUnit.Framework.TestContext.WriteLine($"New seeded video hearing id: {seededHearing.Id}");
            var email = seededHearing.GetParticipants().First().Person.ContactEmail;
            var searchTerm = email.Substring(0, 3).ToUpperInvariant();
            _apiTestContext.Uri = _endpoints.GetPersonBySearchTerm(searchTerm);
            _apiTestContext.HttpMethod = HttpMethod.Get;
        }

        [Then(@"person details should be retrieved")]
        public async Task ThenThePersonDetailsShouldBeRetrieved()
        {
            var json = await _apiTestContext.ResponseMessage.Content.ReadAsStringAsync();
            var model = ApiRequestHelper.DeserialiseSnakeCaseJsonToResponse<PersonResponse>(json);
            model.Should().NotBeNull();
            ValidatePersonData(model);
        }

        private static void ValidatePersonData(PersonResponse model)
        {
            model.Id.Should().NotBeEmpty();
            model.ContactEmail.Should().NotBeNullOrEmpty();
            model.FirstName.Should().NotBeNullOrEmpty();
            model.LastName.Should().NotBeNullOrEmpty();
            model.MiddleNames.Should().NotBeNullOrEmpty();
            model.TelephoneNumber.Should().NotBeNullOrEmpty();
            model.Title.Should().NotBeNullOrEmpty();
            model.Username.Should().NotBeNullOrEmpty();
        }

        [Then(@"person address should be retrieved")]
        public async Task ThenPersonAddressShouldBeRetrieved()
        {
            var json = await _apiTestContext.ResponseMessage.Content.ReadAsStringAsync();
            var model = ApiRequestHelper.DeserialiseSnakeCaseJsonToResponse<PersonResponse>(json);
            model.Should().NotBeNull();
            model.Organisation.Should().NotBeNullOrEmpty();
            model.Id.Should().NotBeEmpty();
            model.HouseNumber.Should().NotBeNullOrEmpty();
            model.Street.Should().NotBeNullOrEmpty();
            model.City.Should().NotBeNullOrEmpty();
            model.County.Should().NotBeNullOrEmpty();
            model.Postcode.Should().NotBeNullOrEmpty();
        }


        [Then(@"persons details should be retrieved")]
        public async Task ThenPersonsDetailsShouldBeRetrieved()
        {
            var json = await _apiTestContext.ResponseMessage.Content.ReadAsStringAsync();
            var model = ApiRequestHelper.DeserialiseSnakeCaseJsonToResponse<List<PersonResponse>>(json);
            model[0].Should().NotBeNull();
            ValidatePersonData(model[0]);
        }
    }
}