using System;
using System.Collections.Generic;
using System.Text;

namespace BookingsApi.Contract
{
    public class SearchTermAndAccountTypeRequest
    {
        public string SearchTerm { get; set; }
        public List<string> AccountType { get; set; } = new List<string>();
    }
}
