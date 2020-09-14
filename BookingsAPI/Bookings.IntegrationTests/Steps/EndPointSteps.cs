using AcceptanceTests.Common.Api.Helpers;
using Bookings.Api.Contract.Requests;
using Bookings.DAL;
using Bookings.Domain;
using Bookings.Domain.Enumerations;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using TechTalk.SpecFlow;
using static Testing.Common.Builders.Api.ApiUriFactory.JVEndPointEndpoints;

namespace Bookings.IntegrationTests.Steps
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
            var seededHearing = await Context.TestDataManager.SeedVideoHearing(null, false, BookingStatus.Booked, 3);
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

        [Given(@"I have remove endpoint from a hearing request")]
        public void GivenIHaveRemoveEndpointFromAHearingRequest()
        {
            var hearing = GetHearingFromDb();
            Context.HttpMethod = HttpMethod.Delete;
            Context.Uri = RemoveEndPointFromHearing(_hearingId, hearing.Endpoints.First().Id);
        }

        [Given(@"I have update display name of an endpoint request")]
        public void GivenIHaveUpdateDisplayNameOfAnEndpointRequest()
        {
            var hearing = GetHearingFromDb();
            var updatedEndPointId = hearing.Endpoints.First().Id;
            PrepareUpdateEndpointRequest(_hearingId, updatedEndPointId, new UpdateEndpointRequest()
            {
                DisplayName = "UpdatedDisplayName",
            });
            
            Context.TestData.TestContextData.Add(EndPointSteps.UpdatedEndPointId, updatedEndPointId);
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
            var jsonBody = RequestHelper.SerialiseRequestToSnakeCaseJson(request);
            Context.HttpContent = new StringContent(jsonBody, Encoding.UTF8, "application/json");

            Context.Uri = AddEndpointToHearing(hearingId);
            Context.HttpMethod = HttpMethod.Post;
        }

        private void PrepareUpdateEndpointRequest(Guid hearingId, Guid endpointId, UpdateEndpointRequest request)
        {
            var jsonBody = RequestHelper.SerialiseRequestToSnakeCaseJson(request);
            Context.HttpContent = new StringContent(jsonBody, Encoding.UTF8, "application/json");

            Context.Uri = UpdateEndpointDisplayName(hearingId, endpointId);
            Context.HttpMethod = HttpMethod.Patch;
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
            var hearingFromDb = GetHearingFromDb();
            var endpointsUpdated = (Guid)Context.TestData.TestContextData[EndPointSteps.UpdatedEndPointId];

            hearingFromDb.GetEndpoints().First(ep => ep.Id == endpointsUpdated).DisplayName.Should().Be("UpdatedDisplayName");
        }

        private Hearing GetHearingFromDb()
        {
            Hearing hearingFromDb;
            using (var db = new BookingsDbContext(Context.BookingsDbContextOptions))
            {
                hearingFromDb = db.VideoHearings
                    .Include(hearing => hearing.Endpoints)
                    .AsNoTracking()
                    .Single(x => x.Id == Context.TestData.NewHearingId);
            }
            return hearingFromDb;
        }

        private void PersistTestHearingData(VideoHearing seededHearing)
        {
            _hearingId = seededHearing.Id;
            Context.TestData.NewHearingId = seededHearing.Id;
            Context.TestData.TestContextData.Add(ExistingEndPoints, seededHearing.GetEndpoints());
        }
    }

}
