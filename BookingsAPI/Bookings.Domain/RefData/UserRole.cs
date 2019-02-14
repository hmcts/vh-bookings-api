using Bookings.Domain.Ddd;

namespace Bookings.Domain.RefData
{
    public class UserRole : Entity<int>
    {
        public UserRole(int id, string name)
        {
            Id = id;
            Name = name;
        }
        
        public string Name { get; set; }
    }
}