using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Bookings.DAL.Queries
{
    public class CursorPagedResult<TResult, TCursor> : IEnumerable<TResult>
    {
        public CursorPagedResult(IEnumerable<TResult> result, TCursor currentCursor, TCursor nextCursor)
        {
            Result = result.ToList();
            CurrentCursor  = currentCursor;
            NextCursor = nextCursor;
        }

        public TCursor NextCursor { get; }

        public TCursor CurrentCursor { get; }

        public IList<TResult> Result { get; }

        public long Count => Result.Count;
        
        public IEnumerator<TResult> GetEnumerator()
        {
            return Result.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}