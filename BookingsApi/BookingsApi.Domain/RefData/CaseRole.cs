using System;
using System.Collections.Generic;
using BookingsApi.Domain.Enumerations;

namespace BookingsApi.Domain.RefData
{
    public class CaseRole : TrackableEntity<int>, IComparable
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

        public int CompareTo(CaseRole other)
        {
            return String.CompareOrdinal(this.Name, other.Name);
        }

        public int CompareTo(object obj)
        {
            return this.CompareTo((CaseRole)obj);
        }
    }
}