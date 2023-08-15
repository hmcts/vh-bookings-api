using System;

namespace BookingsApi.Contract.V1.Queries
{
    public class SearchForHearingsQuery
    {
        public string CaseNumber { get; set; }
        public DateTime? Date { get; set; }
    }
}