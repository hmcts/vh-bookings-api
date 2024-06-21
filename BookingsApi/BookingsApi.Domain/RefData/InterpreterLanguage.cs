namespace BookingsApi.Domain.RefData;

public sealed class InterpreterLanguage : TrackableEntity<int>
{
    public InterpreterLanguage(string code, string value, string welshValue,
        InterpreterType type, bool live)
    {
        Code = code;
        Value = value;
        WelshValue = welshValue;
        Type = type;
        Live = live;
    }
    
    public InterpreterLanguage(int id, string code, string value, string welshValue,
        InterpreterType type, bool live)
    {
        Id = id;
        Code = code;
        Value = value;
        WelshValue = welshValue;
        Type = type;
        Live = live;
    }
    public string Code { get; set; }
    public string Value { get; set; }
    public string WelshValue { get; set; }
    public InterpreterType Type { get; set; }
    public bool Live { get; set; }
    
    public void Deactivate()
    {
        Live = false;
    }

    public void UpdateLanguageDetails(string proposedLanguageValue, string proposedLanguageWelshValue,
        bool proposedLanguageLive, InterpreterType proposedLanguageType)
    {
        Value = proposedLanguageValue;
        WelshValue = proposedLanguageWelshValue;
        Live = proposedLanguageLive;
        Type = proposedLanguageType;
    }
}

public enum InterpreterType
{
    Sign = 1,
    Verbal = 2
}