using BookingsApi.Contract.V2.Enums;

namespace BookingsApi.Contract.V2.Responses;

/// <summary>
/// The response object for interpreter languages
/// </summary>
public class InterpreterLanguagesResponse
{
    /// <summary>
    /// The language code
    /// </summary>
    public string Code { get; set; }
    
    /// <summary>
    /// The plain text description in English
    /// </summary>
    public string Value { get; set; }
    
    /// <summary>
    /// The plain text description in Welsh
    /// </summary>
    public string WelshValue { get; set; }
    
    /// <summary>
    /// The type of interpreter (sign or verbal)
    /// </summary>
    public InterpreterType Type { get; set; }
    
    /// <summary>
    /// Is the language actively used
    /// </summary>
    public bool Live { get; set; }
}