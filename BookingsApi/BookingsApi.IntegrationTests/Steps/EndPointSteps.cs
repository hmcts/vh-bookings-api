﻿using System.Text;
using AcceptanceTests.Common.Api.Helpers;
using BookingsApi.Contract.V1.Requests;
using TechTalk.SpecFlow;
using static Testing.Common.Builders.Api.ApiUriFactory.JVEndPointEndpoints;

namespace BookingsApi.IntegrationTests.Steps
{
    [Binding]
    public sealed class EndPointSteps : BaseSteps
    {
        private Guid _hearingId;
        private Guid _removedEndPointId;
        private static string ExistingEndPoints = "ExistingEndPoints";
        private static string UpdatedEndPointId = "UpdatedEndPointId";

        public EndPointSteps(Contexts.TestContext context) : base(context)
        {
            _hearingId = Guid.Empty;
            _removedEndPointId = Guid.Empty;
        }

        [Given(@"I have a hearing with endpoints")]
        public async Task GivenIHaveAHearingWithEndpoints()
        {
            var seededHearing = await Context.TestDataManager.SeedVideoHearing(options =>
            {
                options.EndpointsToAdd = 3;
            });
            PersistTestHearingData(seededHearing);
        }

        [Given(@"I have a hearing with endpoints for cloning")]
        public async Task GivenIHaveAHearingWithEndpointsForCloning()
        {
            var seededHearing = await Context.TestDataManager.SeedVideoHearing(options =>
            {
                options.EndpointsToAdd = 3;
                options.AddPanelMember = true;
            }, isMultiDayFirstHearing:true);
            PersistTestHearingData(seededHearing);
        }

        [Given(@"I have a hearing with linked participants")]
        public async Task GivenIHaveAHearingWithLinkedParticipants()
        {
            var seededHearing = await Context.TestDataManager.SeedVideoHearing(options =>
            {
                options.EndpointsToAdd = 3;
            }, withLinkedParticipants: true);
            PersistTestHearingData(seededHearing);
        }

        [Given(@"I have a hearing with linked participants for cloning")]
        public async Task GivenIHaveAHearingWithLinkedParticipantsForCloning()
        {
            var seededHearing = await Context.TestDataManager.SeedVideoHearing(
                options => { options.EndpointsToAdd = 3; }, withLinkedParticipants: true, isMultiDayFirstHearing: true);
            PersistTestHearingData(seededHearing);
        }

        [Given(@"I have a hearing without endpoints")]
        public async Task GivenIHaveAHearingWithoutEndpoints()
        {
            var seededHearing = await Context.TestDataManager.SeedVideoHearing();
            PersistTestHearingData(seededHearing);
        }

        [Given(@"I have remove non-existent endpoint from a hearing request")]
        public void GivenIHaveRemoveNonExistentEndpointFromAHearingRequest()
        {
            Context.HttpMethod = HttpMethod.Delete;
            Context.Uri = RemoveEndPointFromHearing(_hearingId, Guid.NewGuid());
        }

        [Given(@"I have remove endpoint from a non-existent hearing request")]
        public void GivenIHaveRemoveEndpointFromANonExistentHearingRequest()
        {
            Context.HttpMethod = HttpMethod.Delete;
            Context.Uri = RemoveEndPointFromHearing(Guid.NewGuid(), Guid.NewGuid());
        }

        [Given(@"I have remove endpoint from a hearing request")]
        public void GivenIHaveRemoveEndpointFromAHearingRequest()
        {
            var hearing = GetHearingFromDb();
            Context.HttpMethod = HttpMethod.Delete;
            Context.Uri = RemoveEndPointFromHearing(_hearingId, hearing.Endpoints[0].Id);
        }

        [Given(@"I have add endpoint to a hearing request")]
        public void GivenIHaveAddEndpointToAHearingRequest()
        {
            PrepareAddEndpointRequest(_hearingId, new AddEndpointRequest()
            {
                DisplayName = "Test",
            });
        }

        private void PrepareAddEndpointRequest(Guid hearingId, AddEndpointRequest request)
        {
            var jsonBody = RequestHelper.Serialise(request);
            Context.HttpContent = new StringContent(jsonBody, Encoding.UTF8, "application/json");

            Context.Uri = AddEndpointToHearing(hearingId);
            Context.HttpMethod = HttpMethod.Post;
        }

        [Then(@"the endpoint should be removed")]
        public void ThenTheEndpointShouldBeAddedOrRemoved()
        {
            var hearingFromDb = GetHearingFromDb();
            hearingFromDb.GetEndpoints().Any(ep => ep.Id == _removedEndPointId).Should().BeFalse();
        }

        [Then(@"the endpoint should be added")]
        public void ThenTheEndpointShouldBeAdded()
        {
            var hearingFromDb = GetHearingFromDb();
            var endpointsAdded = (IList<Endpoint>)Context.TestData.TestContextData[EndPointSteps.ExistingEndPoints];
            hearingFromDb.GetEndpoints().Count.Should().BeGreaterThan(endpointsAdded.Count);
        }

        [Then(@"the endpoint should be updated")]
        public void ThenTheEndpointShouldBeUpdated()
        {
            AssertEndpointUpdated();
        }

        [Then(@"the endpoint should be updated with a defence advocate")]
        public void ThenTheEndpointShouldBeUpdatedWithADefenceAdvocate()
        {
            AssertEndpointUpdated(true);
        }

        private void AssertEndpointUpdated(bool checkForDefenceAdvocate = false)
        {
            var hearingFromDb = GetHearingFromDb();
            var endpointId = (Guid)Context.TestData.TestContextData[UpdatedEndPointId];

            var updatedEndpoint = hearingFromDb.GetEndpoints().First(ep => ep.Id == endpointId);
            updatedEndpoint.DisplayName.Should().Be("UpdatedDisplayName");

            if (!checkForDefenceAdvocate) return;
            var rep = hearingFromDb.GetParticipants().First(x => x.HearingRole.UserRole.IsRepresentative);
            updatedEndpoint.GetRepresentative().Id.Should().Be(rep.Id);
            var originalSeededEndpoints = Context.TestData.TestContextData[ExistingEndPoints] as List<Endpoint>;
            originalSeededEndpoints.First(x => x.Id == endpointId).UpdatedDate.Value.Should()
                .BeBefore(updatedEndpoint.UpdatedDate.Value);
        }

        private Hearing GetHearingFromDb()
        {
            Hearing hearingFromDb;
            using (var db = new BookingsDbContext(Context.BookingsDbContextOptions))
            {
                hearingFromDb = db.VideoHearings
                    .Include(x => x.Participants).ThenInclude(x => x.HearingRole).ThenInclude(x => x.UserRole)
                    .Include(h => h.Participants).ThenInclude(x => x.Person)
                    .Include(h => h.Endpoints).ThenInclude(x => x.GetRepresentative())
                    .AsNoTracking()
                    .Single(x => x.Id == Context.TestData.NewHearingId);
            }

            return hearingFromDb;
        }

        private void PersistTestHearingData(VideoHearing seededHearing)
        {
            _hearingId = seededHearing.Id;
            Context.TestData.NewHearingId = seededHearing.Id;
            Context.TestData.SeededHearing = seededHearing;
            Context.TestData.TestContextData.Add(ExistingEndPoints, seededHearing.GetEndpoints());
        }
    }

}
