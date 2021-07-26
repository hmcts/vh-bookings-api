using System;
using System.Collections.Generic;
using System.Text;

namespace BookingsApi.Contract.Requests
{
    public class SearchTermAndAccountTypeRequest
    {
        public string Term { get; set; }
        public List<string> AccountType { get; set; } = new List<string>();
    }
}
