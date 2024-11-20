using BookingsApi.DAL.Dtos;
using BookingsApi.Common.Helpers;
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
            
            Participants = optionalDto.Participants ?? new List<NewParticipant>();
            HearingRoomName = optionalDto.HearingRoomName;
            OtherInformation = optionalDto.OtherInformation;
            CreatedBy = optionalDto.CreatedBy;
            
            AudioRecordingRequired = optionalDto.AudioRecordingRequired;
            Endpoints = optionalDto.Endpoints;
            
            LinkedParticipants = optionalDto.LinkedParticipants ?? new List<LinkedParticipantDto>();
            JudiciaryParticipants = optionalDto.JudiciaryParticipants ?? new List<NewJudiciaryParticipant>();
            IsMultiDayFirstHearing = optionalDto.IsMultiDayFirstHearing;

            SourceId = optionalDto.SourceId;
        }
        
        public Guid NewHearingId { get; set; }
        public CaseType CaseType { get; }
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
        public Guid? SourceId { get; }
        public List<LinkedParticipantDto> LinkedParticipants { get; }
        public List<NewJudiciaryParticipant> JudiciaryParticipants { get; }
        public bool IsMultiDayFirstHearing { get; }
        public VideoSupplier? ConferenceSupplier { get; set; }
    }

    public class CreateVideoHearingCommandHandler(BookingsDbContext context, IHearingService hearingService)
        : ICommandHandler<CreateVideoHearingCommand>
    {
        public async Task Handle(CreateVideoHearingCommand command)
        {
            var videoHearing = new VideoHearing(command.CaseType, 
                command.ScheduledDateTime,
                command.ScheduledDuration, 
                command.Venue, 
                command.HearingRoomName,
                command.OtherInformation, 
                command.CreatedBy, 
                command.AudioRecordingRequired)
            {
                // Ideally, the domain object would implement the clone method and so this change is a work around.
                IsFirstDayOfMultiDayHearing = command.IsMultiDayFirstHearing
            };

            // denotes this hearing is cloned
            if (command.SourceId.HasValue)
                videoHearing.SourceId = command.SourceId;

            await context.VideoHearings.AddAsync(videoHearing);
            var languages = await context.InterpreterLanguages.Where(x => x.Live).ToListAsync();
            var participants = await hearingService.AddParticipantToService(videoHearing, command.Participants, languages);

            await hearingService.CreateParticipantLinks(participants, command.LinkedParticipants);

            foreach (var newJudiciaryParticipant in command.JudiciaryParticipants)
                await hearingService.AddJudiciaryParticipantToVideoHearing(videoHearing, newJudiciaryParticipant, languages);
            
            videoHearing.AddCases(command.Cases);

            if (command.Endpoints is { Count: > 0 })
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
                hearingService.UpdateParticipantScreeningRequirement(videoHearing, participant, screeningDto);
            }

            
            foreach (var endpointForScreening in (command.Endpoints ?? []).Where(x=> x.Screening != null))
            {
                var endpoint = videoHearing.GetEndpoints().Single(x=> x.ExternalReferenceId == endpointForScreening.ExternalParticipantId);
                var screeningDto = endpointForScreening.Screening;
                videoHearing.AssignScreeningForEndpoint(endpoint, screeningDto.ScreeningType, screeningDto.ProtectedFrom);
            }
            await context.SaveChangesAsync();
            command.NewHearingId = videoHearing.Id;
        }
    }
}