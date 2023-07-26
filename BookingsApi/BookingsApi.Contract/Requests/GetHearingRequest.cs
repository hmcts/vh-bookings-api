using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace BookingsApi.Contract.Requests
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

        [JsonProperty("types")]
        public List<int> Types { get; set; }
        
        [JsonProperty("users")]
        public List<Guid> Users { get; set; }

        [JsonProperty("cursor")]
        public string Cursor { get; set; } = DefaultCursor;

        [JsonProperty("limit")]
        public int Limit { get; set; } = DefaultLimit;

        [JsonProperty("fromDate")]
        public DateTime? FromDate { get; set; }

        [JsonProperty("caseNumber")]
        public string CaseNumber { get; set; }

        [JsonProperty("venueIds")]
        public List<int> VenueIds { get; set; }

        [JsonProperty("endDate")]
        public DateTime? EndDate { get; set; }

        [JsonProperty("lastName")]
        public string LastName { get; set; }

        [JsonProperty("noJudge")]
        public bool NoJudge { get; set; }
        
        [JsonProperty("noAllocated")]
        public bool NoAllocated { get; set; }
    }
}
