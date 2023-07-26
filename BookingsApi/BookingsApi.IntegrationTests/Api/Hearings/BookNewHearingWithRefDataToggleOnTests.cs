using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using BookingsApi.Common.Services;
using BookingsApi.Contract.Requests;
using BookingsApi.Contract.Responses;
using BookingsApi.IntegrationTests.Helper;
using BookingsApi.Validations;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using NUnit.Framework;
using Testing.Common.Builders.Api;
using Testing.Common.Builders.Api.Request;
using Testing.Common.Stubs;

namespace BookingsApi.IntegrationTests.Api.Hearings;

public class BookNewHearingWithRefDataToggleOnTests : ApiTest
{
    private FeatureTogglesStub _featureToggleStub;
    private readonly List<Guid> _hearingIds = new();

    [SetUp]
    public void Setup()
    {
        _hearingIds.Clear();
        _featureToggleStub = Application.Services.GetService(typeof(IFeatureToggles)) as FeatureTogglesStub;
        _featureToggleStub!.RefData = true;
    }

    [TearDown]
    public async Task TearDown()
    {
        foreach (var hearingId in _hearingIds)
        {
            await Hooks.RemoveVideoHearing(hearingId);
        }
    }

    [Test]
    public async Task should_book_a_hearing_with_codes_instead_of_names()
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
        request.HearingVenueCode = null;
        request.CaseTypeServiceId = null;
        request.HearingTypeCode = null;

        // act
        using var client = Application.CreateClient();
        var result = await client.PostAsync(ApiUriFactory.HearingsEndpoints.BookNewHearing, RequestBody.Set(request));

        // assert
        result.IsSuccessStatusCode.Should().BeFalse();
        result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var validationProblemDetails = await ApiClientResponse.GetResponses<ValidationProblemDetails>(result.Content);
        validationProblemDetails.Errors[nameof(request.HearingVenueCode)][0].Should()
            .Be(BookNewHearingRequestValidation.HearingVenueCodeErrorMessage);
        
        validationProblemDetails.Errors[nameof(request.CaseTypeServiceId)][0].Should()
            .Be(BookNewHearingRequestValidation.CaseTypeServiceIdErrorMessage);
        
        validationProblemDetails.Errors[nameof(request.HearingTypeCode)][0].Should()
            .Be(BookNewHearingRequestValidation.HearingTypeCodeErrorMessage);
    }
    
    [Test]
    public async Task should_return_validation_error_when_case_type_service_id_is_not_found()
    {
        // arrange
        var request = CreateBookingRequestWithServiceIdsAndCodes();
        request.CaseTypeServiceId = "999299292929";

        // act
        using var client = Application.CreateClient();
        var result = await client.PostAsync(ApiUriFactory.HearingsEndpoints.BookNewHearing, RequestBody.Set(request));

        // assert
        result.IsSuccessStatusCode.Should().BeFalse();
        result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var validationProblemDetails = await ApiClientResponse.GetResponses<ValidationProblemDetails>(result.Content);
        validationProblemDetails.Errors[nameof(request.CaseTypeServiceId)][0].Should()
            .Be("Case type does not exist");
    }
    
    [Test]
    public async Task should_return_validation_error_when_hearing_type_code_is_not_found()
    {
        // arrange
        var request = CreateBookingRequestWithServiceIdsAndCodes();
        request.HearingTypeCode = "999299292929";

        // act
        using var client = Application.CreateClient();
        var result = await client.PostAsync(ApiUriFactory.HearingsEndpoints.BookNewHearing, RequestBody.Set(request));

        // assert
        result.IsSuccessStatusCode.Should().BeFalse();
        result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var validationProblemDetails = await ApiClientResponse.GetResponses<ValidationProblemDetails>(result.Content);
        validationProblemDetails.Errors[nameof(request.HearingTypeCode)][0].Should()
            .Be("Hearing type does not exist");
    }
    
    [Test]
    public async Task should_return_validation_error_when_venue_code_is_not_found()
    {
        // arrange
        var request = CreateBookingRequestWithServiceIdsAndCodes();
        request.HearingVenueCode = "999299292929";

        // act
        using var client = Application.CreateClient();
        var result = await client.PostAsync(ApiUriFactory.HearingsEndpoints.BookNewHearing, RequestBody.Set(request));

        // assert
        result.IsSuccessStatusCode.Should().BeFalse();
        result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var validationProblemDetails = await ApiClientResponse.GetResponses<ValidationProblemDetails>(result.Content);
        validationProblemDetails.Errors[nameof(request.HearingVenueCode)][0].Should()
            .Be("Hearing venue does not exist");
    }

    private BookNewHearingRequest CreateBookingRequestWithServiceIdsAndCodes()
    {
        var hearingSchedule = DateTime.UtcNow;
        var caseName = "Bookings Api Integration Automated";
        var request = new SimpleBookNewHearingRequest(caseName, hearingSchedule).Build();
        request.CaseTypeName = "Generic";
        request.CaseTypeServiceId = "VHG1";
        request.HearingTypeName = "Automated Test";
        request.HearingTypeCode = "AutomatedTest";
        request.HearingVenueName = "Birmingham Civil and Family Justice Centre";
        request.HearingVenueCode = "231596";
        return request;
    }
}