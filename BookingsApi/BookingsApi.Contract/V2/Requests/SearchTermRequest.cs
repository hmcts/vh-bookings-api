namespace BookingsApi.Contract.V2.Requests
{
    public class SearchTermRequestV2
    {
        public SearchTermRequestV2(string term)
        {
            Term = term;
        }

        public string Term { get; set; }
    }
}
