using System.Collections.Generic;
using BookingsApi.Domain.Ddd;

namespace BookingsApi.Domain.RefData
{
    public class CaseType : Entity<int>
    {
        public CaseType(int id, string name)
        {
            Id = id;
            Name = name;
            CaseRoles = new List<CaseRole>();
            HearingTypes = new List<HearingType>();
        }
        
        public string Name { get; set; }
        public virtual List<CaseRole> CaseRoles { get; set; }
        public virtual List<HearingType> HearingTypes { get; set; }
    }
}