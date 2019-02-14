namespace Bookings.DAL.Queries
{
    public class QueryHandler : IQueryHandler
    {
        private readonly IQueryHandlerFactory _queryHandlerFactory;

        public QueryHandler(IQueryHandlerFactory queryHandlerFactory)
        {
            _queryHandlerFactory = queryHandlerFactory;
        }

        public TResult Handle<TQuery, TResult>(TQuery query) where TQuery : IQuery where TResult : class
        {
            var handler = _queryHandlerFactory.Create<TQuery, TResult>(query);
            return handler.Handle(query);
        }
    }
}