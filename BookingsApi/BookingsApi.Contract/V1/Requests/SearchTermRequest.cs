namespace BookingsApi.Contract.V1.Requests
{
    public class SearchTermRequest(string term)
    {
        public string Term { get; set; } = term;
    }
}
