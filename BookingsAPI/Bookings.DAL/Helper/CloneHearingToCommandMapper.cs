using System;
using System.Collections.Generic;
using System.Linq;
using Bookings.Common.Services;
using Bookings.DAL.Commands;
using Bookings.Domain;
using Bookings.Domain.Participants;

namespace Bookings.DAL.Helper
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
            
            var newEndpoints = hearing.GetEndpoints().Select(x =>
            {
                var sip = randomGenerator.GetWeakDeterministic(DateTime.UtcNow.Ticks, 1, 10);
                var pin = randomGenerator.GetWeakDeterministic(DateTime.UtcNow.Ticks, 1, 4);
                return new NewEndpoint
                {
                    Pin = pin,
                    Sip = $"{sip}{sipAddressStem}",
                    DisplayName = x.DisplayName,
                    DefenceAdvocateUsername = x.DefenceAdvocate?.Person?.Username
                };
            }).ToList();

            var duration = 480;
            var command = new CreateVideoHearingCommand(hearing.CaseType, hearing.HearingType, newDate,
                duration, hearing.HearingVenue, participants, cases, false,
                hearing.AudioRecordingRequired, newEndpoints)
            {
                HearingRoomName = hearing.HearingRoomName,
                OtherInformation = hearing.OtherInformation,
                CreatedBy = hearing.CreatedBy,
                SourceId = hearing.Id
            };

            return command;
        }
    }
}