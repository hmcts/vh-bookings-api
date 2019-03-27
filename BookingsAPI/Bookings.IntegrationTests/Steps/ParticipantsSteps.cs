using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Bookings.Api.Contract.Requests;
using Bookings.Api.Contract.Responses;
using Bookings.DAL;
using Bookings.Domain;
using Bookings.IntegrationTests.Contexts;
using Bookings.IntegrationTests.Helper;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using TechTalk.SpecFlow;
using Testing.Common.Builders.Api;
using Testing.Common.Builders.Api.Request;

namespace Bookings.IntegrationTests.Steps
{
    [Binding]
    public sealed class ParticipantsSteps : StepsBase
    {
        private readonly Contexts.TestContext _apiTestContext;
        private readonly ParticipantsEndpoints _endpoints = new ApiUriFactory().ParticipantsEndpoints;

        public ParticipantsSteps(Contexts.TestContext apiTestContext)
        {
            _apiTestContext = apiTestContext;
        }

        [Given(@"I have a get participants in a hearing request with a (.*) hearing id")]
        [Given(@"I have a get participants in a hearing request with an (.*) hearing id")]
        public async Task GivenIHaveAGetParticipantsInAHearingRequestWithAValidHearingId(Scenario scenario)
        {
            Guid hearingId;
            switch (scenario)
            {
                case Scenario.Valid:
                {
                    var seededHearing = await _apiTestContext.TestDataManager.SeedVideoHearing();
                    _apiTestContext.NewHearingId = seededHearing.Id;
                    hearingId = _apiTestContext.NewHearingId;
                        NUnit.Framework.TestContext.WriteLine($"New seeded video hearing id: {seededHearing.Id}");
                    break;
                }
                case Scenario.Nonexistent:
                    hearingId = Guid.NewGuid();
                    break;
                case Scenario.Invalid:
                    hearingId = Guid.Empty;
                    break;
                default: throw new ArgumentOutOfRangeException(nameof(scenario), scenario, null);
            }
            _apiTestContext.Uri = _endpoints.GetAllParticipantsInHearing(hearingId);
            _apiTestContext.HttpMethod = HttpMethod.Get;
        }

        [Given(@"I have an add participant to a hearing request with a (.*) hearing id")]
        [Given(@"I have an add participant to a hearing request with an (.*) hearing id")]
        public async Task GivenIHaveAnAddParticipantToAHearingRequestWithAHearingId(Scenario scenario)
        {
            var request = BuildRequest();
            _apiTestContext.Participants = request.Participants;
            var jsonBody = ApiRequestHelper.SerialiseRequestToSnakeCaseJson(request);
            _apiTestContext.HttpContent = new StringContent(jsonBody, Encoding.UTF8, "application/json");
            Guid hearingId;
            switch (scenario)
            {
                case Scenario.Valid:
                {
                    var seededHearing = await _apiTestContext.TestDataManager.SeedVideoHearing();
                    _apiTestContext.NewHearingId = seededHearing.Id;
                    hearingId = _apiTestContext.NewHearingId;
                        NUnit.Framework.TestContext.WriteLine($"New seeded video hearing id: {seededHearing.Id}");
                    break;
                }
                case Scenario.Nonexistent:
                    hearingId = Guid.NewGuid();
                    break;
                case Scenario.Invalid:
                    hearingId = Guid.Empty;
                    break;
                default: throw new ArgumentOutOfRangeException(nameof(scenario), scenario, null);
            }
            _apiTestContext.Uri = _endpoints.AddParticipantsToHearing(hearingId);
            _apiTestContext.HttpMethod = HttpMethod.Put;
        }

        [When(@"I send the same request but with a new hearing id")]
        public async Task WhenISendTheSameRequestButWithANewHearingId()
        {
            var seededHearing = await _apiTestContext.TestDataManager.SeedVideoHearing();
            _apiTestContext.OldHearingId = _apiTestContext.NewHearingId;
            _apiTestContext.NewHearingId = seededHearing.Id;
            NUnit.Framework.TestContext.WriteLine($"New seeded video hearing id: {seededHearing.Id}");
            seededHearing.GetParticipants().Count.Should().Be(4);
            _apiTestContext.Uri = _endpoints.AddParticipantsToHearing(_apiTestContext.NewHearingId);
            _apiTestContext.ResponseMessage = await SendPutRequestAsync(_apiTestContext);
        }

