using System.Linq;
using System.Text.Json;

namespace BookingsApi.Common.Helpers;

public class SnakeCaseNamingPolicy : JsonNamingPolicy
{
    /// <summary>
    /// The function converts a PascalCase string to snake_case in C#.
    /// </summary>
    /// <param name="name">The `ConvertName` method you provided converts a PascalCase string to
    /// snake_case. If you provide me with a specific `name` in PascalCase that you would like to
    /// convert, I can demonstrate how the method works with that input.</param>
    /// <returns>
    /// The method is converting a PascalCase string to snake_case. It is inserting an underscore before
    /// each uppercase letter (except the first letter) and then converting the whole string to
    /// lowercase.
    /// </returns>
    public override string ConvertName(string name)
    {
        // Convert PascalCase to snake_case
        return string.Concat(
            name.Select((x, i) => i > 0 && char.IsUpper(x) ? "_" + x : x.ToString())
        ).ToLower();
    }
}