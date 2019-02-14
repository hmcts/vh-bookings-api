using Bookings.Domain.Ddd;
using Bookings.Domain.Enumerations;

namespace Bookings.Domain
{
    public class ChecklistQuestion : Entity<int>
    {
        protected ChecklistQuestion() { }

        public ChecklistQuestion(Role role) : this()
        {
            Role = role;
            
        }

        /// <summary>
        /// The role this question applies to
        /// </summary>
        public Role Role { get; protected set; }

        /// <summary>
        /// A unique identifying string for this question belonging to the given role
        /// </summary>
        public string Key { get; set; }
    }
}
