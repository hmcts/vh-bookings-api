using System;
using System.Linq;
using BookingsApi.Domain.Participants;
using BookingsApi.Infrastructure.Services.Dtos;

namespace BookingsApi.Infrastructure.Services
{
    public static class ParticipantDtoExtension
    {
        public static void SetContactEmailForNonEJudJudgeUser(this ParticipantDto participant, string otherInformation)
        {
            if (participant.HearingRole != "Judge")
            {
                participant.ContactEmailForNonEJudJudgeUser = string.Empty;
            }

            var properties = otherInformation.Split("|");
            participant.ContactEmailForNonEJudJudgeUser= Array.IndexOf(properties, "JudgeEmail") > -1
                ? properties[Array.IndexOf(properties, "JudgeEmail") + 1]
                : string.Empty;
        }
    }
}