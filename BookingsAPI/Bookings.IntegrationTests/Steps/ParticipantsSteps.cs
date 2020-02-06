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
using Bookings.IntegrationTests.Helper;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using TechTalk.SpecFlow;
using Testing.Common.Builders.Api;
using Testing.Common.Builders.Api.Request;
using TestContext = Bookings.IntegrationTests.Contexts.TestContext;

namespace Bookings.IntegrationTests.Steps
{
    [Binding]
    public sealed class ParticipantsSteps : StepsBase
    {
        private readonly ParticipantsEndpoints _endpoints = new ApiUriFactory().ParticipantsEndpoints;

        public ParticipantsSteps(TestContext apiTestContext) : base(apiTestContext)
        {
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
                    var seededHearing = await Context.TestDataManager.SeedVideoHearing();
                    Context.NewHearingId = seededHearing.Id;
                    hearingId = Context.NewHearingId;
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
            Context.Uri = _endpoints.GetAllParticipantsInHearing(hearingId);
            Context.HttpMethod = HttpMethod.Get;
        }

        [Given(@"I have an add participant to a hearing request with a (.*) hearing id")]
        [Given(@"I have an add participant to a hearing request with an (.*) hearing id")]
        public async Task GivenIHaveAnAddParticipantToAHearingRequestWithAHearingId(Scenario scenario)
        {
            var request = BuildRequest();
            Context.Participants = request.Participants;
            var jsonBody = ApiRequestHelper.SerialiseRequestToSnakeCaseJson(request);
            Context.HttpContent = new StringContent(jsonBody, Encoding.UTF8, "application/json");
            Guid hearingId;
            switch (scenario)
            {
                case Scenario.Valid:
                {
                    var seededHearing = await Context.TestDataManager.SeedVideoHearing();
                    Context.NewHearingId = seededHearing.Id;
                    hearingId = Context.NewHearingId;
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
            Context.Uri = _endpoints.AddParticipantsToHearing(hearingId);
            Context.HttpMethod = HttpMethod.Post;
        }

        [When(@"I send the same request but with a new hearing id")]
        public async Task WhenISendTheSameRequestButWithANewHearingId()
        {
            var seededHearing = await Context.TestDataManager.SeedVideoHearing();
            Context.OldHearingId = Context.NewHearingId;
            Context.NewHearingId = seededHearing.Id;
            NUnit.Framework.TestContext.WriteLine($"New seeded video hearing id: {seededHearing.Id}");
            seededHearing.GetParticipants().Count.Should().Be(4);
            Context.Uri = _endpoints.AddParticipantsToHearing(Context.NewHearingId);
            Context.ResponseMessage = await SendPostRequestAsync(Context);
        }

        [Given(@"I have an add participants in a hearing request with an invalid participant")]
        public async Task GivenIHaveAnAddParticipantToAHearingRequestWithAParticipantId()
        {
            var request = new InvalidRequest();
            var jsonBody = ApiRequestHelper.SerialiseRequestToSnakeCaseJson(request.BuildRequest());
            Context.HttpContent = new StringContent(jsonBody, Encoding.UTF8, "application/json");
            var seededHearing = await Context.TestDataManager.SeedVideoHearing();
            Context.NewHearingId = seededHearing.Id;
            NUnit.Framework.TestContext.WriteLine($"New seeded video hearing id: {seededHearing.Id}");
            Context.Uri = _endpoints.AddParticipantsToHearing(Context.NewHearingId);
            Context.HttpMethod = HttpMethod.Post;
        }

        [Given(@"I have a get a single participant in a hearing request with a (.*) hearing id")]
        [Given(@"I have a get a single participant in a hearing request with an (.*) hearing id")]
        public async Task GivenIHaveAGetASingleParticipantInAHearingRequestWithAValidHearingId(Scenario scenario)
        {
            Guid hearingId;
            var seededHearing = await Context.TestDataManager.SeedVideoHearing();
            NUnit.Framework.TestContext.WriteLine($"New seeded video hearing id: {seededHearing.Id}");
            Context.NewHearingId = seededHearing.Id;
            switch (scenario)
            {
                case Scenario.Valid:
                {
                    hearingId = Context.NewHearingId;
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
            Context.Uri = _endpoints.GetParticipantInHearing(hearingId, seededHearing.GetParticipants()[0].Id);
            Context.HttpMethod = HttpMethod.Get;
        }

        [Given(@"I have a get a single participant in a hearing request with a (.*) participant id")]
        [Given(@"I have a get a single participant in a hearing request with an (.*) participant id")]
        public async Task GivenIHaveAGetASingleParticipantInAHearingRequestWithAParticipantId(Scenario scenario)
        {
            var seededHearing = await Context.TestDataManager.SeedVideoHearing();
            NUnit.Framework.TestContext.WriteLine($"New seeded video hearing id: {seededHearing.Id}");
            Context.NewHearingId = seededHearing.Id;
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
            Context.Uri = _endpoints.GetParticipantInHearing(Context.NewHearingId, participantId);
            Context.HttpMethod = HttpMethod.Get;
        }

        [Given(@"I have a remove participant from a hearing request with a (.*) hearing id")]
        [Given(@"I have a remove participant from a hearing request with an (.*) hearing id")]
        public async Task GivenIHaveARemoveParticipantFromAHearingWithAValidHearingId(Scenario scenario)
        {
            var seededHearing = await Context.TestDataManager.SeedVideoHearing();
            NUnit.Framework.TestContext.WriteLine($"New seeded video hearing id: {seededHearing.Id}");
            Context.NewHearingId = seededHearing.Id;
            Context.Participant = seededHearing.GetParticipants().First();
            Context.HttpMethod = HttpMethod.Delete;

            var participantToRemove = seededHearing.GetParticipants().First();
            var participantId = participantToRemove.Id;
            Context.RemovedPersons = new List<string>{participantToRemove.Person.ContactEmail};
            Guid hearingId;

            switch (scenario)
            {
                case Scenario.Valid:
                    {
                        hearingId = Context.NewHearingId;
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
            Context.Uri = _endpoints.RemoveParticipantFromHearing(hearingId, participantId);
        }

        [Given(@"I have a remove participant from a hearing request with a (.*) participant id")]
        [Given(@"I have a remove participant from a hearing request with an (.*) participant id")]
        public async Task GivenIHaveARemoveParticipantFromAHearingWithAParticipantId(Scenario scenario)
        {
            var seededHearing = await Context.TestDataManager.SeedVideoHearing();
            NUnit.Framework.TestContext.WriteLine($"New seeded video hearing id: {seededHearing.Id}");
            Context.NewHearingId = seededHearing.Id;
            Context.HttpMethod = HttpMethod.Delete;

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
            Context.Uri = _endpoints.RemoveParticipantFromHearing(Context.NewHearingId, participantId);
        }

        [Then(@"a list of hearing participants should be retrieved")]
        public async Task ThenAListOfHearingParticipantsShouldBeRetrieved()
        {
            var json = await Context.ResponseMessage.Content.ReadAsStringAsync();
            var model = ApiRequestHelper.DeserialiseSnakeCaseJsonToResponse<List<ParticipantResponse>>(json);
            CheckParticipantData(model);
        }

        [Given(@"I have an update suitability answers request with a (.*) hearing id and (.*) participant id")]
        [Given(@"I have an update suitability answers request with an (.*) hearing id and (.*) participant id")]
        public async Task GivenIHaveAnUpdateSuitabilityAnswersRequest(Scenario hearingScenario, Scenario participantScenario)
        {
            var seededHearing = await Context.TestDataManager.SeedVideoHearing();
            NUnit.Framework.TestContext.WriteLine($"New seeded video hearing id: {seededHearing.Id}");
            Context.NewHearingId = seededHearing.Id;
            Guid participantId;
            Guid hearingId;

            switch (hearingScenario)
            {
                case Scenario.Valid:
                    {
                        hearingId = Context.NewHearingId;
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
                default: throw new ArgumentOutOfRangeException(nameof(hearingScenario), hearingScenario, null);
            }
            switch (participantScenario)
            {
                case Scenario.Valid:
                    {
                        participantId = seededHearing.GetParticipants().First().Id;
                        break;
                    }
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
                default: throw new ArgumentOutOfRangeException(nameof(participantScenario), participantScenario, null);
            }

            Context.Uri = _endpoints.UpdateSuitabilityAnswers(hearingId, participantId);
            Context.HttpMethod = HttpMethod.Put;
            var request = UpdateSuitabilityAnswersRequest();
            var jsonBody = ApiRequestHelper.SerialiseRequestToSnakeCaseJson(request);
            Context.HttpContent = new StringContent(jsonBody, Encoding.UTF8, "application/json");
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
            var json = await Context.ResponseMessage.Content.ReadAsStringAsync();
            var model = new List<ParticipantResponse> { ApiRequestHelper.DeserialiseSnakeCaseJsonToResponse<ParticipantResponse>(json) };
            CheckParticipantData(model);
        }

        [Then(@"the participant should be (.*)")]
        public void ThenTheParticipantShouldBeAddedOrRemoved(string state)
        {
            Hearing hearingFromDb;
            using (var db = new BookingsDbContext(Context.BookingsDbContextOptions))
            {
                hearingFromDb = db.VideoHearings
                    .Include("Participants.Person").AsNoTracking()
                    .Single(x => x.Id == Context.NewHearingId);
            }
            if (state.Equals("added"))
            {
                hearingFromDb.GetParticipants().Count.Should().BeGreaterThan(3);
                foreach (var participantRequest in Context.Participants)
                {
                    hearingFromDb.GetParticipants().Any(x => x.Person.Username == participantRequest.Username).Should()
                        .BeTrue();
                }
            }
            if (state.Equals("removed"))
            {               
                hearingFromDb.GetParticipants().Any(x => x.Id == Context.Participant.Id).Should().BeFalse();
            }
        }

        [Given(@"I have an update participant in a hearing request with a (.*) hearing id")]
        [Given(@"I have an update participant in a hearing request with an (.*) hearing id")]
        public async Task GivenIHaveAnUpdateParticipantInAHearingRequestWithANonexistentHearingId(Scenario scenario)
        {
            var seededHearing = await Context.TestDataManager.SeedVideoHearing();
            Context.NewHearingId = seededHearing.Id;
            var participantId = seededHearing.GetParticipants().First().Id;
            var updateParticipantRequest = new UpdateParticipantRequestBuilder().Build();
            Guid hearingId;

            var jsonBody = ApiRequestHelper.SerialiseRequestToSnakeCaseJson(updateParticipantRequest);
            Context.HttpContent = new StringContent(jsonBody, Encoding.UTF8, "application/json");

            switch (scenario)
            {
                case Scenario.Valid:
                    {
                        hearingId = Context.NewHearingId;
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
            Context.Uri = _endpoints.UpdateParticipantDetails(hearingId,participantId);
            Context.HttpMethod = HttpMethod.Put;
        }

        [Given(@"I have an update participant in a hearing request with a invalid solicitors reference")]
        public async Task GivenIHaveAnUpdateParticipantInAHearingRequestWithAInvalidSolicitorsReference()
        {
            var seededHearing = await Context.TestDataManager.SeedVideoHearing();
            Context.NewHearingId = seededHearing.Id;
            var participantId = seededHearing.GetParticipants().First(x=>x.HearingRole.UserRole.IsRepresentative).Id;
            var updateParticipantRequest = new UpdateParticipantRequestBuilder().Build();
            var hearingId = seededHearing.Id;
            updateParticipantRequest.SolicitorsReference = string.Empty;
            var jsonBody = ApiRequestHelper.SerialiseRequestToSnakeCaseJson(updateParticipantRequest);
            Context.HttpContent = new StringContent(jsonBody, Encoding.UTF8, "application/json");

            Context.Uri = _endpoints.UpdateParticipantDetails(hearingId, participantId);
            Context.HttpMethod = HttpMethod.Put;
        }


        [Given(@"I have an update participant in a hearing request with a invalid address")]
        public async Task GivenIHaveAnUpdateParticipantInAHearingRequestWithAInvalidAddress()
        {
            var seededHearing = await Context.TestDataManager.SeedVideoHearing();
            Context.NewHearingId = seededHearing.Id;
            var participantId = seededHearing.GetParticipants().First(x => x.HearingRole.UserRole.IsIndividual).Id;
            var updateParticipantRequest = new UpdateParticipantRequestBuilder().Build();
            var hearingId = seededHearing.Id;
            updateParticipantRequest.Street = string.Empty;
            updateParticipantRequest.HouseNumber = string.Empty;
            updateParticipantRequest.Postcode = string.Empty;
            updateParticipantRequest.City = string.Empty;
            updateParticipantRequest.County = string.Empty;
            var jsonBody = ApiRequestHelper.SerialiseRequestToSnakeCaseJson(updateParticipantRequest);
            Context.HttpContent = new StringContent(jsonBody, Encoding.UTF8, "application/json");

            Context.Uri = _endpoints.UpdateParticipantDetails(hearingId, participantId);
            Context.HttpMethod = HttpMethod.Put;
        }


        private static AddParticipantsToHearingRequest BuildRequest()
        {
            var newParticipant = new ParticipantRequestBuilder("Defendant", "Defendant LIP").Build();

            return new AddParticipantsToHearingRequest
            {
                Participants = new List<ParticipantRequest> { newParticipant }
            };
        }



        private List<SuitabilityAnswersRequest> UpdateSuitabilityAnswersRequest()
        {
            var answers = new List<SuitabilityAnswersRequest>();
            answers.Add(new SuitabilityAnswersRequest {
                Key = "_Key",
                Answer ="_Answer",
                ExtendedAnswer = "_ExtendedAnswer"
            });
            return answers;
        }





        [TearDown]
        public async Task TearDown()
        {
            if (Context.NewHearingId != Guid.Empty)
            {
                NUnit.Framework.TestContext.WriteLine($"Removing test hearing {Context.NewHearingId}");
                await TestDataManager.RemoveVideoHearing(Context.NewHearingId);
            }
            if (Context.OldHearingId != Guid.Empty)
            {
                NUnit.Framework.TestContext.WriteLine($"Removing test hearing {Context.OldHearingId}");
                await TestDataManager.RemoveVideoHearing(Context.OldHearingId);
            }
        }
    }
}
