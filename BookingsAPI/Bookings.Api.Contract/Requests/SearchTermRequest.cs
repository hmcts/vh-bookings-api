namespace Bookings.Api.Contract.Requests
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
