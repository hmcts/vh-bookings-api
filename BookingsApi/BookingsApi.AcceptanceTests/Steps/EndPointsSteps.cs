using BookingsApi.Contract.V1.Requests;
using TechTalk.SpecFlow;
using static Testing.Common.Builders.Api.ApiUriFactory.JVEndPointEndpoints;
using static Testing.Common.Builders.Api.ApiUriFactory.HearingsEndpoints;
using AcceptanceTests.Common.Api.Helpers;
using BookingsApi.Contract.V1.Responses;
using System;
using FluentAssertions;
using System.Linq;
using BookingsApi.AcceptanceTests.Contexts;
using BookingsApi.AcceptanceTests.Helpers;

namespace BookingsApi.AcceptanceTests.Steps
{
    [Binding]
    public class EndPointsSteps
    {
        private readonly TestContext _context;
        private static string AddEndPointRequest = "AddEndPointRequest";
        private static string RemovedEndPointId = "RemovedEndPointId";
        private static string UpdatedEndPointId = "UpdatedEndPointId";
        private static string UpdateEndPointRequest = "UpdateEndPointRequest";

        public EndPointsSteps(TestContext context)
        {
            _context = context;
        }

        [Given(@"I have add endpoint to a hearing request with a (.*) hearing id")]
        public void GivenIHaveAddEndpointToAHearingRequestWithAHearingId(Scenario scenario)
        {
            var hearingId = GetHearingIdForTest(scenario);
            var addEndpointRequest = new AddEndpointRequest()
            {
                DisplayName = $"DisplayName{Guid.NewGuid()}",
            };

            _context.TestData.TestContextData.Add(AddEndPointRequest, addEndpointRequest);
            _context.Request = _context.Post(AddEndpointToHearing(hearingId), addEndpointRequest);
        }

        [Given(@"I have update endpoint to a hearing request with a (.*) hearing id")]
        public void GivenIHaveUpdateEndpointToAHearingRequestWithANonexistentHearingId(Scenario scenario)
        {
            var hearingId = GetHearingIdForTest(scenario);
            var updateEndpointRequest = new UpdateEndpointRequest()
            {
                DisplayName = $"DisplayName{Guid.NewGuid()}",
            };

            var updatedEndPointId = _context.TestData.EndPointResponses.First().Id;
            _context.TestData.TestContextData.Add(UpdatedEndPointId, updatedEndPointId);
            _context.TestData.TestContextData.Add(UpdateEndPointRequest, updateEndpointRequest);
            _context.Request = _context.Patch(UpdateEndpoint(hearingId, updatedEndPointId), updateEndpointRequest);
        }


        [Given(@"I have remove endpoint to a hearing request with a (.*) hearing id")]
        public void GivenIHaveRemoveEndpointToAHearingRequestWithAValidHearingId(Scenario scenario)
        {
            var hearingId = GetHearingIdForTest(scenario);
            var removedEndPointId = _context.TestData.EndPointResponses.First().Id;
            _context.TestData.TestContextData.Add(RemovedEndPointId, removedEndPointId);
            _context.Request = _context.Delete(RemoveEndPointFromHearing(hearingId, removedEndPointId));
        }

        [Given(@"I have remove nonexistent endpoint to a hearing request with a (.*) hearing id")]
        public void GivenIHaveRemoveNonexistentEndpointToAHearingRequestWithAValidHearingId(Scenario scenario)
        {
            var hearingId = GetHearingIdForTest(scenario);
            _context.Request = _context.Delete(RemoveEndPointFromHearing(hearingId, Guid.NewGuid()));
        }

        [Given(@"I have update nonexistent endpoint to a hearing request with a (.*) hearing id")]
        public void GivenIHaveUpdateNonexistentEndpointToAHearingRequestWithAValidHearingId(Scenario scenario)
        {
            var hearingId = GetHearingIdForTest(scenario);
            _context.Request = _context.Patch(UpdateEndpoint(hearingId, Guid.NewGuid()), new UpdateEndpointRequest()
            {
                DisplayName = "Test",
            });
        }


        [Given(@"I have add endpoint with invalid data to a hearing request with a (.*) hearing id")]
        public void GivenIHaveAddEndpointWithInvalidDataToAHearingRequestWithAValidHearingId(Scenario scenario)
        {
            var hearingId = GetHearingIdForTest(scenario);
            
            _context.Request = _context.Post(AddEndpointToHearing(hearingId), new AddEndpointRequest()
            {
                DisplayName = string.Empty,
            });
        }

        [Given(@"I have update endpoint with invalid data to a hearing request with a (.*) hearing id")]
        public void GivenIHaveUpdateEndpointWithInvalidDataToAHearingRequestWithAValidHearingId(Scenario scenario)
        {
            var hearingId = GetHearingIdForTest(scenario);

            var updatedEndPointId = _context.TestData.EndPointResponses.First().Id;
            _context.Request = _context.Patch(UpdateEndpoint(hearingId, updatedEndPointId), new UpdateEndpointRequest()
            {
                DisplayName = string.Empty,
            });
        }


        [Then(@"the endpoint should be added")]
        public void ThenTheEndpointShouldBeAdded()
        {
            var hearing = GetHearing();
            var requestUsed = (AddEndpointRequest)_context.TestData.TestContextData[AddEndPointRequest];
            var endpointAdded = hearing.Endpoints.FirstOrDefault(ep => ep.DisplayName == requestUsed.DisplayName);

            endpointAdded.Should().NotBeNull();
            endpointAdded.Pin.Should().NotBeNull();
            endpointAdded.Sip.Should().NotBeNull();
        }

        [Then(@"the endpoint should be updated")]
        public void ThenTheEndpointShouldBeUpdated()
        {
            var hearing = GetHearing();
            var updatedEndPointId = (Guid)_context.TestData.TestContextData[UpdatedEndPointId];
            var requestUsed = (UpdateEndpointRequest)_context.TestData.TestContextData[UpdateEndPointRequest];

            var endpointUpdated = hearing.Endpoints.FirstOrDefault(ep => ep.Id == updatedEndPointId);

            endpointUpdated.Should().NotBeNull();
            endpointUpdated.Pin.Should().NotBeNull();
            endpointUpdated.Sip.Should().NotBeNull();
            requestUsed.DisplayName.Should().Be(endpointUpdated.DisplayName);
        }

        [Then(@"the endpoint should be deleted")]
        public void ThenTheEndpointShouldBeDeleted()
        {
            var hearing = GetHearing();
            var removedEndpointId = (Guid)_context.TestData.TestContextData[RemovedEndPointId];
            hearing.Endpoints.Exists(ep => ep.Id == removedEndpointId).Should().BeFalse();
        }

        private HearingDetailsResponse GetHearing()
        {
            _context.Request = _context.Get(GetHearingDetailsById(_context.TestData.Hearing.Id));
            _context.Response = _context.Client().Execute(_context.Request);
            var model = RequestHelper.Deserialise<HearingDetailsResponse>(_context.Response.Content);
            return model;
        }

        private Guid GetHearingIdForTest(Scenario scenario)
        {
            Guid hearingId;
            switch (scenario)
            {
                case Scenario.Valid:
                    hearingId = _context.TestData.Hearing.Id;
                    break;
                case Scenario.Nonexistent:
                    hearingId = Guid.NewGuid();
                    break;
                case Scenario.Invalid:
                    hearingId = Guid.Empty;
                    break;
                default: throw new ArgumentOutOfRangeException(nameof(scenario), scenario, null);
            }

            return hearingId;
        }
    }
}