        [Given(@"I have an add participants in a hearing request with an invalid participant")]
        public async Task GivenIHaveAnAddParticipantToAHearingRequestWithAParticipantId()
        {
            var request = new InvalidRequest();
            var jsonBody = ApiRequestHelper.SerialiseRequestToSnakeCaseJson(request.BuildRequest());
            _apiTestContext.HttpContent = new StringContent(jsonBody, Encoding.UTF8, "application/json");
            var seededHearing = await _apiTestContext.TestDataManager.SeedVideoHearing();
            _apiTestContext.NewHearingId = seededHearing.Id;
            NUnit.Framework.TestContext.WriteLine($"New seeded video hearing id: {seededHearing.Id}");
            _apiTestContext.Uri = _endpoints.AddParticipantsToHearing(_apiTestContext.NewHearingId);
            _apiTestContext.HttpMethod = HttpMethod.Put;
        }

        [Given(@"I have a get a single participant in a hearing request with a (.*) hearing id")]
        [Given(@"I have a get a single participant in a hearing request with an (.*) hearing id")]
        public async Task GivenIHaveAGetASingleParticipantInAHearingRequestWithAValidHearingId(Scenario scenario)
        {
            Guid hearingId;
            var seededHearing = await _apiTestContext.TestDataManager.SeedVideoHearing();
            NUnit.Framework.TestContext.WriteLine($"New seeded video hearing id: {seededHearing.Id}");
            _apiTestContext.NewHearingId = seededHearing.Id;
            switch (scenario)
            {
                case Scenario.Valid:
                {
                    hearingId = _apiTestContext.NewHearingId;
                    break;
                }
                case Scenario.Nonexistent:
                    hearingId = Guid.NewGuid();
                    break;
                case Scenario.Invalid:
                    hearingId = Guid.Empty;
                    break;
                default: throw new ArgumentOutOfRangeException(nameof(scenario), scenario, null);
            }
            _apiTestContext.Uri = _endpoints.GetParticipantInHearing(hearingId, seededHearing.GetParticipants()[0].Id);
            _apiTestContext.HttpMethod = HttpMethod.Get;
        }

        [Given(@"I have a get a single participant in a hearing request with a (.*) participant id")]
        [Given(@"I have a get a single participant in a hearing request with an (.*) participant id")]
        public async Task GivenIHaveAGetASingleParticipantInAHearingRequestWithAParticipantId(Scenario scenario)
        {
            var seededHearing = await _apiTestContext.TestDataManager.SeedVideoHearing();
            NUnit.Framework.TestContext.WriteLine($"New seeded video hearing id: {seededHearing.Id}");
            _apiTestContext.NewHearingId = seededHearing.Id;
            Guid participantId;
            switch (scenario)
            {
                case Scenario.Nonexistent:
                    participantId = Guid.NewGuid();
                    break;
                case Scenario.Invalid:
                    participantId = Guid.Empty;
                    break;
                default: throw new ArgumentOutOfRangeException(nameof(scenario), scenario, null);
            }
            _apiTestContext.Uri = _endpoints.GetParticipantInHearing(_apiTestContext.NewHearingId, participantId);
            _apiTestContext.HttpMethod = HttpMethod.Get;
        }

        [Given(@"I have a remove participant from a hearing request with a (.*) hearing id")]
        [Given(@"I have a remove participant from a hearing request with an (.*) hearing id")]
        public async Task GivenIHaveARemoveParticipantFromAHearingWithAValidHearingId(Scenario scenario)
        {
            var seededHearing = await _apiTestContext.TestDataManager.SeedVideoHearing();
            NUnit.Framework.TestContext.WriteLine($"New seeded video hearing id: {seededHearing.Id}");
            _apiTestContext.NewHearingId = seededHearing.Id;
            _apiTestContext.Participant = seededHearing.GetParticipants().First();
            _apiTestContext.HttpMethod = HttpMethod.Delete;

            var participantId = seededHearing.GetParticipants().First().Id;
            Guid hearingId;

            switch (scenario)
            {
                case Scenario.Valid:
                    {
                        hearingId = _apiTestContext.NewHearingId;
                        break;
                    }
                case Scenario.Nonexistent:
                    {
                        hearingId = Guid.NewGuid();
                        break;
                    }
                case Scenario.Invalid:
                    {
                        hearingId = Guid.Empty;
                        break;
                    }
                default: throw new ArgumentOutOfRangeException(nameof(scenario), scenario, null);
            }
            _apiTestContext.Uri = _endpoints.RemoveParticipantFromHearing(hearingId, participantId);
        }

