using BookingsApi.Client;
using BookingsApi.Contract.V2.Requests;

namespace BookingsApi.IntegrationTests.Api.V2.Persons;

public class PostPersonBySearchTermTests : ApiTest
{
    [Test]
    public async Task should_get_person_by_partial_contact_email_match()
    {
        // arrange
        var seededHearing = await Hooks.SeedVideoHearingV2();
        var person = seededHearing.GetParticipants()[0].Person; 
        var email = person.ContactEmail;
        var request = new SearchTermRequestV2(email.Substring(0, 3).ToUpper());

        // act
        using var client = Application.CreateClient();
        var bookingsApiClient = BookingsApiClient.GetClient(client);
        var personResponse = await bookingsApiClient.SearchForPersonV2Async(request);

        // assert
        personResponse.Should().NotBeNullOrEmpty();
        personResponse.Should().Contain(x=> 
                x.ContactEmail == email && x.Id == person.Id
                && x.FirstName == person.FirstName && x.LastName == person.LastName);
    }
}