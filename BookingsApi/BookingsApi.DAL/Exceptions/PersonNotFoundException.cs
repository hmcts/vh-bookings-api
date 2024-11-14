using System.Text.RegularExpressions;

#pragma warning disable S3925 // "ISerializable" should be implemented correctly
namespace BookingsApi.DAL.Exceptions;

public abstract partial class PersonException(string message) : Exception(message)
{
    private const string Regex = @"(?!\b)\w";
    [GeneratedRegex(Regex)]
    private static partial Regex UsernameRegex();
    protected static string GetObfuscatedUsernameAsync(string username)
    {
        var obfuscatedUsername = UsernameRegex().Replace(username, "*");
        return obfuscatedUsername;
    }
}

public class PersonNotFoundException : ObfuscatedEntityNotFoundException
{
    public PersonNotFoundException(string username) : base(
        $"Person with username {GetObfuscatedUsernameAsync(username)} does not exist")
    {
    }
        
    public PersonNotFoundException(Guid personId) : base(
        $"Person with id {personId} does not exist")
    {
    }
}

public class PersonIsAJudgeException(string username)
    : PersonException($"Person with username {GetObfuscatedUsernameAsync(username)} is a judge");