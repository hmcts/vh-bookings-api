namespace Bookings.DAL.Queries
{
    public interface IQueryHandler
    {
        TResult Handle<TQuery, TResult>(TQuery query) where TQuery:IQuery where TResult: class;
    }

    public interface IQueryHandler<in TQuery, out TResult> where TQuery: IQuery where TResult : class 
    {
        TResult Handle(TQuery query);
    }
}