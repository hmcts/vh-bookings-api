using System;
using System.Collections.Generic;
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
    public sealed class CommonSteps
    {
        private readonly TestContext _context;
        private readonly HearingsEndpoints _endpoints = new ApiUriFactory().HearingsEndpoints;

        public CommonSteps(TestContext context)
        {
            _context = context;
        }

        [Given(@"I have a hearing")]
        public void GivenIHaveAHearing()
        {
            var bookNewHearingRequest = new CreateHearingRequestBuilder().WithContext(_context).Build();
            _context.Request = _context.Post(_endpoints.BookNewHearing(), bookNewHearingRequest);
            _context.Response = _context.Client().Execute(_context.Request);
            _context.Json = _context.Response.Content;
            _context.Response.StatusCode.Should().Be(HttpStatusCode.Created);

            var model = ApiRequestHelper.DeserialiseSnakeCaseJsonToResponse<HearingDetailsResponse>(_context.Json);
            model.Should().NotBeNull();                 
            model.ScheduledDuration.Should().NotBe(100);
            model.ScheduledDateTime.Should().NotBe(DateTime.Today.AddDays(3).AddHours(11).AddMinutes(45));
            model.HearingVenueName.Should().NotBe("Manchester Civil and Family Justice Centre");

            _context.Hearing = model;
            _context.HearingId = model.Id;
            _context.Participants = new List<ParticipantResponse>(model.Participants);
        }

        [When(@"I send the request to the endpoint")]
        public void WhenISendTheRequestToTheEndpoint()
        {
            _context.Response = _context.Client().Execute(_context.Request);
            if (_context.Response.Content != null)
                _context.Json = _context.Response.Content;
        }

        [Then(@"the response should have the status (.*) and success status (.*)")]
        public void ThenTheResponseShouldHaveTheStatusAndSuccessStatus(HttpStatusCode httpStatusCode, bool isSuccess)
        {
            _context.Response.StatusCode.Should().Be(httpStatusCode);
            _context.Response.IsSuccessful.Should().Be(isSuccess);
        }
    }
}