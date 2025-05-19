using System;
using System.Diagnostics.CodeAnalysis;

namespace BookingsApi.Contract.V1.Queries
{
    [ExcludeFromCodeCoverage(Justification = "Deprecated feature but kept for internal testing")]
    public class SearchForHearingsQuery
    {
        public string CaseNumber { get; set; }
        public DateTime? Date { get; set; }
    }
}