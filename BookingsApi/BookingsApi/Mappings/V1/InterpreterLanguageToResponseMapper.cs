using BookingsApi.Contract.V1.Responses;
using ContractInterpreterType = BookingsApi.Contract.V1.Enums.InterpreterType;

namespace BookingsApi.Mappings.V1;

public static class InterpreterLanguageToResponseMapper
{
    public static InterpreterLanguagesResponse MapInterpreterLanguageToResponse(
        InterpreterLanguage interpreterLanguage)
    {
        return new InterpreterLanguagesResponse
        {
            Code = interpreterLanguage.Code,
            Value = interpreterLanguage.Value,
            WelshValue = interpreterLanguage.WelshValue,
            Type = MapInterpreterType(interpreterLanguage.Type),
            Live = interpreterLanguage.Live
        };
    }

    private static ContractInterpreterType MapInterpreterType(InterpreterType interpreterLanguageType)
    {
        return interpreterLanguageType switch
        {
            InterpreterType.Sign => ContractInterpreterType.Sign,
            InterpreterType.Verbal => ContractInterpreterType.Verbal,
            _ => throw new ArgumentOutOfRangeException(nameof(interpreterLanguageType), interpreterLanguageType, null)
        };
    }
}