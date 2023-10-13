using System.Text;
using AcceptanceTests.Common.Api.Helpers;
using BookingsApi.Contract.V1.Requests;
using BookingsApi.Contract.V1.Responses;
using Faker;
using TechTalk.SpecFlow;
using static Testing.Common.Builders.Api.ApiUriFactory.PersonEndpoints;

namespace BookingsApi.IntegrationTests.Steps
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
                            _username = seededHearing.GetParticipants()[0].Person.ContactEmail;
                        }
                        break;
                    }
                case Scenario.Nonexistent:
                    _username = $"{RandomNumber.Next()}@hmcts.net";
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
            var email = seededHearing.GetParticipants()[0].Person.ContactEmail;
            var searchTerm = RequestHelper.Serialise(
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
            var email = seededHearing.GetParticipants()[0].Person.ContactEmail;
            var searchTerm = RequestHelper.Serialise(
                new SearchTermRequest(email.Substring(0, 3).ToUpperInvariant())
                );
            Context.Uri = PostPersonBySearchTerm;
            Context.HttpMethod = HttpMethod.Post;
            Context.HttpContent = new StringContent(searchTerm, Encoding.UTF8, "application/json");
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
            SetupGetHearingsByUsernameForDeletionRequest("does.not.exist@hmcts.net");
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
            SetupAnonymisePersonRequest("does.not.exist@hmcts.net");
        }

        [Given(@"I have a non-existent update person details request")]
        public void GivenIHaveANonExistentUpdatePersonDetailsRequest()
        {
            Context.Uri = UpdatePersonDetails(Guid.NewGuid());
            Context.HttpMethod = HttpMethod.Put;
            var request = new UpdatePersonDetailsRequest
            {
                Username = "new.me@hmcts.net",
                FirstName = "New",
                LastName = "Me"
            };
            Context.HttpMethod = HttpMethod.Put;
            var jsonBody = RequestHelper.Serialise(request);
            Context.HttpContent = new StringContent(jsonBody, Encoding.UTF8, "application/json");
        }
        
        [Given(@"I have a malformed update person details request")]
        public void GivenIHaveAMalformedUpdatePersonDetailsRequest()
        {
            var hearing = Context.TestData.SeededHearing;
            var person = hearing.GetPersons()[0];
            var request = new UpdatePersonDetailsRequest
            {
                Username = String.Empty,
                FirstName = "New"
            };
            Context.Uri = UpdatePersonDetails(person.Id);
            Context.HttpMethod = HttpMethod.Put;
            var jsonBody = RequestHelper.Serialise(request);
            Context.HttpContent = new StringContent(jsonBody, Encoding.UTF8, "application/json");
        }
        
        [Given(@"I have a valid update person details request")]
        public void GivenIHaveAValidUpdatePersonDetailsRequest()
        {
            var hearing = Context.TestData.SeededHearing;
            var person = hearing.GetPersons()[0];
            var request = new UpdatePersonDetailsRequest
            {
                Username = "new.me@hmcts.net",
                FirstName = "New",
                LastName = "Me"
            };
            Context.Uri = UpdatePersonDetails(person.Id);
            Context.HttpMethod = HttpMethod.Put;
            var jsonBody = RequestHelper.Serialise(request);
            Context.HttpContent = new StringContent(jsonBody, Encoding.UTF8, "application/json");
        }

        [Given(@"I have a valid update username request containing a slash")]
        public void GivenIHaveAValidUpdateUsernameRequestContainingASlash()
        {
            var hearing = Context.TestData.SeededHearing;
            var person = hearing.GetPersons().First((p => p.ContactEmail.Contains(("/"))));
            var contactEmail = person.ContactEmail;
            var username = person.ContactEmail;
            Context.Uri = UpdatePersonUsername(contactEmail, username);
            Context.HttpMethod = HttpMethod.Put;
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
            var model = RequestHelper.Deserialise<PersonResponse>(json);
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
            var model = RequestHelper.Deserialise<List<PersonResponse>>(json);
            model[0].Should().NotBeNull();
            ValidatePersonData(model[0]);
        }

        [Then(@"a list of hearing usernames should be retrieved")]
        public async Task ThenAListOfHearingUsernamesShouldBeRetrieved()
        {
            var json = await Context.Response.Content.ReadAsStringAsync();
            var model = RequestHelper.Deserialise<UserWithClosedConferencesResponse>(json);
            model.Should().NotBeNull();
            model.Usernames.Should().NotBeNull();
            model.Usernames.Count.Should().Be(5); // 2 Individual & 2 Representative participants + 1 JOH participant
        }

        
        private async Task SetUserNameForGivenScenario(Scenario scenario)
        {
            switch (scenario)
            {
                case Scenario.Valid:
                    {
                        var seededHearing = await Context.TestDataManager.SeedVideoHearing();
                        Context.TestData.NewHearingId = seededHearing.Id;
                        NUnit.Framework.TestContext.WriteLine($"New seeded video hearing id: {seededHearing.Id}");
                        var participants = seededHearing.GetParticipants();
                       _username = participants[0].Person.Username;
                       
                        Context.TestData.Participant = participants.First(p => p.Person.Username.Equals(_username));
                        break;
                    }
                case Scenario.Nonexistent:
                    _username = $"{RandomNumber.Next()}@hmcts.net";
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
            var response = RequestHelper.Deserialise<List<HearingsByUsernameForDeletionResponse>>(json);

            response.Count.Should().Be(expectedNumOfHearings);
            
            if (expectedNumOfHearings == 1)
            {
                var result = response[0];
                var hearing = Context.TestData.SeededHearing;
                var leadCase = hearing.GetCases().FirstOrDefault(x => x.IsLeadCase) ?? hearing.GetCases()[0];
                result.HearingId.Should().Be(hearing.Id);
                result.Venue.Should().Be(hearing.HearingVenueName);
                result.ScheduledDateTime.Should().Be(hearing.ScheduledDateTime);
                result.CaseName.Should().Be(leadCase.Name);
                result.CaseNumber.Should().Be(leadCase.Number);
            }
        }
    }
}