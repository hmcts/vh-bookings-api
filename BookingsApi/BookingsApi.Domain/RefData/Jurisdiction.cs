using System;
using System.Collections.Generic;
using System.Transactions;
using BookingsApi.Domain.RefData;

namespace BookingsApi.Domain
{
    public class Jurisdiction : TrackableEntity<int>
    {
        public Jurisdiction(string code , string name )
        {
            Code = code;
            Name = name;
            IsLive = true;
            CaseTypes = new List<CaseType>();
        }
        
        public string Code { get; set; }
        public string Name { get; set; }
        public bool IsLive { get; set; }
        
        public virtual List<CaseType> CaseTypes{ get; set; }
       
    }
}
