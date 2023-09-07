using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using BookingsApi.Contract.Requests;
using BookingsApi.Contract.Responses;
using BookingsApi.IntegrationTests.Helper;
using BookingsApi.Validations;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using NUnit.Framework;
using Testing.Common.Builders.Api;
using Testing.Common.Builders.Api.Request;

namespace BookingsApi.IntegrationTests.Api.Hearings;

public class BookNewHearingTests : ApiTest
{
    private readonly List<Guid> _hearingIds = new();

    [SetUp]
    public void Setup()
    {
        _hearingIds.Clear();
    }
    
    [TearDown]
    public new async Task TearDown()
    {
        foreach (var hearingId in _hearingIds)
        {
            await Hooks.RemoveVideoHearing(hearingId);
        }
    }
    
    [Test]
    public async Task should_book_a_hearing()
    {
        // arrange
        var request = CreateBookingRequestWithParticipantsAndAJudge();

        // act
        using var client = Application.CreateClient();
        var result = await client.PostAsync(ApiUriFactory.HearingsEndpoints.BookNewHearing, RequestBody.Set(request));

        // assert
        result.IsSuccessStatusCode.Should().BeTrue();
        result.StatusCode.Should().Be(HttpStatusCode.Created);
        
        var getHearingUri = result.Headers.Location;
        var getResponse = await client.GetAsync(getHearingUri);
        var createdResponse = await ApiClientResponse.GetResponses<HearingDetailsResponse>(result.Content);
        var hearingResponse = await ApiClientResponse.GetResponses<HearingDetailsResponse>(getResponse.Content);
        createdResponse.Should().BeEquivalentTo(hearingResponse);
        _hearingIds.Add(hearingResponse.Id);
    }
    
    [Test]
    public async Task should_return_validation_error_when_booking_a_hearing_with_multiple_judges()
    {
        // arrange
        var request = CreateBookingRequestWithParticipantsAndAJudge();
        request.Participants.Add(new ()
        {
            CaseRoleName = "Judge",
            HearingRoleName = "Judge",
            Representee = null,
            FirstName = "AdditionalJudgeFirstName",
            LastName = "AdditionalJudgeLastName",
            ContactEmail = "random@contact.com",
            Username = "random@contact.com",
            DisplayName = "Additional Judge"
        });

        // act
        using var client = Application.CreateClient();
        var result = await client.PostAsync(ApiUriFactory.HearingsEndpoints.BookNewHearing, RequestBody.Set(request));

        // assert
        result.IsSuccessStatusCode.Should().BeFalse();
        result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var validationProblemDetails = await ApiClientResponse.GetResponses<ValidationProblemDetails>(result.Content);
        validationProblemDetails.Errors.SelectMany(x=> x.Value).Should()
            .Contain("A participant with Judge role already exists in the hearing");
    }

    private BookNewHearingRequest CreateBookingRequestWithParticipantsAndAJudge()
    {
        var hearingSchedule = DateTime.UtcNow.AddMinutes(5);
        var caseName = "Bookings Api Integration Automated";
        var request = new SimpleBookNewHearingRequest(caseName, hearingSchedule).Build();
        request.CaseTypeName = "Generic";
        request.HearingTypeName = "Automated Test";
        request.HearingVenueName = "Birmingham Civil and Family Justice Centre";
        return request;
    }
}