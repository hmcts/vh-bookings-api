using BookingsApi.Domain.Participants;
using BookingsApi.Common.Services;
using BookingsApi.DAL.Commands;
using BookingsApi.DAL.Dtos;
using BookingsApi.Domain.Dtos;

namespace BookingsApi.DAL.Helper
{
    public static class CloneHearingToCommandMapper
    {
        /// <summary>
        /// Map an existing hearing to a CreateVideoHearingCommand for the purpose of multi-booking days
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
                Person = r.Person,
                Representee = r.Representee,
                CaseRole = r.CaseRole,
                DisplayName = r.DisplayName,
                HearingRole = r.HearingRole
            }));
            participants.AddRange(nonReps.Select(r => new NewParticipant
            {
                Person = r.Person,
                CaseRole = r.CaseRole,
                DisplayName = r.DisplayName,
                HearingRole = r.HearingRole
            }));

            var cases = hearing.GetCases().Select(c => new Case(c.Number, $"{c.Name} Day {hearingDay} of {totalDays}")
            {
                IsLeadCase = c.IsLeadCase
            }).ToList();
            
            var newEndpoints = GetNewEndpointsDtos(hearing, randomGenerator, sipAddressStem);

            var linkedParticipantDtos = GetLinkedParticipantDtos(hearing);
            
            var newJudiciaryParticipants = GetNewJudiciaryParticipants(hearing);

            var command = new CreateVideoHearingCommand(new CreateVideoHearingRequiredDto(
                    hearing.CaseType, newDate, duration, hearing.HearingVenue, cases),
                new CreateVideoHearingOptionalDto(participants, hearing.HearingRoomName, hearing.OtherInformation,
                    hearing.CreatedBy, hearing.AudioRecordingRequired, newEndpoints, null, linkedParticipantDtos,
                    newJudiciaryParticipants, false, hearing.Id, hearing.HearingType));

            return command;
        }

        private static List<EndpointDto> GetNewEndpointsDtos(Hearing hearing, IRandomGenerator randomGenerator, string sipAddressStem)
        {
            var newEndpoints = new List<EndpointDto>();
            foreach (var endpoint in hearing.GetEndpoints())
            {
                string sip;
                do
                {
                    sip = randomGenerator.GetWeakDeterministic(DateTime.UtcNow.Ticks, 1, 10);
                } while (newEndpoints.Exists(x => x.Sip.StartsWith(sip)));
                var pin = randomGenerator.GetWeakDeterministic(DateTime.UtcNow.Ticks, 1, 4);
                var newEndpoint =  new EndpointDto
                {
                    Pin = pin,
                    Sip = $"{sip}{sipAddressStem}",
                    DisplayName = endpoint.DisplayName,
                    EndpointParticipants = endpoint.EndpointParticipants?.Select(x => new EndpointParticipantDto
                    {
                        ContactEmail = x.Participant.Person.ContactEmail,
                        Type = x.Type
                    }).ToList() ?? new List<EndpointParticipantDto>(),
                };
            
                newEndpoints.Add(newEndpoint);
            }

            return newEndpoints;
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