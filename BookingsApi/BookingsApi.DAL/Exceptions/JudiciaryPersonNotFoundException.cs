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

public class JudiciaryPersonNotFoundException : ObfuscatedEntityNotFoundException
{
    public JudiciaryPersonNotFoundException(string personalCode) :
        base($"Judiciary Person with personal code: {personalCode} does not exist")
    {
    }

    public JudiciaryPersonNotFoundException(Guid id) :
        base($"Judiciary Person with External ref id: {id} does not exist")
    {
    }
}

public class JudiciaryLeaverNotFoundException : JudiciaryLeaverException
{
    public JudiciaryLeaverNotFoundException(string username) : 
        base($"Judiciary Person with username {GetObfuscatedUsernameAsync(username)} does not exist")
    {
    }
        
    public JudiciaryLeaverNotFoundException(Guid id) : 
        base($"Judiciary Person with External ref id: {id} does not exist")
    {
    }
}
    
public class JudiciaryPersonAlreadyExistsException : JudiciaryPersonException
{
    public JudiciaryPersonAlreadyExistsException(string username) : 
        base($"Judiciary Person with username {GetObfuscatedUsernameAsync(username)} already exists")
    {
    }
        
    public JudiciaryPersonAlreadyExistsException(Guid externalRefId) : 
        base($"Judiciary Person with ExternalRefId {externalRefId} already exists")
    {
    }
}