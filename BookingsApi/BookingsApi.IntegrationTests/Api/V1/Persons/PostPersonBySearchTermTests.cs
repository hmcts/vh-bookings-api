using BookingsApi.Contract.V1.Requests;
using BookingsApi.Contract.V1.Responses;

namespace BookingsApi.IntegrationTests.Api.V1.Persons;

public class PostPersonBySearchTermTests : ApiTest
{
    [Test]
    public async Task should_get_person_by_partial_contact_email_match()
    {
        // arrange
        var seededHearing = await Hooks.SeedVideoHearing();
        var person = seededHearing.GetParticipants()[0].Person; 
        var email = person.ContactEmail;
        var request = new SearchTermRequest(email.Substring(0, 3).ToUpper());

        // act
        using var client = Application.CreateClient();
        var result = await client.PostAsync(ApiUriFactory.PersonEndpoints.PostPersonBySearchTerm, RequestBody.Set(request));

        // assert
        result.IsSuccessStatusCode.Should().BeTrue();
        result.StatusCode.Should().Be(HttpStatusCode.OK);
        var personResponse = await ApiClientResponse.GetResponses<List<PersonResponse>>(result.Content);
        personResponse.Should().NotBeNullOrEmpty();
        personResponse.Exists(x=> 
                x.ContactEmail == email && x.Id == person.Id
                && x.FirstName == person.FirstName && x.LastName == person.LastName)
            .Should().BeTrue();
    }
}