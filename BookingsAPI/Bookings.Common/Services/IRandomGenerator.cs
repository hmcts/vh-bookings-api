namespace Bookings.Common.Services
{
    public interface IRandomGenerator
    {
        string GetWeakDeterministic(long ticks, uint skip, uint take);
    }
}