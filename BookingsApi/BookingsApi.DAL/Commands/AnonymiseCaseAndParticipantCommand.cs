using System;
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
            foreach (var hearingId in command.HearingIds)
            {
                _context.VideoHearings
                    .Include(vh => vh.HearingCases)
                    .ThenInclude(vh => vh.Case)
                    .Where(hearing => hearing.Id == hearingId).ToList().ForEach(hearing =>
                {
                    hearing.HearingCases.ToList().ForEach(
                        hearingCase =>
                        {
                            if (hearingCase.HearingId == hearingId
                                && !hearingCase.Case.Name.ToLowerInvariant().Contains(AnonymisedNameSuffix))
                            {
                                hearingCase.Case.Name =
                                    $"{RandomStringGenerator.GenerateRandomString(9)}{AnonymisedNameSuffix}";
                            }
                        });
                });

                _context.Participants
                    .Where(participant => participant.HearingId == hearingId)
                    .ToList()
                    .ForEach(participant =>
                    {
                        var randomString = RandomStringGenerator.GenerateRandomString(9);

                        if (!participant.DisplayName.ToLowerInvariant().Contains(AnonymisedNameSuffix))
                        {
                            participant.DisplayName = $"{randomString}{AnonymisedNameSuffix}";
                        }

                        var castingParticipantAsRepresentative = participant as Representative;

                        if (castingParticipantAsRepresentative != null &&
                            !string.IsNullOrEmpty(castingParticipantAsRepresentative.Representee))
                        {
                            castingParticipantAsRepresentative.Representee = randomString;
                        }
                    });
            }

            var jobHistory = await _context.JobHistory.FirstOrDefaultAsync() as UpdateJobHistory; 

            if (jobHistory == null)
            {
                await _context.JobHistory.AddAsync(new UpdateJobHistory());
            }
            else
            {
                jobHistory.UpdateLastRunDate();
            }

            await _context.SaveChangesAsync();
        }
    }
}