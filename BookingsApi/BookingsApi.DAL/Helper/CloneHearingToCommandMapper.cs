using BookingsApi.Common.Services;
using BookingsApi.DAL.Commands;
using BookingsApi.DAL.Dtos;
using BookingsApi.DAL.Extensions;
using BookingsApi.Domain.Participants;

namespace BookingsApi.DAL.Helper
{
    public static class CloneHearingToCommandMapper
    {
        /// <summary>
        /// Map an existing hearing to a CreateVideoHearingCommand for the purpose of multi-booking days
        /// TODO: remove obsolete CaseRole mappings as part of https://tools.hmcts.net/jira/browse/VIH-10899
        /// </summary>
        /// <param name="hearing">Original hearing</param>
        /// <param name="newDate">New hearing date</param>
        /// <param name="randomGenerator">generator for unique sips</param>
        /// <param name="sipAddressStem">sip address stem</param>
        /// <param name="totalDays">Total number of days of multi-booking hearing</param>
        /// <param name="hearingDay">Day number of multi-booking hearing</param>
        /// <param name="duration">Duration of the multi-booking hearing</param>
        /// <returns>command to create a new video hearing</returns>
        public static CreateVideoHearingCommand CloneToCommand(Hearing hearing, DateTime newDate,
            IRandomGenerator randomGenerator, string sipAddressStem, int totalDays, int hearingDay, int duration)
        {
            var reps = hearing.GetParticipants().Where(x => x.HearingRole.UserRole.IsRepresentative)
                .Cast<Representative>().ToList();
            var nonReps = hearing.GetParticipants().Where(x => !x.HearingRole.UserRole.IsRepresentative).ToList();
            var participants = new List<NewParticipant>();
            participants.AddRange(reps.Select(r => new NewParticipant
            {
                ExternalReferenceId = r.ExternalReferenceId,
                MeasuresExternalId = r.MeasuresExternalId,
                Person = r.Person,
                Representee = r.Representee,
                CaseRole = r.CaseRole,
                DisplayName = r.DisplayName,
                HearingRole = r.HearingRole,
                Screening = r.Screening.MapToScreeningDto()
            }));
            participants.AddRange(nonReps.Select(r => new NewParticipant
            {
                ExternalReferenceId = r.ExternalReferenceId,
                MeasuresExternalId = r.MeasuresExternalId,
                Person = r.Person,
                CaseRole = r.CaseRole,
                DisplayName = r.DisplayName,
                HearingRole = r.HearingRole,
                Screening = r.Screening.MapToScreeningDto()
            }));

            var cases = hearing.GetCases().Select(c => new Case(c.Number, $"{c.Name} Day {hearingDay} of {totalDays}")
            {
                IsLeadCase = c.IsLeadCase
            }).ToList();


            var newEndpoints = GetNewEndpointsDtos(hearing, randomGenerator, sipAddressStem);

            var linkedParticipantDtos = GetLinkedParticipantDtos(hearing);
            
            var newJudiciaryParticipants = GetNewJudiciaryParticipants(hearing);

            var command = new CreateVideoHearingCommand(new CreateVideoHearingRequiredDto(
                    hearing.CaseType, newDate, duration, hearing.HearingVenue, cases, hearing.ConferenceSupplier),
                new CreateVideoHearingOptionalDto(participants, hearing.HearingRoomName, hearing.OtherInformation,
                    hearing.CreatedBy, hearing.AudioRecordingRequired, newEndpoints, null, linkedParticipantDtos,
                    newJudiciaryParticipants, false, hearing.Id, hearing.HearingType));

            return command;
        }

        private static List<NewEndpoint> GetNewEndpointsDtos(Hearing hearing, IRandomGenerator randomGenerator, string sipAddressStem)
        {
            var endpointsToClone = hearing.GetEndpoints().Select(x => new NewEndpointRequestDto()
            {
                DisplayName = x.DisplayName,
                DefenceAdvocateContactEmail = x.DefenceAdvocate?.Person?.ContactEmail,
                ExternalReferenceId = x.ExternalReferenceId,
                MeasuresExternalId = x.MeasuresExternalId,
                Screening = x.Screening.MapToScreeningDto(),
            }).ToList();

            return NewEndpointGenerator.GenerateNewEndpoints(endpointsToClone, randomGenerator, sipAddressStem);
        }

        private static List<LinkedParticipantDto> GetLinkedParticipantDtos(Hearing hearing)
        {
            var hearingParticipants = hearing.Participants.Where(x => x.LinkedParticipants.Any()).ToList();

            var linkedParticipantDtos = new List<LinkedParticipantDto>();
            foreach (var hearingParticipant in hearingParticipants)
            {
                var participantEmail = hearingParticipant.Person.ContactEmail;
                var participantLink = hearingParticipant.GetLinkedParticipants()
                    .FirstOrDefault(x => x.ParticipantId == hearingParticipant.Id);
                if (participantLink != null)
                {
                    var linkedParticipant = hearing.Participants.SingleOrDefault(x => x.Id == participantLink.LinkedId);

                    var linkedParticipantDto = new LinkedParticipantDto
                    (
                        participantEmail,
                        linkedParticipant?.Person.ContactEmail,
                        participantLink.Type
                    );

                    linkedParticipantDtos.Add(linkedParticipantDto);
                }
            }

            return linkedParticipantDtos;
        }

        private static List<NewJudiciaryParticipant> GetNewJudiciaryParticipants(Hearing hearing)
        {
            return hearing
                .GetJudiciaryParticipants()
                .Select(x => new NewJudiciaryParticipant
                {
                    DisplayName = x.DisplayName,
                    PersonalCode = x.JudiciaryPerson.PersonalCode,
                    HearingRoleCode = x.HearingRoleCode
                })
                .ToList();
        }
    }
}