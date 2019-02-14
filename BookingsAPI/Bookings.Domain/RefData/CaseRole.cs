using System.Collections.Generic;
using Bookings.Domain.Ddd;
using Bookings.Domain.Enumerations;

namespace Bookings.Domain.RefData
{
    public class CaseRole : Entity<int>
    {
        public CaseRole(int id, string name)
        {
            Id = id;
            Name = name;
            HearingRoles = new List<HearingRole>();
        }
        public string Name { get; set; }
        public CaseRoleGroup Group { get; set; }
        public List<HearingRole> HearingRoles { get; set; }
    }
}