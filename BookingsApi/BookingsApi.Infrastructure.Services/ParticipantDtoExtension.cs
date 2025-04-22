using BookingsApi.Infrastructure.Services.Dtos;

namespace BookingsApi.Infrastructure.Services
{
    public static class ParticipantDtoExtension
    {
        public static string GetContactEmail(this ParticipantDto participant)
        {
            return !string.IsNullOrEmpty(participant.ContactEmailForNonEJudJudgeUser) 
                ? participant.ContactEmailForNonEJudJudgeUser 
                : participant.ContactEmail;
        }

        public static string GetContactTelephone(this ParticipantDto participant)
        {
            return !string.IsNullOrEmpty(participant.ContactPhoneForNonEJudJudgeUser) 
                ? participant.ContactPhoneForNonEJudJudgeUser 
                : participant.ContactTelephone;
        }
    }
}