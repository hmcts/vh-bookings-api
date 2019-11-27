using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Bookings.DAL.Commands.Core;
using Bookings.Domain;
using Bookings.DAL.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace Bookings.DAL.Commands
{
    public class UpdateHearingCommand : ICommand
    {
        public Guid HearingId { get; set; }
        public DateTime ScheduledDateTime { get; set; }
        public int ScheduledDuration { get; set; }
        public HearingVenue HearingVenue { get; set; }
        public string HearingRoomName { get; set; }
        public string OtherInformation { get; set; }
        public string UpdatedBy { get; set; }
        public List<Case> Cases { get; set; }
        public bool QuestionnaireNotRequired { get; set; }
        public bool StreamingFlag { get; set; }

        public UpdateHearingCommand(Guid hearingId, DateTime scheduledDateTime, int scheduledDuration,
            HearingVenue hearingVenue, string hearingRoomName, string otherInformation, string updatedBy, 
            List<Case> cases, bool questionnaireNotRequired)
        {
            HearingId = hearingId;
            ScheduledDateTime = scheduledDateTime;
            ScheduledDuration = scheduledDuration;
            HearingVenue = hearingVenue;
            HearingRoomName = hearingRoomName;
            OtherInformation = otherInformation;
            UpdatedBy = updatedBy;
            Cases = cases;
            QuestionnaireNotRequired = questionnaireNotRequired;
        }
    }

    public class UpdateHearingCommandHandler : ICommandHandler<UpdateHearingCommand>
    {
        private readonly BookingsDbContext _context;

        public UpdateHearingCommandHandler(BookingsDbContext context)
        {
            _context = context;
        }

        public async Task Handle(UpdateHearingCommand command)
        {
            var hearing = await _context.VideoHearings
                .Include("HearingCases.Case")
                .SingleOrDefaultAsync(x => x.Id == command.HearingId);

            if (hearing == null)
            {
                throw new HearingNotFoundException(command.HearingId);
            }

            hearing.UpdateHearingDetails(command.HearingVenue, command.ScheduledDateTime, 
                command.ScheduledDuration, command.HearingRoomName, command.OtherInformation, 
                command.UpdatedBy, command.Cases, command.QuestionnaireNotRequired, command.StreamingFlag);

            await _context.SaveChangesAsync();
        }
    }
}