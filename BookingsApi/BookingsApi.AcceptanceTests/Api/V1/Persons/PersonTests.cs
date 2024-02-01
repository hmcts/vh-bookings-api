using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BookingsApi.Contract.V1.Requests;
using BookingsApi.Contract.V1.Responses;
using FluentAssertions;
using NUnit.Framework;
using Testing.Common.Builders.Api.V1.Request;

namespace BookingsApi.AcceptanceTests.Api.V1.Persons;

public class PersonTests : ApiTest
{
    private List<HearingDetailsResponse> _hearings = new();
    
    [OneTimeTearDown]
    public async Task TearDown()
    {
        foreach (var hearing in _hearings)
            await BookingsApiClient.RemoveHearingAsync(hearing.Id);
        
        TestContext.WriteLine("Removed hearing");
    }

    [Test]
    public async Task GetPersonByContactEmail()
    {
        // arrange
        var hearing = await BookHearing();
        _hearings.Add(hearing);
        var participant = hearing.Participants[0];

        // act
        var result = await BookingsApiClient.GetPersonByContactEmailAsync(participant.ContactEmail);

        // assert
        result.Should().NotBeNull();
        result.Should().BeEquivalentTo(participant, options => options.ExcludingMissingMembers().Excluding(p => p.Id));
    }
    
    [Test]
    public async Task GetPersonByUsernameEmail()
    {
        // arrange
        var hearing = await BookHearing();
        _hearings.Add(hearing);
        var participant = hearing.Participants.Find(x=> x.Username is not null);

        // act
        var result = await BookingsApiClient.GetPersonByUsernameAsync(participant.Username);

        // assert
        result.Should().BeEquivalentTo(participant, options => options.ExcludingMissingMembers().Excluding(p => p.Id));
    }
    
    [Test]
    public async Task GetPersonBySearchTerm()
    {
        // arrange
        var hearing = await BookHearing();
        _hearings.Add(hearing);
        var participant = hearing.Participants.Find(x=> x.UserRoleName == "Individual");

        // act
        var result = await BookingsApiClient.PostPersonBySearchTermAsync(new SearchTermRequest(participant.ContactEmail[..3]));

        // assert
        result.Should().NotBeNullOrEmpty();
        PersonResponse searchedForParticipant = null;

        searchedForParticipant = result.FirstOrDefault(x => x.ContactEmail == participant.ContactEmail);
        searchedForParticipant.Should().NotBeNull();    
        searchedForParticipant.Should().BeEquivalentTo(participant, 
            options => options.ExcludingMissingMembers()
                                    .Excluding(p => p.Id));
    }

    private Task<HearingDetailsResponse> BookHearing()
    {
        var hearingSchedule = DateTime.UtcNow.AddMinutes(5);
        var caseName = "Bookings Api AC Automated";
        var request = new SimpleBookNewHearingRequest(caseName, hearingSchedule).Build();
        return BookingsApiClient.BookNewHearingAsync(request);
    }

}