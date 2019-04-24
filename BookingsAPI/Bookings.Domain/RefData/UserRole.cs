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
        public bool IsIndividual => Name.ToLower().Equals("individual");
        public bool IsRepresentative => Name.ToLower().Equals("representative");
    }
}