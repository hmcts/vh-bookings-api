using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BookingsApi.DAL.Commands.Core;
using BookingsApi.DAL.Exceptions;
using BookingsApi.Domain;
using Microsoft.EntityFrameworkCore;

namespace BookingsApi.DAL.Commands.V1
{
    public class UpdateSuitabilityAnswersCommand : ICommand
    {
        public Guid ParticipantId { get; set; }
        public Guid HearingId { get; set; }
        public IList<SuitabilityAnswer> SuitabilityAnswers { get; set; }
        public UpdateSuitabilityAnswersCommand(Guid hearingId, Guid participantId, IList<SuitabilityAnswer> suitabilityAnswers)
        {
            ParticipantId = participantId;
            HearingId = hearingId;
            SuitabilityAnswers = suitabilityAnswers;
        }
    }

    public class UpdateSuitabilityAnswersCommandHandler : ICommandHandler<UpdateSuitabilityAnswersCommand>
    {
        private readonly BookingsDbContext _context;

        public UpdateSuitabilityAnswersCommandHandler(BookingsDbContext context)
        {
            _context = context;
        }

        public async Task Handle(UpdateSuitabilityAnswersCommand command)
        {
            var hearing = await _context.VideoHearings
                .Include("Participants.Person")
                .Include("Participants.Questionnaire")
                .Include("Participants.Questionnaire.SuitabilityAnswers")
                .SingleOrDefaultAsync(x => x.Id == command.HearingId);

            if (hearing == null)
            {
                throw new HearingNotFoundException(command.HearingId);
            }

            var participant = hearing.Participants.FirstOrDefault(p => p.Id == command.ParticipantId);
            if (participant == null)
            {
                throw new ParticipantNotFoundException(command.ParticipantId);
            }

            if (participant.Questionnaire == null)
            {
                participant.Questionnaire = new Questionnaire
                {
                    Participant = participant,
                    ParticipantId = participant.Id
                };
            }

            participant.Questionnaire.AddSuitabilityAnswers(command.SuitabilityAnswers);
            await _context.SaveChangesAsync();
        }
    }
}
