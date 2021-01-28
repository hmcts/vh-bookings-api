using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bookings.DAL.Commands.Core;
using Bookings.DAL.Dtos;
using Bookings.DAL.Helper;
using Bookings.Domain;
using Bookings.Domain.Enumerations;
using Bookings.Domain.RefData;

namespace Bookings.DAL.Commands
{
    public class CreateVideoHearingCommand : ICommand
    {
        public CreateVideoHearingCommand(CaseType caseType, HearingType hearingType, DateTime scheduledDateTime,
            int scheduledDuration, HearingVenue venue, List<NewParticipant> participants, List<Case> cases, 
            bool questionnaireNotRequired, bool audioRecordingRequired, List<NewEndpoint> endpoints, List<LinkedParticipantDto> linkedParticipants)
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
            LinkedParticipants = linkedParticipants;
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
        public List<NewEndpoint> Endpoints { get; }
        public string CancelReason { get; set; }
        public Guid? SourceId { get; set; }
        public List<LinkedParticipantDto> LinkedParticipants { get; }
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
            
            // denotes this hearing is cloned
            if (command.SourceId.HasValue)
            {
                videoHearing.SourceId = command.SourceId;
            }

            await _context.VideoHearings.AddAsync(videoHearing);
            
            var participants = await _hearingService.AddParticipantToService(videoHearing, command.Participants);

            var participantLinks = await _hearingService.CreateParticipantLinks(participants, command.LinkedParticipants);
            foreach (var participantLink in participantLinks)
            {
                await _context.LinkedParticipant.AddAsync(participantLink);

                var interpreteeLink = new LinkedParticipant(participantLink.LinkedParticipantId, 
                    participantLink.ParticipantId, LinkedParticipantType.Interpretee);

                await _context.LinkedParticipant.AddAsync(interpreteeLink);
            }

            videoHearing.AddCases(command.Cases);

            if (command.Endpoints != null && command.Endpoints.Count > 0)
            {
                var dtos = command.Endpoints;
                var newEndpoints = (from dto in dtos
                    let defenceAdvocate =
                        DefenceAdvocateHelper.CheckAndReturnDefenceAdvocate(dto.DefenceAdvocateUsername,
                            videoHearing.GetParticipants())
                    select new Endpoint(dto.DisplayName, dto.Sip, dto.Pin, defenceAdvocate)).ToList();

                videoHearing.AddEndpoints(newEndpoints);
            }

            await _context.SaveChangesAsync();
            command.NewHearingId = videoHearing.Id;
            
        }
    }
}