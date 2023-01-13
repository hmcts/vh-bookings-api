using System;

namespace BookingsApi.Contract.Requests
{
    public class SearchForAllocationHearingsRequest
    {
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public Guid[] Cso { get; set; }
        public string[] CaseType { get; set; }
        public string CaseNumber { get; set; }
        public bool IsAllocated { get; set; }
    }
}