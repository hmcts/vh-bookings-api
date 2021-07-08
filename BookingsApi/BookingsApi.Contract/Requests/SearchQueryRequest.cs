using System;
using System.Collections.Generic;
using System.Text;

namespace BookingsApi.Contract.Requests
{
    public class SearchQueryRequest
    {
        public string Term { get; set; }
        public List<string> JudiciaryUsernamesFromAd { get; set; } = new List<string>();
    }
}
