namespace BookingsApi.DAL.Dtos
{
    public class AnonymisationDataDto
    {
        public List<string> Usernames { get; set; }
        public List<Guid> HearingIds { get; set; }
    }
}