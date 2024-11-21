using BookingsApi.Client;

namespace BookingsApi.IntegrationTests.Api.V1.Persons;

public class AnonymisePersonWithUsernameTests : ApiTest
{
    [Test]
    public async Task should_return_not_found_when_person_with_username_does_not_exist()
    {
        // arrange
        var username = "does@not-exist.com";
        
        // act
        using var client = Application.CreateClient();
        var bookingsApiClient = BookingsApiClient.GetClient(client);
       
        var act = async () => await bookingsApiClient.AnonymisePersonWithUsernameAsync(username);
        
        // assert
        var exception = await act.Should().ThrowAsync<BookingsApiException>();
        exception.Which.StatusCode.Should().Be((int)HttpStatusCode.NotFound);
        exception.Which.Response.Should().MatchRegex("^\"Person with username.*does not exist\"$");
    }
    
    [Test]
    public async Task should_anonymise_person_with_username()
    {
        // arrange
        var person = await Hooks.SeedPerson();
        
        // act
        using var client = Application.CreateClient();
        var bookingsApiClient = BookingsApiClient.GetClient(client);
        await bookingsApiClient.AnonymisePersonWithUsernameAsync(person.Username);
        
        // assert
        await using var db = new BookingsDbContext(BookingsDbContextOptions);
        var updatedPerson = await db.Persons.FindAsync(person.Id);

        updatedPerson!.FirstName.Should().NotBeEquivalentTo(person.FirstName);
        updatedPerson.LastName.Should().NotBeEquivalentTo(person.LastName);
        updatedPerson.Username.Should().NotBeEquivalentTo(person.Username);
        updatedPerson.ContactEmail.Should().NotBeEquivalentTo(person.ContactEmail);
        
        updatedPerson.Username.Should().EndWith("@hmcts.net");
        updatedPerson.ContactEmail.Should().EndWith("@hmcts.net");
    }
}