using Bookings.DAL.Commands.Core;
using Bookings.DAL.Exceptions;
using Bookings.Domain;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bookings.DAL.Commands
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
                .Include("Participants.SuitabilityAnswers")
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
            participant.AddSuitabilityAnswers(command.SuitabilityAnswers);
            await _context.SaveChangesAsync();
        }
    }
}
