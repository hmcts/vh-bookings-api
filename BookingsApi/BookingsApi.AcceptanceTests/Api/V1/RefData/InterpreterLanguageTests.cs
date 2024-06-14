using System.Linq;
using System.Threading.Tasks;
using BookingsApi.Contract.V1.Enums;
using FluentAssertions;

namespace BookingsApi.AcceptanceTests.Api.V1.RefData;

public class InterpreterLanguageTests : ApiTest
{
    [Test(Description = "Used to verify ref data has been run and languages are available")]
    public async Task should_return_languages()
    {
        // arrange / act
        var allLanguages = await BookingsApiClient.GetAvailableInterpreterLanguagesAsync();

        // assert
        allLanguages.Should().NotBeNullOrEmpty("No interpreter languages were returned, did you run ref data migrations?");
        allLanguages.Should().AllSatisfy(language =>
        {
            language.Code.Should().NotBeNullOrWhiteSpace();
            language.Value.Should().NotBeNullOrWhiteSpace();
        });
        
        allLanguages.Where(x => x.Type == InterpreterType.Sign).Should()
            .NotBeEmpty("No sign interpreter languages were returned, did you run ref data migrations?");
        
        allLanguages.Where(x => x.Type == InterpreterType.Verbal).Should()
            .NotBeEmpty("No sign interpreter languages were returned, did you run ref data migrations?");
            
    }
}