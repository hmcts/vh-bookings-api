using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using BookingsApi.Common.Services;
using BookingsApi.Contract.V1.Requests;
using BookingsApi.Contract.V1.Responses;
using BookingsApi.IntegrationTests.Helper;
using BookingsApi.Validations.V1;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using NUnit.Framework;
using Testing.Common.Builders.Api;
using Testing.Common.Builders.Api.V1.Request;
using Testing.Common.Stubs;

namespace BookingsApi.IntegrationTests.Api.V1.Hearings;

public class BookNewHearingWithRefDataToggleOffTests : ApiTest
{
    private FeatureTogglesStub _featureToggleStub;
    private readonly List<Guid> _hearingIds = new();

    [SetUp]
    public void Setup()
    {
        _hearingIds.Clear();
        _featureToggleStub = Application.Services.GetService(typeof(IFeatureToggles)) as FeatureTogglesStub;
        _featureToggleStub!.RefData = false;
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
        var request = CreateBookingRequestWithServiceIdsAndCodes();

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
    public async Task should_return_validation_error_when_validation_fails()
    {
        // arrange
        var request = CreateBookingRequestWithServiceIdsAndCodes();
        request.HearingVenueName = null;
        request.CaseTypeName = null;
        request.HearingTypeName = null;
        request.Cases = new List<CaseRequest>();
        request.Participants = new List<ParticipantRequest>();
        request.ScheduledDuration = -100;
        request.ScheduledDateTime = DateTime.Today.AddDays(-5);

        // act
        using var client = Application.CreateClient();
        var result = await client.PostAsync(ApiUriFactory.HearingsEndpoints.BookNewHearing, RequestBody.Set(request));

        // assert
        result.IsSuccessStatusCode.Should().BeFalse();
        result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var validationProblemDetails = await ApiClientResponse.GetResponses<ValidationProblemDetails>(result.Content);
        validationProblemDetails.Errors[nameof(request.HearingVenueName)][0].Should()
            .Be(BookNewHearingRequestValidation.HearingVenueErrorMessage);
        
        validationProblemDetails.Errors[nameof(request.CaseTypeName)][0].Should()
            .Be(BookNewHearingRequestValidation.CaseTypeNameErrorMessage);
        
        validationProblemDetails.Errors[nameof(request.HearingTypeName)][0].Should()
            .Be(BookNewHearingRequestValidation.HearingTypeNameErrorMessage);
        
        validationProblemDetails.Errors[nameof(request.Cases)][0].Should()
            .Be("'Cases' must not be empty.");
        
        validationProblemDetails.Errors[nameof(request.Cases)][1].Should()
            .Be(BookNewHearingRequestValidation.CasesErrorMessage);
        
        validationProblemDetails.Errors[nameof(request.Participants)][0].Should()
            .Be("'Participants' must not be empty.");
        
        validationProblemDetails.Errors[nameof(request.Participants)][1].Should()
            .Be(BookNewHearingRequestValidation.ParticipantsErrorMessage);
        
        validationProblemDetails.Errors[nameof(request.ScheduledDuration)][0].Should()
            .Be(BookNewHearingRequestValidation.ScheduleDurationErrorMessage);
        
        validationProblemDetails.Errors[nameof(request.ScheduledDateTime)][0].Should()
            .Be(BookNewHearingRequestValidation.ScheduleDateTimeInPastErrorMessage);
    }
    
    [Test]
    public async Task should_return_validation_error_when_case_type_name_is_not_found()
    {
        // arrange
        var request = CreateBookingRequestWithServiceIdsAndCodes();
        request.CaseTypeName = "doesnotexist";

        // act
        using var client = Application.CreateClient();
        var result = await client.PostAsync(ApiUriFactory.HearingsEndpoints.BookNewHearing, RequestBody.Set(request));

        // assert
        result.IsSuccessStatusCode.Should().BeFalse();
        result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var validationProblemDetails = await ApiClientResponse.GetResponses<ValidationProblemDetails>(result.Content);
        validationProblemDetails.Errors[nameof(request.CaseTypeName)][0].Should()
            .Be("Case type does not exist");
    }
    
    [Test]
    public async Task should_return_validation_error_when_hearing_type_name_is_not_found()
    {
        // arrange
        var request = CreateBookingRequestWithServiceIdsAndCodes();
        request.HearingTypeName = "doesnotexist";

        // act
        using var client = Application.CreateClient();
        var result = await client.PostAsync(ApiUriFactory.HearingsEndpoints.BookNewHearing, RequestBody.Set(request));

        // assert
        result.IsSuccessStatusCode.Should().BeFalse();
        result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var validationProblemDetails = await ApiClientResponse.GetResponses<ValidationProblemDetails>(result.Content);
        validationProblemDetails.Errors[nameof(request.HearingTypeName)][0].Should()
            .Be("Hearing type does not exist");
    }
    
    [Test]
    public async Task should_return_validation_error_when_venue_name_is_not_found()
    {
        // arrange
        var request = CreateBookingRequestWithServiceIdsAndCodes();
        request.HearingVenueName = "doesnotexist";

        // act
        using var client = Application.CreateClient();
        var result = await client.PostAsync(ApiUriFactory.HearingsEndpoints.BookNewHearing, RequestBody.Set(request));

        // assert
        result.IsSuccessStatusCode.Should().BeFalse();
        result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var validationProblemDetails = await ApiClientResponse.GetResponses<ValidationProblemDetails>(result.Content);
        validationProblemDetails.Errors[nameof(request.HearingVenueName)][0].Should()
            .Be("Hearing venue does not exist");
    }

    private BookNewHearingRequest CreateBookingRequestWithServiceIdsAndCodes()
    {
        var hearingSchedule = DateTime.UtcNow;
        var caseName = "Bookings Api Integration Automated";
        var request = new SimpleBookNewHearingRequest(caseName, hearingSchedule).Build();
        request.CaseTypeName = "Generic";
        request.CaseTypeServiceId = "vhG1"; // intentionally incorrect case
        request.HearingTypeName = "Automated Test";
        request.HearingTypeCode = "automatedtest"; // intentionally incorrect case
        request.HearingVenueName = "Birmingham Civil and Family Justice Centre";
        request.HearingVenueCode = "231596";
        return request;
    }
}