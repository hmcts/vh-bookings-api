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
                    {LinkedId = x.LinkedId, TypeV2 = x.Type.MapToContractEnum()}).ToList(),
                InterpreterLanguage = participant.InterpreterLanguage != null ?
                    InterpreterLanguageToResponseMapper.MapInterpreterLanguageToResponse(participant.InterpreterLanguage) :
                    null,
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

            if (participant.Screening != null)
            {
                var screeningResponse = new ScreeningResponseV2
                {
                    Type = participant.Screening.Type.MapToContractEnum(),
                    ProtectFromEndpointsIds = participant.Screening.GetEndpoints().Select(x=> x.EndpointId!.Value).ToList(),
                    ProtectFromParticipantsIds = participant.Screening.GetParticipants().Select(x=> x.ParticipantId!.Value).ToList()
                };

                participantResponse.Screening = screeningResponse;
            }

            participantResponse.TrimAllStringsRecursively();
            return participantResponse;
        }
    }
}