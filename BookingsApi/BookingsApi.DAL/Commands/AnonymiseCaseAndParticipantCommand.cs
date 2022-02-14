﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BookingsApi.DAL.Commands.Core;
using BookingsApi.Domain;
using BookingsApi.Domain.Participants;
using Microsoft.EntityFrameworkCore;

namespace BookingsApi.DAL.Commands
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
                .Select(r => AnonymiseName(r, hearindIds))
                .ToList();

            _context.VideoHearings.UpdateRange(videoHearings);


            var participants = _context.Participants
                .Where(participant => hearindIds.Contains(participant.HearingId))
                 .ToList();

            participants = participants
                .Select(r => AnonymiseDisplayName(r))
                .ToList();

            _context.Participants.UpdateRange(participants);


            if (!(await _context.JobHistory.FirstOrDefaultAsync() is UpdateJobHistory jobHistory))
            {
                await _context.JobHistory.AddAsync(new UpdateJobHistory());
            }
            else
            {
                jobHistory.UpdateLastRunDate();
            }

            await _context.SaveChangesAsync();
        }

        private Participant AnonymiseDisplayName(Participant participant)
        {
            var randomString = RandomStringGenerator.GenerateRandomString(9);

            if (!participant.DisplayName.ToLowerInvariant().Contains(AnonymisedNameSuffix))
            {
                participant.DisplayName = $"{randomString}{AnonymisedNameSuffix}";
            }

            if (participant is Representative castingParticipantAsRepresentative &&
                !string.IsNullOrEmpty(castingParticipantAsRepresentative.Representee))
            {
                castingParticipantAsRepresentative.Representee = randomString;
            }

            return participant;
        }

        private VideoHearing AnonymiseName(VideoHearing hearing, IList<Guid> hearingIds)
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