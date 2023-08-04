namespace BookingsApi.DAL.Queries
{
    public class CursorPagedResult<TResult, TCursor> : IEnumerable<TResult>
    {
        public CursorPagedResult(IEnumerable<TResult> result, TCursor nextCursor)
        {
            Result = result.ToList();
            NextCursor = nextCursor;
        }

        public TCursor NextCursor { get; }

        private IList<TResult> Result { get; }

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