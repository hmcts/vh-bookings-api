using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Bookings.DAL.Commands.Core;
using Bookings.Domain;
using Bookings.Domain.RefData;

namespace Bookings.DAL.Commands
{
    public class CreateVideoHearingCommand : ICommand
    {
        public CreateVideoHearingCommand(CaseType caseType, HearingType hearingType, DateTime scheduledDateTime,
            int scheduledDuration, HearingVenue venue, List<NewParticipant> participants, List<Case> cases, 
            bool questionnaireNotRequired, bool audioRecordingRequired, List<Endpoint> endpoints)
        {
            CaseType = caseType;
            HearingType = hearingType;
            ScheduledDateTime = scheduledDateTime;
            ScheduledDuration = scheduledDuration;
            Venue = venue;
            Participants = participants;
            Cases = cases;
            QuestionnaireNotRequired = questionnaireNotRequired;
            AudioRecordingRequired = audioRecordingRequired;
            Endpoints = endpoints;
        }

        public Guid NewHearingId { get; set; }
        public CaseType CaseType { get; }
        public HearingType HearingType { get; }
        public DateTime ScheduledDateTime { get; }
        public int ScheduledDuration { get; }
        public HearingVenue Venue { get; }
        public List<NewParticipant> Participants { get; }
        public List<Case> Cases { get; }
        public string HearingRoomName { get; set; }
        public string OtherInformation { get; set; }
        public string CreatedBy { get; set; }
        public bool QuestionnaireNotRequired { get; set; }
        public bool AudioRecordingRequired { get; set; }
        public List<Endpoint> Endpoints { get; }
        public string CancelReason { get; set; }
    }

    public class CreateVideoHearingCommandHandler : ICommandHandler<CreateVideoHearingCommand>
    {
        private readonly BookingsDbContext _context;
        private readonly IHearingService _hearingService;

        public CreateVideoHearingCommandHandler(BookingsDbContext context, IHearingService hearingService)
        {
            _context = context;
            _hearingService = hearingService;
        }

        public async Task Handle(CreateVideoHearingCommand command)
        {
            var videoHearing = new VideoHearing(command.CaseType, command.HearingType, command.ScheduledDateTime,
                command.ScheduledDuration, command.Venue, command.HearingRoomName,
                command.OtherInformation, command.CreatedBy, command.QuestionnaireNotRequired, 
                command.AudioRecordingRequired, command.CancelReason);

            await _context.VideoHearings.AddAsync(videoHearing);
            
            await _hearingService.AddParticipantToService(videoHearing, command.Participants);

            videoHearing.AddCases(command.Cases);

            if (command.Endpoints != null && command.Endpoints.Count > 0)
            {
                videoHearing.AddEndpoints(command.Endpoints);
            }
            
            await _context.SaveChangesAsync();
            command.NewHearingId = videoHearing.Id;
            
        }
    }
}