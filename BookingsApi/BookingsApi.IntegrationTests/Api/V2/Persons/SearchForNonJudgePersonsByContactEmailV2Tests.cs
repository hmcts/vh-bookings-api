using BookingsApi.Client;

namespace BookingsApi.IntegrationTests.Api.V2.Persons;

public class SearchForNonJudgePersonsByContactEmailV2Tests : ApiTest
{
    [Test]
    public async Task should_return_person_with_matching_contact_email()
    {
        // arrange
        var person = await Hooks.SeedPerson();
        
        // act
        var client = Application.CreateClient();
        var bookingsApiClient = BookingsApiClient.GetClient(client);
        var result = await bookingsApiClient.SearchForNonJudgePersonsByContactEmailV2Async(person.ContactEmail);
        
        // assert
        result.Should().NotBeNull();
        result.ContactEmail.Should().Be(person.ContactEmail);
        result.Id.Should().Be(person.Id);
    }

    [Test]
    public async Task should_return_not_found_when_contact_email_does_not_exist()
    {
        // arrange
        var client = Application.CreateClient();
        var bookingsApiClient = BookingsApiClient.GetClient(client);

        // act
        Func<Task> act = async () =>
            await bookingsApiClient.SearchForNonJudgePersonsByContactEmailV2Async("madeup@test.com");

        // assert
        var exception = await act.Should().ThrowAsync<BookingsApiException>();
        exception.Which.StatusCode.Should().Be((int)HttpStatusCode.NotFound);
        exception.Which.Response.Should().MatchRegex("^\"Person with madeup@test.com does not exist\"$");
    }
    
    [Test]
    public async Task should_return_validation_problem_when_input_is_not_a_valid_email()
    {
        // arrange
        var client = Application.CreateClient();
        var bookingsApiClient = BookingsApiClient.GetClient(client);
    
        // act
        Func<Task> act = async () =>
            await bookingsApiClient.SearchForNonJudgePersonsByContactEmailV2Async("madeup");
    
        // assert
        var exception = await act.Should().ThrowAsync<BookingsApiException>();
        exception.And.StatusCode.Should().Be((int)HttpStatusCode.BadRequest);
        var bookingsApiException = exception.And.As<BookingsApiException<ValidationProblemDetails>>();
        var errors = bookingsApiException.Result.Errors;
        errors.Should().ContainKey("contactEmail");
        errors["contactEmail"].Should().Contain("Please provide a valid contactEmail");
    }
}