using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BookingsApi.DAL.Commands.Core;
using BookingsApi.Domain;
using BookingsApi.Domain.Participants;
using Microsoft.EntityFrameworkCore;

namespace BookingsApi.DAL.Commands.V1
{
    public class AnonymiseCaseAndParticipantCommand : ICommand
    {
        public List<Guid> HearingIds { get; set; }
    }

    public class AnonymiseCaseAndParticipantCommandHandler : ICommandHandler<AnonymiseCaseAndParticipantCommand>
    {
        private readonly BookingsDbContext _context;
        public const string AnonymisedNameSuffix = "@email.net";

        public AnonymiseCaseAndParticipantCommandHandler(BookingsDbContext context)
        {
            _context = context;
        }

        public async Task Handle(AnonymiseCaseAndParticipantCommand command)
        {
            var hearindIds = command.HearingIds;

            var videoHearings = _context.VideoHearings
                .Include(vh => vh.HearingCases)
                .ThenInclude(vh => vh.Case)
                .Where(hearing => hearindIds.Contains(hearing.Id))
                .ToList();

            videoHearings = videoHearings
                .Select(r => AnonymiseCaseName(r, hearindIds))
                .ToList();

            _context.VideoHearings.UpdateRange(videoHearings);


            var participants = _context.Participants
                .Where(participant => hearindIds.Contains(participant.HearingId))
                 .ToList();

            participants = participants
                .Select(r => AnonymiseParticipantDisplayName(r))
                .ToList();

            _context.Participants.UpdateRange(participants);

            await _context.SaveChangesAsync();
        }

        private Participant AnonymiseParticipantDisplayName(Participant participant)
        {
            var randomString = RandomStringGenerator.GenerateRandomString(9);

            if (!participant.DisplayName.ToLowerInvariant().Contains(AnonymisedNameSuffix))
            {
                participant.DisplayName = $"{randomString}{AnonymisedNameSuffix}";
            }

            if (participant is Representative participantAsRepresentative &&
                !string.IsNullOrEmpty(participantAsRepresentative.Representee))
            {
                participantAsRepresentative.Representee = randomString;
            }

            return participant;
        }

        private VideoHearing AnonymiseCaseName(VideoHearing hearing, IList<Guid> hearingIds)
        {
            hearing.HearingCases.ToList().ForEach(
                hearingCase =>
                {
                    if (hearingIds.Any(r => r == hearingCase.HearingId)
                             && !hearingCase.Case.Name.ToLowerInvariant().Contains(AnonymisedNameSuffix))
                    {
                        hearingCase.Case.Name =
                             $"{RandomStringGenerator.GenerateRandomString(9)}{AnonymisedNameSuffix}";
                    }
                });

            return hearing;
        }
    }
}