using BookingsApi.Contract.V1.Enums;

namespace BookingsApi.Contract.V1.Responses;

public class InterpreterLanguagesResponse
{
    public string Code { get; set; }
    public string Value { get; set; }
    public string WelshValue { get; set; }
    public InterpreterType Type { get; set; }
    public bool Live { get; set; }
}