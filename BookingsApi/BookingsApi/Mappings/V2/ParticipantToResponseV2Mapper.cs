using BookingsApi.Contract.V2.Responses;
using BookingsApi.Mappings.V1;
using BookingsApi.Mappings.V2.Extensions;

namespace BookingsApi.Mappings.V2
{
    public class ParticipantToResponseV2Mapper
    {
        public ParticipantResponseV2 MapParticipantToResponse(Participant participant)
        {
            var participantResponse = new ParticipantResponseV2
            {
                Id = participant.Id,
                ExternalReferenceId = participant.ExternalReferenceId,
                MeasuresExternalId = participant.MeasuresExternalId,
                DisplayName = participant.DisplayName,
                HearingRoleCode = participant.HearingRole.Code,
                HearingRoleName = participant.HearingRole.Name,
                UserRoleName = participant.HearingRole.UserRole.Name,
                Title = participant.Person.Title,
                FirstName = participant.Person.FirstName,
                LastName = participant.Person.LastName,
                MiddleNames = participant.Person.MiddleNames,
                ContactEmail = participant.Person.ContactEmail,
                Username = participant.Person.Username,
                TelephoneNumber = participant.Person.TelephoneNumber,
                Organisation = participant.Person.Organisation?.Name,
                LinkedParticipants = participant.LinkedParticipants.Select(x => new LinkedParticipantResponseV2
                    { LinkedId = x.LinkedId, TypeV2 = x.Type.MapToContractEnum() }).ToList(),
                InterpreterLanguage = participant.InterpreterLanguage != null
                    ? InterpreterLanguageToResponseMapperV2.MapInterpreterLanguageToResponse(participant
                        .InterpreterLanguage)
                    : null,
                OtherLanguage = participant.OtherLanguage
            };

            switch (participant.HearingRole.UserRole.Name)
            {
                case "Representative":
                    var representative = (Representative)participant;
                    participantResponse.Representee = representative.Representee;
                    break;
                case "Individual":
                case "Judge":
                case "StaffMember":
                case "JudicialOfficeHolder":
                    break;
            }

            participantResponse.Screening = participant.Screening == null
                ? null
                : ScreeningToResponseV2Mapper.MapScreeningToResponse(participant.Screening);


            participantResponse.TrimAllStringsRecursively();
            return participantResponse;
        }
    }
}