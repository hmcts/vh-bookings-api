using Bookings.Api.Contract.Requests;
using Bookings.Api.Contract.Responses;
using Bookings.IntegrationTests.Helper;
using Faker;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using AcceptanceTests.Common.Api.Helpers;
using TechTalk.SpecFlow;
using static Testing.Common.Builders.Api.ApiUriFactory.PersonEndpoints;

namespace Bookings.IntegrationTests.Steps
{
    [Binding]
    public class PersonBaseSteps : BaseSteps
    {
        private string _username;

        public PersonBaseSteps(Contexts.TestContext apiTestContext) : base(apiTestContext)
        {
            _username = string.Empty;
        }

        [Given(@"I have a get person by username request with a (.*) username")]
        [Given(@"I have a get person by username request with an (.*) username")]
        public async Task GivenIHaveAGetPersonByUsernameRequest(Scenario scenario)
        {
            await SetUserNameForGivenScenario(scenario);
            Context.Uri = GetPersonByUsername(_username);
            Context.HttpMethod = HttpMethod.Get;
        }

        [Given(@"I have a get (.*) by contact email request with a (.*) contact email")]
        [Given(@"I have a get (.*) by contact email request with an (.*) contact email")]
        public async Task GivenIHaveAGetPersonByContactEmailRequest(string personType, Scenario scenario)
        {
            switch (scenario)
            {
                case Scenario.Valid:
                    {
                        var seededHearing = await Context.TestDataManager.SeedVideoHearing();
                        Context.TestData.NewHearingId = seededHearing.Id;
                        NUnit.Framework.TestContext.WriteLine($"New seeded video hearing id: {seededHearing.Id}");
                        if (personType.Equals("individual"))
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
            Context.Uri = GetPersonByContactEmail(_username);
            Context.HttpMethod = HttpMethod.Get;
        }


        [Given(@"I have a get person by contact email search term request")]
        public async Task GivenIHaveAGetPersonByContactEmailSearchTermRequest()
        {
            var seededHearing = await Context.TestDataManager.SeedVideoHearing();
            Context.TestData.NewHearingId = seededHearing.Id;
            NUnit.Framework.TestContext.WriteLine($"New seeded video hearing id: {seededHearing.Id}");
            var email = seededHearing.GetParticipants().First().Person.ContactEmail;
            var searchTerm = RequestHelper.SerialiseRequestToSnakeCaseJson(
                new SearchTermRequest(email.Substring(0, 3))
                );

            Context.Uri = PostPersonBySearchTerm;
            Context.HttpMethod = HttpMethod.Post;
            Context.HttpContent = new StringContent(searchTerm, Encoding.UTF8, "application/json");

        }

        [Given(@"I have a get person by contact email search term request that case insensitive")]
        public async Task GivenIHaveAGetPersonByContactEmailSearchTermRequestThatCaseInsensitive()
        {
            var seededHearing = await Context.TestDataManager.SeedVideoHearing();
            Context.TestData.NewHearingId = seededHearing.Id;
            NUnit.Framework.TestContext.WriteLine($"New seeded video hearing id: {seededHearing.Id}");
            var email = seededHearing.GetParticipants().First().Person.ContactEmail;
            var searchTerm = RequestHelper.SerialiseRequestToSnakeCaseJson(
                new SearchTermRequest(email.Substring(0, 3).ToUpperInvariant())
                );
            Context.Uri = PostPersonBySearchTerm;
            Context.HttpMethod = HttpMethod.Post;
            Context.HttpContent = new StringContent(searchTerm, Encoding.UTF8, "application/json");
        }

        [Given(@"I have a get person suitability answers by username request with an (.*) username")]
        public async Task GivenIHaveAGetPersonSuitabilityAnswersByUsernameRequest(Scenario scenario)
        {
            await SetUserNameForGivenScenario(scenario, true, true);
            Context.Uri = GetPersonSuitabilityAnswers(_username);
            Context.HttpMethod = HttpMethod.Get;
        }

        [Given(@"I have a get person without suitability answers by username request with an (.*) username")]
        public async Task GivenIHaveAGetPersonWithoutSuitabilityAnswersByUsernameRequest(Scenario scenario)
        {
            await SetUserNameForGivenScenario(scenario);
            Context.Uri = GetPersonSuitabilityAnswers(_username);
            Context.HttpMethod = HttpMethod.Get;
        }

        [Given(@"I have a request to get the usernames for old hearings")]
        public void GivenIHaveARequestToGetTheUsernamesForOldHearings()
        {
            Context.Uri = GetPersonByClosedHearings();
            Context.HttpMethod = HttpMethod.Get;
        }

        [Given(@"I have a search for hearings using a judge username request")]
        public void GivenIHaveASearchForHearingsUsingAJudgeUsernameRequest()
        {
            var judge = Context.TestData.SeededHearing.GetParticipants().First(x => x.HearingRole.UserRole.IsJudge);
            SetupGetHearingsByUsernameForDeletionRequest(judge.Person.Username);
        }
        
        [Given(@"I have a search for hearings using a non-judge username request")]
        public void GivenIHaveASearchForHearingsUsingANonJudgeUsernameRequest()
        {
            var nonJudge = Context.TestData.SeededHearing.GetParticipants().First(x => !x.HearingRole.UserRole.IsJudge);
            SetupGetHearingsByUsernameForDeletionRequest(nonJudge.Person.Username);
        }

        [Given(@"I have a search for hearings using non-existent username request")]
        public void GivenIHaveASearchForHearingsUsingANonExistentUsernameRequest()
        {
            SetupGetHearingsByUsernameForDeletionRequest("does.not.exist@test.net");
        }
        
        [Given(@"I have a valid anonymise person request")]
        public void GivenIHaveAValidAnonymisePersonRequest()
        {
            var participant = Context.TestData.SeededHearing.GetParticipants().First(x => !x.HearingRole.UserRole.IsJudge);
            SetupAnonymisePersonRequest(participant.Person.Username);
        }
        
        [Given(@"I have a non-existent anonymise person request")]
        public void GivenIHaveANonExistentAnonymisePersonRequest()
        {
            SetupAnonymisePersonRequest("does.not.exist@test.net");
        }
        
        private void SetupGetHearingsByUsernameForDeletionRequest(string username)
        {
            Context.Uri = GetHearingsByUsernameForDeletion(username);
            Context.HttpMethod = HttpMethod.Get;
        }

        private void SetupAnonymisePersonRequest(string username)
        {
            Context.Uri = AnonymisePerson(username);
            Context.HttpMethod = HttpMethod.Patch;
        }
        
        [Then(@"person details should be retrieved")]
        public async Task ThenThePersonDetailsShouldBeRetrieved()
        {
            var json = await Context.Response.Content.ReadAsStringAsync();
            var model = RequestHelper.DeserialiseSnakeCaseJsonToResponse<PersonResponse>(json);
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

        
        [Then(@"persons details should be retrieved")]
        public async Task ThenPersonsDetailsShouldBeRetrieved()
        {
            var json = await Context.Response.Content.ReadAsStringAsync();
            var model = RequestHelper.DeserialiseSnakeCaseJsonToResponse<List<PersonResponse>>(json);
            model[0].Should().NotBeNull();
            ValidatePersonData(model[0]);
        }

        [Then(@"a list of hearing usernames should be retrieved")]
        public async Task ThenAListOfHearingUsernamesShouldBeRetrieved()
        {
            var json = await Context.Response.Content.ReadAsStringAsync();
            var model = RequestHelper.DeserialiseSnakeCaseJsonToResponse<UserWithClosedConferencesResponse>(json);
            model.Should().NotBeNull();
            model.Usernames.Should().NotBeNull();
            model.Usernames.Count.Should().Be(3); // Individual & Representative participants
        }


        [Then(@"suitability answers retrieved should '(.*)'")]
        public async Task ThenPersonsSuitabilityAnswersShouldBeRetrieved(string scenario)
        {
            var json = await Context.Response.Content.ReadAsStringAsync();
            var model = RequestHelper.DeserialiseSnakeCaseJsonToResponse<List<PersonSuitabilityAnswerResponse>>(json);

            model[0].Should().NotBeNull();
            model[0].HearingId.Should().NotBeEmpty();
            model[0].HearingId.Should().Be(Context.TestData.NewHearingId);
            model[0].ParticipantId.Should().NotBeEmpty();
            model[0].ParticipantId.Should().Be(Context.TestData.Participant.Id);
            model[0].ScheduledAt.Should().BeAfter(DateTime.MinValue);

            if(scenario == "be empty")
            {
                model[0].Answers.Count.Should().Be(0);
                model[0].UpdatedAt.Should().Be(DateTime.MinValue);
            }
            else
            {
                model[0].Answers.Should().NotBeNull();
                model[0].Answers.Count.Should().Be(2);
                model[0].UpdatedAt.Should().BeAfter(DateTime.MinValue);
            }
            
        }

        private async Task SetUserNameForGivenScenario(Scenario scenario, bool hasSuitability = false, bool addSuitabilityAnswer = false)
        {
            switch (scenario)
            {
                case Scenario.Valid:
                    {
                        var seededHearing = await Context.TestDataManager.SeedVideoHearing(addSuitabilityAnswer);
                        Context.TestData.NewHearingId = seededHearing.Id;
                        NUnit.Framework.TestContext.WriteLine($"New seeded video hearing id: {seededHearing.Id}");
                        var participants = seededHearing.GetParticipants();
                        if(hasSuitability)
                        {
                            _username = participants.First(p => p.Questionnaire != null && p.Questionnaire.SuitabilityAnswers.Any()).Person.Username;
                        }
                        else
                        {
                            _username = participants.First().Person.Username;
                        }
                        Context.TestData.Participant = participants.First(p => p.Person.Username.Equals(_username));
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

        }
        
        [Then(@"a list of hearings for deletion is (.*)")]
        public async Task ThenAListOfHearingsForDeletionIs(int expectedNumOfHearings)
        {
            var json = await Context.Response.Content.ReadAsStringAsync();
            var response = RequestHelper.DeserialiseSnakeCaseJsonToResponse<List<HearingsByUsernameForDeletionResponse>>(json);

            response.Count.Should().Be(expectedNumOfHearings);
            
            if (expectedNumOfHearings == 1)
            {
                var result = response[0];
                var hearing = Context.TestData.SeededHearing;
                var leadCase = hearing.GetCases().FirstOrDefault(x => x.IsLeadCase) ?? hearing.GetCases().First();
                result.HearingId.Should().Be(hearing.Id);
                result.Venue.Should().Be(hearing.HearingVenueName);
                result.ScheduledDateTime.Should().Be(hearing.ScheduledDateTime);
                result.CaseName.Should().Be(leadCase.Name);
                result.CaseNumber.Should().Be(leadCase.Number);
            }
        }
    }
}