﻿using System;
using System.Net;
using AcceptanceTests.Common.Api.Helpers;
using Bookings.AcceptanceTests.Contexts;
using Bookings.AcceptanceTests.Models;
using Bookings.Api.Contract.Responses;
using FluentAssertions;
using TechTalk.SpecFlow;
using static Testing.Common.Builders.Api.ApiUriFactory.HearingsEndpoints;

namespace Bookings.AcceptanceTests.Steps
{
    [Binding]
    public sealed class CommonSteps
    {
        private readonly TestContext _context;

        public CommonSteps(TestContext context)
        {
            _context = context;
        }

        [Given(@"I have a hearing")]
        public void GivenIHaveAHearing()
        {
            var bookNewHearingRequest = new CreateHearingRequestBuilder(_context.TestData.CaseName).WithContext(_context).Build();
            _context.Request = _context.Post(BookNewHearing, bookNewHearingRequest);
            _context.Response = _context.Client().Execute(_context.Request);
            _context.Response.StatusCode.Should().Be(HttpStatusCode.Created);

            var model = RequestHelper.DeserialiseSnakeCaseJsonToResponse<HearingDetailsResponse>(_context.Response.Content);
            model.Should().NotBeNull();                 
            model.ScheduledDuration.Should().NotBe(100);
            model.ScheduledDateTime.Should().NotBe(DateTime.Today.AddDays(3).AddHours(11).AddMinutes(45));
            model.HearingVenueName.Should().NotBe("Manchester Civil and Family Justice Centre");

            _context.TestData.Hearing = model;
            _context.TestData.Hearing.Id = model.Id;
            _context.TestData.ParticipantsResponses = model.Participants;
            _context.TestData.EndPointResponses = model.Endpoints;
        }

        [When(@"I send the request to the endpoint")]
        public void WhenISendTheRequestToTheEndpoint()
        {
            _context.Response = _context.Client().Execute(_context.Request);
        }

        [Then(@"the response should have the status (.*) and success status (.*)")]
        public void ThenTheResponseShouldHaveTheStatusAndSuccessStatus(HttpStatusCode httpStatusCode, bool isSuccess)
        {
            _context.Response.StatusCode.Should().Be(httpStatusCode);
            _context.Response.IsSuccessful.Should().Be(isSuccess);
        }
    }
}