using System;
using System.Net;
using System.Threading.Tasks;
using BookingsApi.Contract.V1.Requests;
using BookingsApi.Domain.Enumerations;
using BookingsApi.IntegrationTests.Helper;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using NUnit.Framework;
using Testing.Common.Builders.Api;

namespace BookingsApi.IntegrationTests.Api.V1.CancelBooking;

public class CancelBookingTests : ApiTest
{
    [Test]
    public async Task should_return_badrequest_when_cancelling_with_invalid_hearingid()
    {
        using var client = Application.CreateClient();

        var result = await client.PatchAsync(ApiUriFactory.HearingsEndpoints.CancelBookingUri(Guid.Empty), RequestBody.Set(
            new CancelBookingRequest
            {
                UpdatedBy = "Test",
                CancelReason = "Cancelled test"
            }));

        result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var validationProblemDetails = await ApiClientResponse.GetResponses<ValidationProblemDetails>(result.Content);
        validationProblemDetails.Errors.Count.Should().Be(1);
    }

    [Test]
    public async Task should_return_badrequest_when_cancelling_with_invalid_request()
    {
        using var client = Application.CreateClient();

        var result = await client.PatchAsync(ApiUriFactory.HearingsEndpoints.CancelBookingUri(Guid.NewGuid()), RequestBody.Set(
            new CancelBookingRequest
            {
                UpdatedBy = "Test",
            }));

        result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var validationProblemDetails = await ApiClientResponse.GetResponses<ValidationProblemDetails>(result.Content);
        validationProblemDetails.Errors.Count.Should().Be(1);
    }

    [Test]
    public async Task should_return_notfound_when_cancelling_with_nonexistent_hearing()
    {
        using var client = Application.CreateClient();

        var result = await client.PatchAsync(ApiUriFactory.HearingsEndpoints.CancelBookingUri(Guid.NewGuid()), RequestBody.Set(
            new CancelBookingRequest
            {
                UpdatedBy = "Test",
                CancelReason = "Cancelled test"
            }));

        result.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [TestCase(BookingStatus.Booked)]
    [TestCase(BookingStatus.Created)]
    public async Task should_cancel_a_hearing(BookingStatus status)
    {
        var seededHearing = await Hooks.SeedVideoHearing(status: status, configureOptions: options =>
        {
            options.ScheduledDate = DateTime.UtcNow;
        });
        
        var hearingId = seededHearing.Id;

        using var client = Application.CreateClient();

        var result = await client.PatchAsync(ApiUriFactory.HearingsEndpoints.CancelBookingUri(hearingId), RequestBody.Set(
            new CancelBookingRequest 
            { 
                UpdatedBy = "Test",
                CancelReason = "Cancelled test"
            }));

        result.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Test]
    public async Task should_fail_cancelling_failed_hearing()
    {
        var seededHearing = await Hooks.SeedVideoHearing(status: BookingStatus.Failed, configureOptions: options =>
        {
            options.ScheduledDate = DateTime.UtcNow;
        });

        var hearingId = seededHearing.Id;

        using var client = Application.CreateClient();

        var result = await client.PatchAsync(ApiUriFactory.HearingsEndpoints.CancelBookingUri(hearingId), RequestBody.Set(
            new CancelBookingRequest
            {
                UpdatedBy = "Test",
                CancelReason = "Cancelled test"
            }));

        result.StatusCode.Should().Be(HttpStatusCode.Conflict);
        var serializableError = await ApiClientResponse.GetResponses<SerializableError>(result.Content);
        serializableError.ContainsKey("BookingStatus").Should().BeTrue();
        JsonConvert.DeserializeObject<string[]>(serializableError["BookingStatus"].ToString()!).Should().Contain("Cannot change the booking status from Failed to Cancelled");
    }
    
    [TearDown]
    public async Task TearDown()
    {
        await Hooks.ClearSeededHearings();
    }
}