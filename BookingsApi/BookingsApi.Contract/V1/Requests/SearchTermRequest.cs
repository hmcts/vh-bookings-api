namespace BookingsApi.Contract.V1.Requests
{
    public class SearchTermRequest
    {
        public SearchTermRequest(string term)
        {
            Term = term;
        }

        public string Term { get; set; }
    }
}
