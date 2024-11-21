using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BookingsApi.Contract.V2.Requests;
using BookingsApi.Contract.V2.Responses;
using FluentAssertions;
using Testing.Common.Builders.Api.V2;

namespace BookingsApi.AcceptanceTests.Api.V2.Persons;

public class PersonTests : ApiTest
{
    private readonly List<HearingDetailsResponseV2> _hearings = new();
    
    [OneTimeTearDown]
    public async Task TearDown()
    {
        foreach (var hearing in _hearings)
            await BookingsApiClient.RemoveHearingAsync(hearing.Id);
        
        TestContext.WriteLine("Removed hearing");
    }
    
    
    [Test]
    public async Task GetPersonBySearchTerm()
    {
        // arrange
        var hearing = await BookHearing();
        _hearings.Add(hearing);
        var participant = hearing.Participants.Find(x=> x.UserRoleName == "Individual");

        // act
        var result = await BookingsApiClient.SearchForPersonV2Async(new SearchTermRequestV2(participant.ContactEmail[..3]));

        // assert
        result.Should().NotBeNullOrEmpty();

        var searchedForParticipant = result.First(x => x.ContactEmail == participant.ContactEmail);
        searchedForParticipant.Should().NotBeNull();
        searchedForParticipant.Should().BeEquivalentTo(participant, options => options.ExcludingMissingMembers());
    }

    private Task<HearingDetailsResponseV2> BookHearing()
    {
        var hearingSchedule = DateTime.UtcNow.AddMinutes(5);
        var caseName = "Bookings Api AC Automated";
        var request =
            new SimpleBookNewHearingRequestV2(caseName, hearingSchedule,
                SimpleBookNewHearingRequestV2.JudgePersonalCode).Build();
        return BookingsApiClient.BookNewHearingWithCodeAsync(request);
    }

}