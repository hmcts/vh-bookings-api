using BookingsApi.DAL.Dtos;
using BookingsApi.DAL.Helper;
using BookingsApi.DAL.Services;
using BookingsApi.Domain.Extensions;

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
            ConferenceSupplier = requiredDto.Supplier;
            
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
        public List<NewEndpoint> Endpoints { get; }
        public string CancelReason { get; }
        public Guid? SourceId { get; }
        public List<LinkedParticipantDto> LinkedParticipants { get; }
        public List<NewJudiciaryParticipant> JudiciaryParticipants { get; }
        public bool IsMultiDayFirstHearing { get; }
        public VideoSupplier? ConferenceSupplier { get; set; }
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
            var languages = await _context.InterpreterLanguages.Where(x => x.Live).ToListAsync();
            var participants = await _hearingService.AddParticipantToService(videoHearing, command.Participants, languages);

            await _hearingService.CreateParticipantLinks(participants, command.LinkedParticipants);

            foreach (var newJudiciaryParticipant in command.JudiciaryParticipants)
                await _hearingService.AddJudiciaryParticipantToVideoHearing(videoHearing, newJudiciaryParticipant, languages);
            
            videoHearing.AddCases(command.Cases);

            if (command.Endpoints != null && command.Endpoints.Count > 0)
            {
                var dtos = command.Endpoints;
                var newEndpoints = new List<Endpoint>();
                foreach (var dto in dtos)
                {
                    var defenceAdvocate =
                        DefenceAdvocateHelper.CheckAndReturnDefenceAdvocate(dto.ContactEmail,
                            videoHearing.GetParticipants());
                    var endpoint = new Endpoint(dto.ExternalParticipantId, dto.DisplayName, dto.Sip, dto.Pin, defenceAdvocate)
                    {
                        MeasuresExternalId = dto.MeasuresExternalId,
                    };
                    var language = languages.GetLanguage(dto.LanguageCode, "Hearing");
                    endpoint.UpdateLanguagePreferences(language, dto.OtherLanguage);
                    newEndpoints.Add(endpoint);
                }

                videoHearing.AddEndpoints(newEndpoints);
            }
            
            if (command.ConferenceSupplier.HasValue)
            {
                videoHearing.OverrideSupplier(command.ConferenceSupplier.Value);
            }
            videoHearing.UpdateBookingStatusJudgeRequirement();
            
            foreach (var participantForScreening in command.Participants.Where(x=> x.Screening != null))
            {
                var participant = videoHearing.GetParticipants().Single(x=> x.ExternalReferenceId == participantForScreening.ExternalReferenceId);
                var screeningDto = participantForScreening.Screening;
                _hearingService.UpdateParticipantScreeningRequirement(videoHearing, participant, screeningDto);
            }

            
            foreach (var endpointForScreening in (command.Endpoints ?? []).Where(x=> x.Screening != null))
            {
                var endpoint = videoHearing.GetEndpoints().Single(x=> x.ExternalReferenceId == endpointForScreening.ExternalParticipantId);
                var screeningDto = endpointForScreening.Screening;
                videoHearing.AssignScreeningForEndpoint(endpoint, screeningDto.ScreeningType, screeningDto.ProtectedFrom);
            }
            await _context.SaveChangesAsync();
            command.NewHearingId = videoHearing.Id;
        }
    }
}