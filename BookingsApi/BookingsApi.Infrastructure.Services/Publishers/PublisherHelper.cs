using BookingsApi.Common;
using BookingsApi.Domain;
using BookingsApi.Domain.Participants;
using System.Collections.Generic;
using System.Linq;

namespace BookingsApi.Infrastructure.Services.Publishers
{
    public static class PublisherHelper
    {
        public static IEnumerable<Participant> GetExistingParticipantsSinceLastUpdate(VideoHearing videoHearing)
        {
            var existingParticipants = videoHearing.Participants.Where(x => x is Judge || (x.DoesPersonAlreadyExist() && x.Person?.ContactEmail != x.Person?.Username));
            var areParticipantsAddedToExistingBooking = existingParticipants.Any(x => x.CreatedDate.TrimMilliseconds() > videoHearing.CreatedDate.TrimMilliseconds());
            if (areParticipantsAddedToExistingBooking)
            {
                existingParticipants = existingParticipants.Where(x => x.CreatedDate.TrimMilliseconds() == videoHearing.UpdatedDate.TrimMilliseconds());
            }

            return existingParticipants;
        }

        public static IEnumerable<Participant> GetNewParticipantsSinceLastUpdate(VideoHearing videoHearing)
        {
            var newParticipants = videoHearing.Participants.Where(x => x is not Judge && (!x.DoesPersonAlreadyExist() || x.Person?.ContactEmail == x.Person?.Username));
            var areParticipantsAddedToExistingBooking = newParticipants.Any(x => x.CreatedDate.TrimMilliseconds() > videoHearing.CreatedDate.TrimMilliseconds());
            if (areParticipantsAddedToExistingBooking)
            {
                newParticipants = newParticipants.Where(x => x.CreatedDate.TrimMilliseconds() == videoHearing.UpdatedDate.TrimMilliseconds());
            }
            return newParticipants;
        }

        public static IEnumerable<Participant> GetAddedParticipantsSinceLastUpdate(VideoHearing videoHearing)
        {
            var newParticipants = videoHearing.Participants;
            var areParticipantsAddedToExistingBooking = videoHearing.Participants.Any(x => x.CreatedDate.TrimMilliseconds() > videoHearing.CreatedDate.TrimMilliseconds());
            if (areParticipantsAddedToExistingBooking)
            {
                newParticipants = videoHearing.Participants.Where(x => x.CreatedDate.TrimMilliseconds() == videoHearing.UpdatedDate.TrimMilliseconds()).ToList();
            }
            return newParticipants;
        }

        public static IEnumerable<(Participant participant, int totalDays)> GetNewParticipantsForMultiDaysSinceLastUpdate(VideoHearing videoHearing, IList<VideoHearing> multiDayHearings)
        {
            var participants = videoHearing.Participants;
            var areParticipantsAddedToExistingBooking = participants.Any(x => x.CreatedDate.TrimMilliseconds() > videoHearing.CreatedDate.TrimMilliseconds());
            if (areParticipantsAddedToExistingBooking)
            {
                participants = participants.Where(x => x.CreatedDate.TrimMilliseconds() == videoHearing.UpdatedDate.TrimMilliseconds()).ToList();
            }

            var newParticipants = new List<(Participant participant, int totalDays)>();
            foreach (var participant in participants)
            {
                // Only send the notification once
                var previousHearings = multiDayHearings.Where(x => x.ScheduledDateTime < videoHearing.ScheduledDateTime).ToList();
                var isPreviouslyNotified = previousHearings.Exists(x => x.Participants.Any(y => y.Id == participant.Id));
                if (isPreviouslyNotified)
                {
                    continue;
                }

                var totalDays = multiDayHearings.Count(x => x.ScheduledDateTime >= videoHearing.ScheduledDateTime);
                newParticipants.Add((participant, totalDays));
            }

            return newParticipants;
        }
    }
}
