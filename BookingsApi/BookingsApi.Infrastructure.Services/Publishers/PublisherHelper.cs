using System;
using BookingsApi.Common;
using BookingsApi.Domain;
using BookingsApi.Domain.Participants;
using System.Collections.Generic;
using System.Linq;

namespace BookingsApi.Infrastructure.Services.Publishers
{
    public static class PublisherHelper
    {
        public static IEnumerable<Participant> GetExistingParticipantsSinceLastUpdate(VideoHearing videoHearing, IList<Guid> participantsToBeNotifiedIds)
        {
            var existingParticipants = videoHearing.Participants.Where(x =>
                x is Judge || (x.DoesPersonAlreadyExist() && x.Person?.ContactEmail != x.Person?.Username) &&
                participantsToBeNotifiedIds.Contains(x.Id));
            
            // var existingParticipants = videoHearing.Participants.Where(x => x is Judge || (x.DoesPersonAlreadyExist() && x.Person?.ContactEmail != x.Person?.Username));
            // var areParticipantsAddedToExistingBooking = existingParticipants.Any(x => x.CreatedDate.TrimMilliseconds() > videoHearing.CreatedDate.TrimMilliseconds());
            // if (areParticipantsAddedToExistingBooking)
            // {
            //     existingParticipants = existingParticipants.Where(x => x.CreatedDate.TrimSeconds() == videoHearingUpdateDate);
            // }

            return existingParticipants;
        }

        /// <summary>
        /// Retrieve new participant added to the hearing based on the DoesPersonAlreadyExist method
        /// and the comparison of the Person.ContactEmail and Person.Username to be the same as the BQS will update after notification
        /// </summary>
        /// <param name="videoHearing"></param>
        /// <returns></returns>
        public static IEnumerable<Participant> GetNewParticipantsSinceLastUpdate(VideoHearing videoHearing, IList<Guid> participantsToBeNotifiedIds)
        {
            var newParticipants = videoHearing.Participants.Where(x =>
                x is not Judge && (!x.DoesPersonAlreadyExist() || x.Person?.ContactEmail == x.Person?.Username) &&
                participantsToBeNotifiedIds.Contains(x.Id));
            
            //var newParticipants = videoHearing.Participants.Where(x => x is not Judge && (!x.DoesPersonAlreadyExist() || x.Person?.ContactEmail == x.Person?.Username));
            // var areParticipantsAddedToExistingBooking = newParticipants.Any(x => x.CreatedDate.TrimMilliseconds() > videoHearing.CreatedDate.TrimMilliseconds());
            // if (areParticipantsAddedToExistingBooking)
            // {
            //     newParticipants = newParticipants.Where(x => x.CreatedDate.TrimSeconds() == videoHearingUpdateDate);
            // }
            return newParticipants;
        }

        public static IEnumerable<Participant> GetAddedParticipantsSinceLastUpdate(VideoHearing videoHearing, IList<Guid> participantsToBeNotifiedIds)
        {
            var newParticipants = videoHearing.Participants
                .Where(p=>participantsToBeNotifiedIds.Contains(p.Id));
            // var areParticipantsAddedToExistingBooking = videoHearing.Participants.Any(x => x.CreatedDate.TrimMilliseconds() > videoHearing.CreatedDate.TrimMilliseconds());
            // if (areParticipantsAddedToExistingBooking)
            // {
            //     newParticipants = videoHearing.Participants.Where(x => x.CreatedDate.TrimMilliseconds() == videoHearing.UpdatedDate.TrimMilliseconds()).ToList();
            // }
            return newParticipants;
        }
        
        public static IEnumerable<JudiciaryParticipant> GetAddedJudiciaryParticipantsSinceLastUpdate(VideoHearing videoHearing, IList<Guid> participantsToBeNotifiedIds)
        {
            var existingParticipants = videoHearing.JudiciaryParticipants
                .Where(x=> participantsToBeNotifiedIds.Contains(x.Id));
            // var areParticipantsAddedToExistingBooking = existingParticipants.Any(x => x.CreatedDate?.TrimMilliseconds() > videoHearing.CreatedDate.TrimMilliseconds());
            // if (areParticipantsAddedToExistingBooking)
            // {
            //     existingParticipants = existingParticipants.Where(x => x.CreatedDate?.TrimSeconds() == videoHearingUpdateDate).ToList();
            // }

            return existingParticipants;
        }
    }
}