        [Given(@"I have a remove participant from a hearing request with a (.*) participant id")]
        [Given(@"I have a remove participant from a hearing request with an (.*) participant id")]
        public async Task GivenIHaveARemoveParticipantFromAHearingWithAParticipantId(Scenario scenario)
        {
            var seededHearing = await _apiTestContext.TestDataManager.SeedVideoHearing();
            NUnit.Framework.TestContext.WriteLine($"New seeded video hearing id: {seededHearing.Id}");
            _apiTestContext.NewHearingId = seededHearing.Id;
            _apiTestContext.HttpMethod = HttpMethod.Delete;

            Guid participantId;
            switch (scenario)
            {
                case Scenario.Nonexistent:
                {
                    participantId = Guid.NewGuid();
                    break;
                }
                case Scenario.Invalid:
                {
                    participantId = Guid.Empty;
                    break;
                }
                default: throw new ArgumentOutOfRangeException(nameof(scenario), scenario, null);
            }
            _apiTestContext.Uri = _endpoints.RemoveParticipantFromHearing(_apiTestContext.NewHearingId, participantId);
        }

        [Then(@"a list of hearing participants should be retrieved")]
        public async Task ThenAListOfHearingParticipantsShouldBeRetrieved()
        {
            var json = await _apiTestContext.ResponseMessage.Content.ReadAsStringAsync();
            var model = ApiRequestHelper.DeserialiseSnakeCaseJsonToResponse<List<ParticipantResponse>>(json);
            CheckParticipantData(model);
        }

        private static void CheckParticipantData(IReadOnlyCollection<ParticipantResponse> model)
        {
            model.Should().NotBeNull();
            foreach (var participant in model)
            {
                participant.ContactEmail.Should().NotBeNullOrEmpty();
                participant.FirstName.Should().NotBeNullOrEmpty();
                participant.Id.Should().NotBeEmpty();
                participant.LastName.Should().NotBeNullOrEmpty();
                participant.MiddleNames.Should().NotBeNullOrEmpty();
                participant.TelephoneNumber.Should().NotBeNullOrEmpty();
                participant.Title.Should().NotBeNullOrEmpty();
                participant.UserRoleName.Should().NotBeNullOrEmpty();
                participant.Username.Should().NotBeNullOrEmpty();
                participant.CaseRoleName.Should().NotBeNullOrEmpty();
                participant.HearingRoleName.Should().NotBeNullOrEmpty();
            }
        }

        [Then(@"a hearing participant should be retrieved")]
        public async Task ThenAHearingParticipantShouldBeRetrieved()
        {
            var json = await _apiTestContext.ResponseMessage.Content.ReadAsStringAsync();
            var model = new List<ParticipantResponse> { ApiRequestHelper.DeserialiseSnakeCaseJsonToResponse<ParticipantResponse>(json) };
            CheckParticipantData(model);
        }

        [Then(@"the participant should be (.*)")]
        public void ThenTheParticipantShouldBeAddedOrRemoved(string state)
        {
            Hearing hearingFromDb;
            using (var db = new BookingsDbContext(_apiTestContext.BookingsDbContextOptions))
            {
                hearingFromDb = db.VideoHearings
                    .Include("Participants.Person").AsNoTracking()
                    .Single(x => x.Id == _apiTestContext.NewHearingId);
            }
            if (state.Equals("added"))
            {
                hearingFromDb.GetParticipants().Count.Should().BeGreaterThan(3);
                foreach (var participantRequest in _apiTestContext.Participants)
                {
                    hearingFromDb.GetParticipants().Any(x => x.Person.Username == participantRequest.Username).Should()
                        .BeTrue();
                }
            }
            if (state.Equals("removed"))
            {               
                hearingFromDb.GetParticipants().Any(x => x.Id == _apiTestContext.Participant.Id).Should().BeFalse();
            }
        }

        private static AddParticipantsToHearingRequest BuildRequest()
        {
            var newParticipant = new ParticipantRequestBuilder("Defendant", "Defendant LIP").Build();

            return new AddParticipantsToHearingRequest
            {
                Participants = new List<ParticipantRequest> { newParticipant }
            };
        }

        [TearDown]
        public async Task TearDown()
        {
            if (_apiTestContext.NewHearingId != Guid.Empty)
            {
                NUnit.Framework.TestContext.WriteLine($"Removing test hearing {_apiTestContext.NewHearingId}");
                await Hooks.RemoveVideoHearing(_apiTestContext.NewHearingId);
            }
            if (_apiTestContext.OldHearingId != Guid.Empty)
            {
                NUnit.Framework.TestContext.WriteLine($"Removing test hearing {_apiTestContext.OldHearingId}");
                await Hooks.RemoveVideoHearing(_apiTestContext.OldHearingId);
            }
        }
    }
}
