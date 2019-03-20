using System;
using System.Net;
using Bookings.AcceptanceTests.Contexts;
using Bookings.AcceptanceTests.Models;
using Bookings.Api.Contract.Responses;
using FluentAssertions;
using TechTalk.SpecFlow;
using Testing.Common.Builders.Api;

namespace Bookings.AcceptanceTests.Steps
{
    [Binding]
    public sealed class HearingsSteps : BaseSteps
    {
        private readonly TestContext _acTestContext;
        private readonly HearingsEndpoints _endpoints = new ApiUriFactory().HearingsEndpoints;

        public HearingsSteps(TestContext acTestContext)
        {
            _acTestContext = acTestContext;
        }

        [Given(@"I have a get details for a given hearing request with a valid hearing id")]
        public void GivenIHaveAGetDetailsForAGivenHearingRequestWithAValidHearingId()
        {
            _acTestContext.Request = _acTestContext.Get(_endpoints.GetHearingDetailsById(_acTestContext.HearingId));
        }

        [Given(@"I have a valid book a new hearing request")]
        public void GivenIHaveAValidBookANewHearingRequest()
        {
            var bookNewHearingRequest = CreateHearingRequest.BuildRequest();         
            _acTestContext.Request = _acTestContext.Post(_endpoints.BookNewHearing(), bookNewHearingRequest);
        }

        [Given(@"I have a valid update hearing request")]
        public void GivenIHaveAValidUpdateHearingRequest()
        {
            var updateHearingRequest = UpdateHearingRequest.BuildRequest();
            _acTestContext.Request = _acTestContext.Put(_endpoints.UpdateHearingDetails(_acTestContext.HearingId), updateHearingRequest);
        }

        [Given(@"I have a remove hearing request with a valid hearing id")]
        public void GivenIHaveARemoveHearingRequestWithAValidHearingId()
        {
            _acTestContext.Request = _acTestContext.Delete(_endpoints.RemoveHearing(_acTestContext.HearingId));
        }       

        [Then(@"hearing details should be retrieved")]
        public void ThenAHearingDetailsShouldBeRetrieved()
        {
            var model = ApiRequestHelper.DeserialiseSnakeCaseJsonToResponse<HearingDetailsResponse>(_acTestContext.Json);
            model.Should().NotBeNull();
            _acTestContext.HearingId = model.Id;

            model.CaseTypeName.Should().NotBeNullOrEmpty();
            foreach (var theCase in model.Cases)
            {
                theCase.Name.Should().NotBeNullOrEmpty();
                theCase.Number.Should().NotBeNullOrEmpty();
            }
            model.HearingTypeName.Should().NotBeNullOrEmpty();
            model.HearingVenueName.Should().NotBeNullOrEmpty();
            foreach (var participant in model.Participants)
            {
                participant.CaseRoleName.Should().NotBeNullOrEmpty();
                participant.ContactEmail.Should().NotBeNullOrEmpty();
                participant.DisplayName.Should().NotBeNullOrEmpty();
                participant.FirstName.Should().NotBeNullOrEmpty();
                participant.HearingRoleName.Should().NotBeNullOrEmpty();
                participant.Id.Should().NotBeEmpty();
                participant.LastName.Should().NotBeNullOrEmpty();
                participant.MiddleNames.Should().NotBeNullOrEmpty();
                participant.TelephoneNumber.Should().NotBeNullOrEmpty();
                participant.Title.Should().NotBeNullOrEmpty();
                participant.UserRoleName.Should().NotBeNullOrEmpty();
            }
            model.ScheduledDateTime.Should().BeAfter(DateTime.MinValue);
            model.ScheduledDuration.Should().BePositive();
            model.HearingRoomName.Should().NotBeNullOrEmpty();
            model.OtherInformation.Should().NotBeNullOrEmpty();
        }

        [Then(@"hearing details should be updated")]
        public void ThenHearingDetailsShouldBeUpdated()
        {
            var model = ApiRequestHelper.DeserialiseSnakeCaseJsonToResponse<HearingDetailsResponse>(_acTestContext.Json);
            model.Should().NotBeNull();
            model.ScheduledDuration.Should().Be(100);
            model.ScheduledDateTime.Should().Be(DateTime.Today.AddDays(3).AddHours(11).AddMinutes(45));
            model.HearingVenueName.Should().Be("Manchester Civil and Family Justice Centre");
            model.OtherInformation.Should().Be("OtherInformation12345");
            model.HearingRoomName.Should().Be("HearingRoomName12345");
        }

        [Then(@"the hearing no longer exists")]
        public void ThenTheHearingNoLongerExists()
        {
            _acTestContext.Request = _acTestContext.Get(_endpoints.GetHearingDetailsById(_acTestContext.HearingId));
            _acTestContext.Response = _acTestContext.Client().Execute(_acTestContext.Request);
            _acTestContext.Response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }
    }
}
