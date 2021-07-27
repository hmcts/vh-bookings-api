using System;
using System.Collections.Generic;
using System.Text;

namespace BookingsApi.Contract.Requests
{
    public class SearchTermAndAccountTypeRequest
    {
        public SearchTermAndAccountTypeRequest(string term)
        {
            Term = term;
        }
        public string Term { get; set; }

        public List<string> AccountType { get; set; } = new List<string>();
    }
}
