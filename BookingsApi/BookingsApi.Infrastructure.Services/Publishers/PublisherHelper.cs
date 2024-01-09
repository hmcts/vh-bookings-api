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
    }
}
