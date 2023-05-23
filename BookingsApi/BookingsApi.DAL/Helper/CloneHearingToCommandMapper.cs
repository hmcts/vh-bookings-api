using System;
using System.Collections.Generic;
using System.Linq;
using BookingsApi.Domain;
using BookingsApi.Domain.Participants;
using BookingsApi.Common.Services;
using BookingsApi.DAL.Commands;
using BookingsApi.DAL.Dtos;

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
        /// <returns>command to create a new video hearing</returns>
        public static CreateVideoHearingCommand CloneToCommand(Hearing hearing, DateTime newDate,
            IRandomGenerator randomGenerator, string sipAddressStem, int totalDays, int hearingDay)
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

            var duration = 480;
            var command = new CreateVideoHearingCommand(hearing.CaseType, hearing.HearingType, newDate,
                duration, hearing.HearingVenue, participants, cases, true,
                hearing.AudioRecordingRequired, newEndpoints, linkedParticipantDtos, false)
            {
                HearingRoomName = hearing.HearingRoomName,
                OtherInformation = hearing.OtherInformation,
                CreatedBy = hearing.CreatedBy,
                SourceId = hearing.Id
            };

            return command;
        }

        private static List<NewEndpoint> GetNewEndpointsDtos(Hearing hearing, IRandomGenerator randomGenerator, string sipAddressStem)
        {
            var newEndpoints = new List<NewEndpoint>();
            foreach (var endpoint in hearing.GetEndpoints())
            {
                string sip;
                do
                {
                    sip = randomGenerator.GetWeakDeterministic(DateTime.UtcNow.Ticks, 1, 10);
                } while (newEndpoints.Any(x => x.Sip.StartsWith(sip)));
                var pin = randomGenerator.GetWeakDeterministic(DateTime.UtcNow.Ticks, 1, 4);
                var newEndpoint =  new NewEndpoint
                {
                    Pin = pin,
                    Sip = $"{sip}{sipAddressStem}",
                    DisplayName = endpoint.DisplayName,
                    ContactEmail = endpoint.DefenceAdvocate?.Person?.ContactEmail
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
    }
}