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

public class UpdateHearingParticipantsV2Tests : ApiTest
{
    private readonly List<Guid> _hearingIds = new();
    
    [SetUp]
    public void Setup() => _hearingIds.Clear();
    
        
    [TearDown]
    public new void TearDown() => _hearingIds.Select(async id => await Hooks.RemoveVideoHearing(id));
    
    
    [Test]
    public async Task should_update_participant_in_a_hearing_and_return_200()
    {
        // arrange
        var hearing = await Hooks.SeedVideoHearing(options =>
        {
            options.Case = new Case("Case1 Num", "Case1 Name");
        },false, BookingStatus.Created);
        _hearingIds.Add(hearing.Id);

        var participantToUpdate = hearing.Participants[0];
        var request = new UpdateHearingParticipantsRequestV2
        {
            ExistingParticipants = new List<UpdateParticipantRequestV2>()
            {
                new ()
                {
                    ParticipantId = participantToUpdate.Id,
                    DisplayName = participantToUpdate.DisplayName,
                    Title = participantToUpdate.Person.Title
                }
            } 
        };
        // act
        using var client = Application.CreateClient();
        var result = await client
            .PostAsync(ApiUriFactory.HearingParticipantsEndpointsV2.UpdateHearingParticipants(_hearingIds[0]),RequestBody.Set(request));

        // assert
        result.StatusCode.Should().Be(HttpStatusCode.OK);
    }

}