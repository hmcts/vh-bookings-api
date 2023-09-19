using System;

namespace BookingsApi.Domain.RefData
{
    public class HearingRole : TrackableEntity<int>, IComparable
    {
        public HearingRole(int id, string name)
        {
            Id = id;
            Name = name;
            Live = true; // Default to true
        }
        public string Name { get; set; }
        public int UserRoleId { get; set; }
        public int? CaseRoleId { get; set; }
        public UserRole UserRole { get; set; }
        public bool Live { get; set; }

        public int CompareTo(HearingRole other)
        {
            return String.CompareOrdinal(this.Name, other.Name);
        }

        public int CompareTo(object obj)
        {
            return this.CompareTo((HearingRole) obj);
        }
        
        public bool IsJudge() => 
            string.Equals(Name, "Judge", StringComparison.InvariantCultureIgnoreCase);
        
        public bool IsInterpreter () => 
            string.Equals(Name, "Interpreter", StringComparison.InvariantCultureIgnoreCase);
    }
}