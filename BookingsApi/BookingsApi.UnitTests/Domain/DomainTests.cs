namespace BookingsApi.UnitTests.Domain;

public abstract class DomainTests
{
    /// <summary>
    /// Apply a small delay to ensure the UpdatedDate is different
    /// </summary>
    protected static async Task ApplyDelay()
    {
        await Task.Delay(10);
    }
}