
namespace BookingsApi.Domain.RefData
{
    public class UserRole : TrackableEntity<int>
    {
        public UserRole(int id, string name)
        {
            Id = id;
            Name = name;
        }
        
        public string Name { get; set; }
        public bool IsIndividual => Name.ToLower().Equals("individual");
        public bool IsRepresentative => Name.ToLower().Equals("representative");
        public bool IsJudge => Name.ToLower().Equals("judge");
        public bool IsJudicialOfficeHolder => Name.ToLower().Equals("judicial office holder");
    }
}