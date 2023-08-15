namespace BookingsApi.DAL.Dtos
{
    public class LinkedParticipantDto
    {
        public LinkedParticipantDto(string participantContactEmail, string linkedParticipantContactEmail, LinkedParticipantType type)
        {
            ParticipantContactEmail = participantContactEmail;
            LinkedParticipantContactEmail = linkedParticipantContactEmail;
            Type = type;
        }

        public string ParticipantContactEmail { get; set; }
        public string LinkedParticipantContactEmail { get; set; }
        public LinkedParticipantType Type { get; set; }
    }
}