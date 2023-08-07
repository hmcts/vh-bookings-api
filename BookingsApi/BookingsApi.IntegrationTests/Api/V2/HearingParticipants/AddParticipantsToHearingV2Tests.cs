using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using BookingsApi.Contract.V2.Requests;
using BookingsApi.Domain;
using BookingsApi.Domain.Enumerations;
using BookingsApi.IntegrationTests.Helper;
using FluentAssertions;
using NUnit.Framework;
using Testing.Common.Builders.Api;

namespace BookingsApi.IntegrationTests.Api.V2.HearingParticipants;

public class AddParticipantsToHearingV2Tests : ApiTest
{
    private readonly List<Guid> _hearingIds = new();
    
    [SetUp]
    public void Setup() => _hearingIds.Clear();
    
        
    [TearDown]
    public new async Task TearDown() => _hearingIds.Select(async id => await Hooks.RemoveVideoHearing(id));
    
    
    [Test]
    public async Task should_add_participant_to_hearing_and_return_200()
    {
        // arrange
        var hearing = await Hooks.SeedVideoHearing(options =>
        {
            options.Case = new Case("Case1 Num", "Case1 Name");
        },false, BookingStatus.Created);
        _hearingIds.Add(hearing.Id);
        var request = BuildRequestObject();

        // act
        using var client = Application.CreateClient();
        var result = await client
            .PostAsync(ApiUriFactory.HearingParticipantsEndpointsV2.AddParticipantsToHearing(_hearingIds[0]),RequestBody.Set(request));

        // assert
        result.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    private static AddParticipantsToHearingRequestV2 BuildRequestObject()
    {
        var request = new AddParticipantsToHearingRequestV2
        {
            Participants = new List<ParticipantRequestV2>
            {
                new ()
                {
                    Username = "username@test.email.com",
                    CaseRoleName = "Applicant",
                    DisplayName = "DisplayName",
                    FirstName = "FirstName",
                    HearingRoleName = "Applicant LIP",
                    LastName = "LastName",
                    MiddleNames = "MiddleNames",
                    OrganisationName = "OrganisationName",
                    ContactEmail = "contact@test.email.com",
                    TelephoneNumber = "0123456789",
                    Title = "Title",
                }
            }
        };
        return request;
    }
}