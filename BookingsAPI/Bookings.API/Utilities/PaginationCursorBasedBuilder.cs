using Bookings.Api.Contract.Responses;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Bookings.API.Utilities
{
    /// <summary>
    /// Builder to add cursor based pagination to a list of domain objects
    /// </summary>
    /// <typeparam name="TResponseType">The final response object to create</typeparam>
    /// <typeparam name="TModelType">The model that will be paged</typeparam>
    public class PaginationCursorBasedBuilder<TResponseType, TModelType> where TResponseType : PagedCursorBasedResponse
    {
        private readonly Func<List<TModelType>, TResponseType> _factory;
        private IQueryable<TModelType> _items;
        private string _cursor;
        private int _limit = 100;
        private string _resourceUrl;
        private IEnumerable<int> _caseTypes;

        public PaginationCursorBasedBuilder(Func<List<TModelType>, TResponseType> factory)
        {
            _factory = factory;
        }

        public PaginationCursorBasedBuilder<TResponseType, TModelType> ResourceUrl(string resourceUrl)
        {
            _resourceUrl = resourceUrl;
            return this;
        }

        public PaginationCursorBasedBuilder<TResponseType, TModelType> CaseTypes(IEnumerable<int> caseTypes)
        {
            _caseTypes = caseTypes;
            return this;
        }

        public PaginationCursorBasedBuilder<TResponseType, TModelType> WithSourceItems(IQueryable<TModelType> items)
        {
            _items = items ?? throw new ArgumentNullException(nameof(items));
            return this;
        }

        public PaginationCursorBasedBuilder<TResponseType, TModelType> Limit(int limit)
        {
            if (limit < 1)
                throw new ArgumentException("Number of maximum records to return needs to greater 0 and less then 1000", nameof(limit));

            _limit = limit;
            return this;
        }

        public PaginationCursorBasedBuilder<TResponseType, TModelType> Cursor(string cursor)
        {
            _cursor = cursor;
            return this;
        }

        public TResponseType Build()
        {
            var response = _factory(_items.ToList());
            response.Limit = _limit;
            response.NextPageUrl = GetPageUrl(response.NextCursor);
            response.PrevPageUrl = GetPageUrl(_cursor);

            return response;
        }

        private string GetPageUrl(string cursor)
        {
            var types = string.Empty;
            if (_caseTypes != null && _caseTypes.Any())
            {
                types = string.Join("&types=", _caseTypes);
            }

            return $"{_resourceUrl}?types={types}&cursor={cursor}&limit={_limit}";
        }
    }
}