﻿using System.Collections.Generic;
using Bookings.Domain.Ddd;

namespace Bookings.Domain
{
    public class Case : Entity<long>
    {
        public Case(string number, string name)
        {
            Number = number;
            Name = name;

        }

        public string Number { get; set; }
        public string Name { get; set; }
        public bool IsLeadCase { get; set; }
        public virtual IList<HearingCase> HearingCases { get; protected set; }
    }
}