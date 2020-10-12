using System;

namespace Bookings.Api.Contract.Queries
{
    public class SearchForHearingsQuery
    {
        public string CaseNumber { get; set; }
        public DateTime? Date { get; set; }
    }
}