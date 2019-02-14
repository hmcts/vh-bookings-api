using Bookings.Domain.Ddd;

namespace Bookings.Domain.RefData
{
    public class HearingRole : Entity<int>
    {
        public HearingRole(int id, string name)
        {
            Id = id;
            Name = name;
        }
        public string Name { get; set; }
        public int UserRoleId { get; set; }
        public UserRole UserRole { get; set; }
    }
}