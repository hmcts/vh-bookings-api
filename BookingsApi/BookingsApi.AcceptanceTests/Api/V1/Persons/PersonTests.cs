using System;
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
    private HearingDetailsResponse _hearing;
    
    [TearDown]
    public async Task TearDown()
    {
        if (_hearing == null) return;
        await BookingsApiClient.RemoveHearingAsync(_hearing.Id);
        TestContext.WriteLine("Removed hearing");
    }

    [Test]
    public async Task GetPersonByContactEmail()
    {
        // arrange
        await BookHearing();
        var participant = _hearing.Participants[0];

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
        await BookHearing();
        var participant = _hearing.Participants[0];

        // act
        var result = await BookingsApiClient.GetPersonByUsernameAsync(participant.Username);

        // assert
        result.Should().BeEquivalentTo(participant, options => options.ExcludingMissingMembers().Excluding(p => p.Id));
    }
    
    [Test]
    public async Task GetPersonBySearchTerm()
    {
        // arrange
        await BookHearing();
        var participant = _hearing.Participants[0];

        // act
        var result = await BookingsApiClient.PostPersonBySearchTermAsync(new SearchTermRequest(participant.Username[..5]));

        // assert
        result.Should().NotBeNullOrEmpty();
        result.First(x => x.Username == participant.Username)
            .Should().BeEquivalentTo(participant, options => options.ExcludingMissingMembers().Excluding(p => p.Id));
    }

    private async Task BookHearing()
    {
        var hearingSchedule = DateTime.UtcNow.AddMinutes(5);
        var caseName = "Bookings Api AC Automated";
        var request = new SimpleBookNewHearingRequest(caseName, hearingSchedule).Build();
        _hearing = await BookingsApiClient.BookNewHearingAsync(request);
    }

}