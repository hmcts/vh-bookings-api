using System.Text.RegularExpressions;

namespace BookingsApi.DAL.Exceptions;

public abstract class BookingsDalException(string message) : Exception(message);
    
public abstract class EntityNotFoundException(string message) : BookingsDalException(message);

public abstract partial class ObfuscatedEntityNotFoundException(string message) : EntityNotFoundException(message)
{
    private const string Regex = @"(?!\b)\w";
    [GeneratedRegex(Regex)]
    private static partial Regex UsernameRegex();
    protected static string GetObfuscatedUsernameAsync(string username) => UsernameRegex().Replace(username, "*");
}