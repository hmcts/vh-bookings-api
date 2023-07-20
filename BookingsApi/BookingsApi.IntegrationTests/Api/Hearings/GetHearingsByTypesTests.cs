using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using BookingsApi.Contract.Requests;
using BookingsApi.Contract.Responses;
using BookingsApi.IntegrationTests.Helper;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using NUnit.Framework;
using Testing.Common.Builders.Api;

namespace BookingsApi.IntegrationTests.Api.Hearings;

public class GetHearingsByTypesTests : ApiTest
{
    [Test]
    public async Task should_return_bad_request_when_invalid_case_type_is_provided()
    {
        // arrange
        using var client = Application.CreateClient();
        var caseType = 99;
        var request = new GetHearingRequest { Types = new List<int> { caseType } };
        
        // act
        var result = await client.SendAsync(
            new HttpRequestMessage(HttpMethod.Get, ApiUriFactory.HearingsEndpoints.GetHearingsByTypes)
            {
                Content = RequestBody.Set(request)

            });
        

        // assert
        result.IsSuccessStatusCode.Should().BeFalse();
        result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var validationProblemDetails = await ApiClientResponse.GetResponses<ValidationProblemDetails>(result.Content);
        validationProblemDetails.Errors[nameof(request.Types)][0].Should()
            .Be("Invalid value for hearing types");
    }

    [Test]
    public async Task should_return_bad_request_when_invalid_venue_id_is_provided()
    {
        // arrange
        using var client = Application.CreateClient();
        var venueId = 99999;
        var request = new GetHearingRequest { VenueIds = new List<int> { venueId } };
        
        // act
        var result = await client.SendAsync(
            new HttpRequestMessage(HttpMethod.Get, ApiUriFactory.HearingsEndpoints.GetHearingsByTypes)
            {
                Content = RequestBody.Set(request)

            });
        

        // assert
        result.IsSuccessStatusCode.Should().BeFalse();
        result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var validationProblemDetails = await ApiClientResponse.GetResponses<ValidationProblemDetails>(result.Content);
        validationProblemDetails.Errors[nameof(request.VenueIds)][0].Should()
            .Be("Invalid value for venue ids");
    }

    [Test]
    public async Task should_get_a_paged_list_of_hearings()
    {
        // arrange
        using var client = Application.CreateClient();
        var hearing1 = await Hooks.SeedVideoHearing( configureOptions: options => { options.ScheduledDate = System.DateTime.UtcNow; });
        var hearing2 = await Hooks.SeedVideoHearing( configureOptions: options => { options.ScheduledDate = System.DateTime.UtcNow.AddMinutes(1); });
        var request = new GetHearingRequest { Limit = 1 };

        // act
        var result = await client.SendAsync(
            new HttpRequestMessage(HttpMethod.Get, ApiUriFactory.HearingsEndpoints.GetHearingsByTypes)
            {
                Content = RequestBody.Set(request)

            });

        // assert
        result.IsSuccessStatusCode.Should().BeTrue();
        result.StatusCode.Should().Be(HttpStatusCode.OK);
        var response = await ApiClientResponse.GetResponses<BookingsResponse>(result.Content);
        
        // ensure limit is applied per 'page'
        response.Hearings.Count.Should().Be(1);
        var aHearing = response.Hearings.SelectMany(x => x.Hearings).First(x => x.HearingId == hearing1.Id);

        aHearing.HearingNumber.Should().Be(hearing1.GetCases()[0].Number);
        aHearing.HearingName.Should().Be(hearing1.GetCases()[0].Name);
        
        // get second page
        request.Cursor = response.NextCursor;
        result = await client.SendAsync(
            new HttpRequestMessage(HttpMethod.Get, ApiUriFactory.HearingsEndpoints.GetHearingsByTypes)
            {
                Content = RequestBody.Set(request)

            });
        result.IsSuccessStatusCode.Should().BeTrue();
        result.StatusCode.Should().Be(HttpStatusCode.OK);
        response = await ApiClientResponse.GetResponses<BookingsResponse>(result.Content);
        
        var bHearing = response.Hearings.SelectMany(x => x.Hearings).First(x => x.HearingId == hearing2.Id);
        bHearing.HearingNumber.Should().Be(hearing2.GetCases()[0].Number);
        bHearing.HearingName.Should().Be(hearing2.GetCases()[0].Name);
    }

    [TearDown]
    public async Task TearDown()
    {
        await Hooks.ClearSeededHearings();
    }
}