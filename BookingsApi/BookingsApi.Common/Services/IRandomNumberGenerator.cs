namespace BookingsApi.Common.Services
{
    public interface IRandomNumberGenerator
    {
        int Generate(int min, int max);
    }
}
