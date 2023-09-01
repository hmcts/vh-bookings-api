using BookingsApi.Contract.V1.Responses;

namespace BookingsApi.IntegrationTests.Api.V1.Persons;

public class GetPersonByContactEmailTests : ApiTest
{
    [Test]
    public async Task should_return_bad_request_when_an_invalid_email_is_provided()
    {
        // arrange
        var contactEmail = "modshfod";

        // act
        using var client = Application.CreateClient();
        var result = await client.GetAsync(ApiUriFactory.PersonEndpoints.GetPersonByContactEmail(contactEmail));

        // assert
        result.IsSuccessStatusCode.Should().BeFalse();
        result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var validationProblemDetails = await ApiClientResponse.GetResponses<ValidationProblemDetails>(result.Content);
        validationProblemDetails.Errors["contactEmail"][0].Should().Be("Please provide a valid contactEmail");
    }

    [Test]
    public async Task should_return_not_found_when_person_does_not_exist()
    {
        // arrange
        var contactEmail = "random@test.com";
        
        // act
        using var client = Application.CreateClient();
        var result = await client.GetAsync(ApiUriFactory.PersonEndpoints.GetPersonByContactEmail(contactEmail));

        // assert
        result.IsSuccessStatusCode.Should().BeFalse();
        result.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Test]
    public async Task should_return_person_when_found()
    {
        // arrange
        var hearing = await Hooks.SeedVideoHearing();
        var person = hearing.GetParticipants()[0].Person;
        var contactEmail = person.ContactEmail;
        
        // act
        using var client = Application.CreateClient();
        var result = await client.GetAsync(ApiUriFactory.PersonEndpoints.GetPersonByContactEmail(contactEmail));

        // assert
        result.IsSuccessStatusCode.Should().BeTrue();
        result.StatusCode.Should().Be(HttpStatusCode.OK);
        var personResponse = await ApiClientResponse.GetResponses<PersonResponse>(result.Content);
        personResponse.Id.Should().Be(person.Id);
        personResponse.Username.Should().Be(person.Username);
        personResponse.ContactEmail.Should().Be(person.ContactEmail);
        personResponse.TelephoneNumber.Should().Be(person.TelephoneNumber);
        personResponse.FirstName.Should().Be(person.FirstName);
        personResponse.LastName.Should().Be(person.LastName);
        personResponse.MiddleNames.Should().Be(person.MiddleNames);
        personResponse.Title.Should().Be(person.Title);
        personResponse.Organisation.Should().Be(person.Organisation?.Name);
    }
}