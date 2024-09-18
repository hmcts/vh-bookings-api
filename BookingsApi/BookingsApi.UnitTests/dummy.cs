using System.Text.Json;
using BookingsApi.Common.Helpers;
using BookingsApi.Contract.V1.Enums;
using BookingsApi.Contract.V1.Responses;

namespace BookingsApi.UnitTests;

[TestFixture]
public class dummy
{
    [Test]
    public void serialize()
    {
        var language = new InterpreterLanguagesResponse
        {
            Code = "en",
            Value = "English",
            WelshValue = "Saesneg",
            Type = InterpreterType.Verbal,
            Live = true
        };

        var response =
            JsonSerializer.Serialize(language, DefaultSerializerSettings.DefaultSystemTextJsonSerializerSettings());
        
        Console.WriteLine(response);

        response.Should().NotBeEmpty();
    }
}