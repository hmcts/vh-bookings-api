using BookingsApi.Client;
using BookingsApi.Contract.V2.Enums;

namespace BookingsApi.IntegrationTests.Api.V2.InterpreterLanguages;

public class GetAvailableInterpreterLanguageTests : ApiTest
{
    [Test]
    public async Task should_return_available_languages()
    {
        // arrange
        using var client = Application.CreateClient();
        var bookingsApiClient = BookingsApiClient.GetClient(client);

        // act
        var response = await bookingsApiClient.GetAvailableInterpreterLanguagesAsync();

        // assert
        response.Should().NotBeEmpty("No interpreter languages were returned, did you run ref data migrations?");
        response.Where(x => x.Type == InterpreterType.Sign).Should()
            .NotBeEmpty("No sign interpreter languages were returned, did you run ref data migrations?");
        response.Where(x => x.Type == InterpreterType.Verbal).Should()
            .NotBeEmpty("No verbale interpreter languages were returned, did you run ref data migrations?");
    }
}