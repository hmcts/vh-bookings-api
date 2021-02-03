namespace BookingsApi.Domain.Ddd
{
    public interface IEntity<out TKey>  
    {
        TKey Id { get; }
    }
}