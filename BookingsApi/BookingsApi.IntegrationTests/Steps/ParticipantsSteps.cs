﻿using System.Text;
using AcceptanceTests.Common.Api.Helpers;
using BookingsApi.Contract.V1.Requests;
using BookingsApi.Contract.V1.Responses;
using TechTalk.SpecFlow;
using Testing.Common.Builders.Api.V1.Request;
using TestContext = BookingsApi.IntegrationTests.Contexts.TestContext;
using static Testing.Common.Builders.Api.ApiUriFactory.ParticipantsEndpoints;

namespace BookingsApi.IntegrationTests.Steps
{
    [Binding]
    public sealed class ParticipantsBaseSteps : BaseSteps
    {
        public ParticipantsBaseSteps(TestContext apiTestContext) : base(apiTestContext)
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
                    Context.TestData.NewHearingId = seededHearing.Id;
                    hearingId = Context.TestData.NewHearingId;
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
            Context.Uri = GetAllParticipantsInHearing(hearingId);
            Context.HttpMethod = HttpMethod.Get;
        }

        [Given(@"I have an add participant to a hearing request with a (.*) hearing id")]
        [Given(@"I have an add participant to a hearing request with an (.*) hearing id")]
        public async Task GivenIHaveAnAddParticipantToAHearingRequestWithAHearingId(Scenario scenario)
        {
            var request = BuildRequest();
            Context.TestData.Participants = request.Participants;
            var jsonBody = RequestHelper.Serialise(request);
            Context.HttpContent = new StringContent(jsonBody, Encoding.UTF8, "application/json");
            Guid hearingId;
            switch (scenario)
            {
                case Scenario.Valid:
                {
                    var seededHearing = await Context.TestDataManager.SeedVideoHearing();
                    Context.TestData.NewHearingId = seededHearing.Id;
                    hearingId = Context.TestData.NewHearingId;
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
            Context.Uri = AddParticipantsToHearing(hearingId);
            Context.HttpMethod = HttpMethod.Post;
        }

        [When(@"I send the same request but with a new hearing id")]
        public async Task WhenISendTheSameRequestButWithANewHearingId()
        {
            var seededHearing = await Context.TestDataManager.SeedVideoHearing();
            var noOfParticipants = seededHearing.GetParticipants().Count;
            Context.TestData.OldHearingId = Context.TestData.NewHearingId;
            Context.TestData.NewHearingId = seededHearing.Id;
            NUnit.Framework.TestContext.WriteLine($"New seeded video hearing id: {seededHearing.Id}");
            seededHearing.GetParticipants().Count.Should().Be(noOfParticipants);
            Context.Uri = AddParticipantsToHearing(Context.TestData.NewHearingId);
            Context.Response = await SendPostRequestAsync(Context);
        }

        [Given(@"I have an add participants in a hearing request with an invalid participant")]
        public async Task GivenIHaveAnAddParticipantToAHearingRequestWithAParticipantId()
        {
            var request = new InvalidRequest();
            var jsonBody = RequestHelper.Serialise(request.BuildRequest());
            Context.HttpContent = new StringContent(jsonBody, Encoding.UTF8, "application/json");
            var seededHearing = await Context.TestDataManager.SeedVideoHearing();
            Context.TestData.NewHearingId = seededHearing.Id;
            NUnit.Framework.TestContext.WriteLine($"New seeded video hearing id: {seededHearing.Id}");
            Context.Uri = AddParticipantsToHearing(Context.TestData.NewHearingId);
            Context.HttpMethod = HttpMethod.Post;
        }

        [Given(@"I have a get a single participant in a hearing request with a (.*) hearing id")]
        [Given(@"I have a get a single participant in a hearing request with an (.*) hearing id")]
        public async Task GivenIHaveAGetASingleParticipantInAHearingRequestWithAValidHearingId(Scenario scenario)
        {
            Guid hearingId;
            var seededHearing = await Context.TestDataManager.SeedVideoHearing();
            NUnit.Framework.TestContext.WriteLine($"New seeded video hearing id: {seededHearing.Id}");
            Context.TestData.NewHearingId = seededHearing.Id;
            switch (scenario)
            {
                case Scenario.Valid:
                {
                    hearingId = Context.TestData.NewHearingId;
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
            Context.Uri = GetParticipantInHearing(hearingId, seededHearing.GetParticipants()[0].Id);
            Context.HttpMethod = HttpMethod.Get;
        }

        [Given(@"I have a get a single participant in a hearing request with a (.*) participant id")]
        [Given(@"I have a get a single participant in a hearing request with an (.*) participant id")]
        public async Task GivenIHaveAGetASingleParticipantInAHearingRequestWithAParticipantId(Scenario scenario)
        {
            var seededHearing = await Context.TestDataManager.SeedVideoHearing();
            NUnit.Framework.TestContext.WriteLine($"New seeded video hearing id: {seededHearing.Id}");
            Context.TestData.NewHearingId = seededHearing.Id;
            var participantId = scenario switch
            {
                Scenario.Nonexistent => Guid.NewGuid(),
                Scenario.Invalid => Guid.Empty,
                _ => throw new ArgumentOutOfRangeException(nameof(scenario), scenario, null)
            };
            Context.Uri = GetParticipantInHearing(Context.TestData.NewHearingId, participantId);
            Context.HttpMethod = HttpMethod.Get;
        }

        [Given(@"I have a remove participant from a hearing request with a (.*) hearing id")]
        [Given(@"I have a remove participant from a hearing request with an (.*) hearing id")]
        public async Task GivenIHaveARemoveParticipantFromAHearingWithAValidHearingId(Scenario scenario)
        {
            var seededHearing = await Context.TestDataManager.SeedVideoHearing();
            NUnit.Framework.TestContext.WriteLine($"New seeded video hearing id: {seededHearing.Id}");
            Context.TestData.NewHearingId = seededHearing.Id;
            Context.TestData.Participant = seededHearing.GetParticipants()[0];
            Context.HttpMethod = HttpMethod.Delete;

            var participantToRemove = seededHearing.GetParticipants()[0];
            var participantId = participantToRemove.Id;
            Context.TestData.RemovedPersons.Add(participantToRemove.Person.ContactEmail);
            Guid hearingId;

            switch (scenario)
            {
                case Scenario.Valid:
                    {
                        hearingId = Context.TestData.NewHearingId;
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
            Context.Uri = RemoveParticipantFromHearing(hearingId, participantId);
        }

        [Given(@"I have a remove participant from a hearing request with a (.*) participant id")]
        [Given(@"I have a remove participant from a hearing request with an (.*) participant id")]
        public async Task GivenIHaveARemoveParticipantFromAHearingWithAParticipantId(Scenario scenario)
        {
            var seededHearing = await Context.TestDataManager.SeedVideoHearing();
            NUnit.Framework.TestContext.WriteLine($"New seeded video hearing id: {seededHearing.Id}");
            Context.TestData.NewHearingId = seededHearing.Id;
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
            Context.Uri = RemoveParticipantFromHearing(Context.TestData.NewHearingId, participantId);
        }

        [Then(@"a list of hearing participants should be retrieved")]
        public async Task ThenAListOfHearingParticipantsShouldBeRetrieved()
        {
            var json = await Context.Response.Content.ReadAsStringAsync();
            var model = RequestHelper.Deserialise<List<ParticipantResponse>>(json);
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
            var json = await Context.Response.Content.ReadAsStringAsync();
            var model = new List<ParticipantResponse> { RequestHelper.Deserialise<ParticipantResponse>(json) };
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
                    .Single(x => x.Id == Context.TestData.NewHearingId);
            }
            if (state.Equals("added"))
            {
                hearingFromDb.GetParticipants().Count.Should().BeGreaterThan(3);
                foreach (var participantRequest in Context.TestData.Participants)
                {
                    hearingFromDb.GetParticipants().Any(x => x.Person.ContactEmail == participantRequest.ContactEmail).Should()
                        .BeTrue();
                }
            }
            if (state.Equals("removed"))
            {               
                hearingFromDb.GetParticipants().Any(x => x.Id == Context.TestData.Participant.Id).Should().BeFalse();
            }
        }

        private static AddParticipantsToHearingRequest BuildRequest()
        {
            var newParticipant = new ParticipantRequestBuilder("Respondent", "Litigant in person").Build();

            return new AddParticipantsToHearingRequest
            {
                Participants = new List<ParticipantRequest> { newParticipant }
            };
        }

        [TearDown]
        public async Task TearDown()
        {
            if (Context.TestData.NewHearingId != Guid.Empty)
            {
                NUnit.Framework.TestContext.WriteLine($"Removing test hearing {Context.TestData.NewHearingId}");
                await TestDataManager.RemoveVideoHearing(Context.TestData.NewHearingId);
            }
            if (Context.TestData.OldHearingId != Guid.Empty)
            {
                NUnit.Framework.TestContext.WriteLine($"Removing test hearing {Context.TestData.OldHearingId}");
                await TestDataManager.RemoveVideoHearing(Context.TestData.OldHearingId);
            }
        }
    }
}
