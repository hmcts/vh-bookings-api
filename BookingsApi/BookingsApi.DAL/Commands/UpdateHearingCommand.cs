using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BookingsApi.Domain;
using BookingsApi.DAL.Commands.Core;
using BookingsApi.DAL.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace BookingsApi.DAL.Commands
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
        public bool AudioRecordingRequired { get; set; }

        public UpdateHearingCommand(Guid hearingId, DateTime scheduledDateTime, int scheduledDuration,
            HearingVenue hearingVenue, string hearingRoomName, string otherInformation, string updatedBy,
            List<Case> cases, bool questionnaireNotRequired, bool audioRecordingRequired)
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
            AudioRecordingRequired = audioRecordingRequired;
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
                .Include(x=>x.HearingCases).ThenInclude(x=> x.Case)
                // .Include(x=>x.HearingVenue)
                .SingleOrDefaultAsync(x => x.Id == command.HearingId);

            if (hearing == null)
            {
                throw new HearingNotFoundException(command.HearingId);
            }

            hearing.UpdateHearingDetails(command.HearingVenue, command.ScheduledDateTime,
                command.ScheduledDuration, command.HearingRoomName, command.OtherInformation,
                command.UpdatedBy, command.Cases, command.QuestionnaireNotRequired, command.AudioRecordingRequired);

            await _context.SaveChangesAsync();
        }
    }
}