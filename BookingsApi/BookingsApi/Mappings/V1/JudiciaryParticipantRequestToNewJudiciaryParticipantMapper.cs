using BookingsApi.Contract.V1.Requests;

namespace BookingsApi.Mappings.V1
{
    public static class JudiciaryParticipantRequestToNewJudiciaryParticipantMapper
    {
        public static NewJudiciaryParticipant Map(JudiciaryParticipantRequest requestParticipant)
        {
            return new NewJudiciaryParticipant
            {
                DisplayName = requestParticipant.DisplayName,
                PersonalCode = requestParticipant.PersonalCode,
                HearingRoleCode = requestParticipant.HearingRoleCode.MapToDomainEnum(),
                OptionalContactTelephone = requestParticipant.OptionalContactTelephone,
                OptionalContactEmail = requestParticipant.OptionalContactEmail
            };
        }
    }
}
