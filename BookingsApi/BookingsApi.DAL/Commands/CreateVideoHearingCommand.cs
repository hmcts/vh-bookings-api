using BookingsApi.DAL.Dtos;
using BookingsApi.DAL.Helper;
using BookingsApi.DAL.Services;
using BookingsApi.Domain.Dtos;
using BookingsApi.Domain.Participants;

namespace BookingsApi.DAL.Commands
{
    public class CreateVideoHearingCommand : ICommand
    {
        public CreateVideoHearingCommand(CreateVideoHearingRequiredDto requiredDto, CreateVideoHearingOptionalDto optionalDto)
        {
            CaseType = requiredDto.CaseType;
            ScheduledDateTime = requiredDto.ScheduledDateTime;
            ScheduledDuration = requiredDto.ScheduledDuration;
            Venue = requiredDto.Venue;
            Cases = requiredDto.Cases;
            
            HearingType = optionalDto.HearingType;
            Participants = optionalDto.Participants ?? new List<NewParticipant>();
            HearingRoomName = optionalDto.HearingRoomName;
            OtherInformation = optionalDto.OtherInformation;
            CreatedBy = optionalDto.CreatedBy;
            
            AudioRecordingRequired = optionalDto.AudioRecordingRequired;
            Endpoints = optionalDto.Endpoints;
            CancelReason = optionalDto.CancelReason;
            
            LinkedParticipants = optionalDto.LinkedParticipants ?? new List<LinkedParticipantDto>();
            JudiciaryParticipants = optionalDto.JudiciaryParticipants ?? new List<NewJudiciaryParticipant>();
            IsMultiDayFirstHearing = optionalDto.IsMultiDayFirstHearing;

            SourceId = optionalDto.SourceId;
        }

        public Guid NewHearingId { get; set; }
        public CaseType CaseType { get; }
        public HearingType HearingType { get; }
        public DateTime ScheduledDateTime { get; }
        public int ScheduledDuration { get; }
        public HearingVenue Venue { get; }
        public List<NewParticipant> Participants { get; }
        public List<Case> Cases { get; }
        public string HearingRoomName { get; }
        public string OtherInformation { get; }
        public string CreatedBy { get; }
        public bool AudioRecordingRequired { get; }
        public List<NewEndpointDto> Endpoints { get; }
        public string CancelReason { get; }
        public Guid? SourceId { get; }
        public List<LinkedParticipantDto> LinkedParticipants { get; }
        public List<NewJudiciaryParticipant> JudiciaryParticipants { get; }
        public bool IsMultiDayFirstHearing { get; }
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
            var videoHearing = new VideoHearing(command.CaseType, 
                command.HearingType,
                command.ScheduledDateTime,
                command.ScheduledDuration, 
                command.Venue, 
                command.HearingRoomName,
                command.OtherInformation, 
                command.CreatedBy, 
                command.AudioRecordingRequired, 
                command.CancelReason);

            // Ideally, the domain object would implement the clone method and so this change is a work around.
            videoHearing.IsFirstDayOfMultiDayHearing = command.IsMultiDayFirstHearing;
            // denotes this hearing is cloned
            if (command.SourceId.HasValue)
                videoHearing.SourceId = command.SourceId;

            await _context.VideoHearings.AddAsync(videoHearing);
            
            var participants = await _hearingService.AddParticipantToService(videoHearing, command.Participants);

            await _hearingService.CreateParticipantLinks(participants, command.LinkedParticipants);

            foreach (var newJudiciaryParticipant in command.JudiciaryParticipants)
                await _hearingService.AddJudiciaryParticipantToVideoHearing(videoHearing, newJudiciaryParticipant);
            
            videoHearing.AddCases(command.Cases);

            if (command.Endpoints != null && command.Endpoints.Count > 0)
            {
                var dtos = command.Endpoints;
                var newEndpoints = new List<Endpoint>();
                foreach (var dto in dtos)
                {
                    var endpoint = new Endpoint(dto.DisplayName, dto.Sip, dto.Pin);
                    
                    foreach (var endpointParticipant in dto.EndpointParticipants)
                    {
                        var participant = participants.SingleOrDefault(x => x.Person.ContactEmail == endpointParticipant.ContactEmail);
                        endpoint.LinkParticipantToEndpoint(participant, endpointParticipant.Type);
                    }
                    
                    newEndpoints.Add(endpoint);
                }

                videoHearing.AddEndpoints(newEndpoints);
            }
            
            videoHearing.UpdateBookingStatusJudgeRequirement();
            await _context.SaveChangesAsync();
            command.NewHearingId = videoHearing.Id;
        }
    }
}