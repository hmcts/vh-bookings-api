using System.Linq;
using System.Text.Json;

namespace BookingsApi.Common.Helpers;

public class SnakeCaseNamingPolicy : JsonNamingPolicy
{
    public override string ConvertName(string name)
    {
        // Convert PascalCase to snake_case
        return string.Concat(
            name.Select((x, i) => i > 0 && char.IsUpper(x) ? "_" + x : x.ToString())
        ).ToLower();
    }
}