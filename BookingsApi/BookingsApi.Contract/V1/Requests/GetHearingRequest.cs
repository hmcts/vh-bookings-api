using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace BookingsApi.Contract.V1.Requests
{
    public class GetHearingRequest
    {
        public GetHearingRequest()
        {
            Types = new List<int>();
            Users = new List<Guid>();
            VenueIds = new List<int>();
        }
        
        private const int DefaultLimit = 100;

        public const string DefaultCursor = "0";

        [JsonPropertyName("types")]
        public List<int> Types { get; set; }
        
        [JsonPropertyName("users")]
        public List<Guid> Users { get; set; }

        [JsonPropertyName("cursor")]
        public string Cursor { get; set; } = DefaultCursor;

        [JsonPropertyName("limit")]
        public int Limit { get; set; } = DefaultLimit;

        [JsonPropertyName("fromDate")]
        public DateTime? FromDate { get; set; }

        [JsonPropertyName("caseNumber")]
        public string CaseNumber { get; set; }

        [JsonPropertyName("venueIds")]
        public List<int> VenueIds { get; set; }

        [JsonPropertyName("endDate")]
        public DateTime? EndDate { get; set; }

        [JsonPropertyName("lastName")]
        public string LastName { get; set; }

        [JsonPropertyName("noJudge")]
        public bool NoJudge { get; set; }
        
        [JsonPropertyName("noAllocated")]
        public bool NoAllocated { get; set; }
    }
}
