using System;
using BookingsApi.Infrastructure.Services.Dtos;

namespace BookingsApi.Infrastructure.Services
{
    public static class ParticipantDtoExtension
    {
        public static void SetOtherFieldsForNonEJudJudgeUser(this ParticipantDto participant, string otherInformation)
        {
            if (participant.UserRole != "Judge")
            {
                participant.ContactEmailForNonEJudJudgeUser = string.Empty;
                participant.ContactPhoneForNonEJudJudgeUser = string.Empty;
            }

            var properties = otherInformation.Split("|");
            participant.ContactEmailForNonEJudJudgeUser = Array.IndexOf(properties, "JudgeEmail") > -1
                ? properties[Array.IndexOf(properties, "JudgeEmail") + 1]
                : string.Empty;

            participant.ContactPhoneForNonEJudJudgeUser = Array.IndexOf(properties, "JudgePhone") > -1
                ? properties[Array.IndexOf(properties, "JudgePhone") + 1]
                : string.Empty;
        }
    }
}