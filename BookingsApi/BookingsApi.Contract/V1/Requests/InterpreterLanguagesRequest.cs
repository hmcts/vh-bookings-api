using BookingsApi.Contract.V1.Enums;

namespace BookingsApi.Contract.V1.Requests;

public class InterpreterLanguagesRequest
{
    /// <summary>
    /// The code or key for the language
    /// </summary>
    public string Code { get; set; }
    
    /// <summary>
    /// The text value of the language in English
    /// </summary>
    public string Value { get; set; }
    
    /// <summary>
    /// The text value of the language in Welsh
    /// </summary>
    public string WelshValue { get; set; }
    
    /// <summary>
    /// The type (spoken or sign) of the interpreter
    /// </summary>
    public InterpreterType Type { get; set; }
    
    /// <summary>
    /// Is the language active
    /// </summary>
    public bool Live { get; set; }
}