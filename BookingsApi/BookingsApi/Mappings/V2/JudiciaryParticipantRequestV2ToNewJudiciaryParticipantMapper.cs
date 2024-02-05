using BookingsApi.Contract.V2.Requests;

namespace BookingsApi.Mappings.V2
{
    public static class JudiciaryParticipantRequestV2ToNewJudiciaryParticipantMapper
    {
        public static NewJudiciaryParticipant Map(JudiciaryParticipantRequestV2 requestParticipant)
        {
            return new NewJudiciaryParticipant
            {
                DisplayName = requestParticipant.DisplayName,
                PersonalCode = requestParticipant.PersonalCode,
                HearingRoleCode = requestParticipant.HearingRoleCode.MapToDomainEnum(),
                OptionalContactEmail = requestParticipant.ContactEmail,
                OptionalContactTelephone = requestParticipant.ContactTelephone
            };
        }
    }
}
