namespace BookingsApi.DAL.Exceptions;

public abstract partial class JudiciaryPersonException(string message) : Exception(message)
{
    private const string Regex = @"(?!\b)\w";
    [System.Text.RegularExpressions.GeneratedRegex(Regex)]
    private static partial System.Text.RegularExpressions.Regex UsernameRegex();
    protected static string GetObfuscatedUsernameAsync(string username)
    {
        return UsernameRegex().Replace(username, "*");
    }

}

public abstract partial class JudiciaryLeaverException(string message) : Exception(message)
{
    private const string Regex = @"(?!\b)\w";
    [System.Text.RegularExpressions.GeneratedRegex(Regex)]
    private static partial System.Text.RegularExpressions.Regex UsernameRegex();
    protected static string GetObfuscatedUsernameAsync(string username)
    {
        return UsernameRegex().Replace(username, "*");
    }
}

public class JudiciaryPersonNotFoundException(string personalCode)
    : ObfuscatedEntityNotFoundException($"Judiciary Person with personal code: {personalCode} does not exist");

public class JudiciaryLeaverNotFoundException(string username)
    : JudiciaryLeaverException($"Judiciary Person with username {GetObfuscatedUsernameAsync(username)} does not exist");
