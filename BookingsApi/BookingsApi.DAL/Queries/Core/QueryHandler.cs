﻿namespace BookingsApi.DAL.Queries.Core
{
    public class QueryHandler : IQueryHandler
    {
        private readonly IQueryHandlerFactory _queryHandlerFactory;

        public QueryHandler(IQueryHandlerFactory queryHandlerFactory)
        {
            _queryHandlerFactory = queryHandlerFactory;
        }

        public async Task<TResult> Handle<TQuery, TResult>(TQuery query) where TQuery : IQuery where TResult : class
        {
            var handler = _queryHandlerFactory.Create<TQuery, TResult>(query);
            return await handler.Handle(query);
        }
    }
}