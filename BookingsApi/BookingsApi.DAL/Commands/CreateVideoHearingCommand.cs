using BookingsApi.DAL.Dtos;
using BookingsApi.DAL.Helper;
using BookingsApi.DAL.Services;
using BookingsApi.Domain.Extensions;
using Microsoft.IdentityModel.Tokens;

namespace BookingsApi.DAL.Commands
{
    public class CreateVideoHearingCommand(
        CreateVideoHearingRequiredDto requiredDto,
        CreateVideoHearingOptionalDto optionalDto)
        : ICommand
    {
        public Guid NewHearingId { get; set; }
        public CaseType CaseType { get; } = requiredDto.CaseType;
        public DateTime ScheduledDateTime { get; } = requiredDto.ScheduledDateTime;
        public int ScheduledDuration { get; } = requiredDto.ScheduledDuration;
        public HearingVenue Venue { get; } = requiredDto.Venue;
        public List<NewParticipant> Participants { get; } = optionalDto.Participants ?? new List<NewParticipant>();
        public List<Case> Cases { get; } = requiredDto.Cases;
        public string HearingRoomName { get; } = optionalDto.HearingRoomName;
        public string OtherInformation { get; } = optionalDto.OtherInformation;
        public string CreatedBy { get; } = optionalDto.CreatedBy;
        public bool AudioRecordingRequired { get; } = optionalDto.AudioRecordingRequired;
        public List<NewEndpoint> Endpoints { get; } = optionalDto.Endpoints;
        public Guid? SourceId { get; } = optionalDto.SourceId;
        public List<LinkedParticipantDto> LinkedParticipants { get; } = optionalDto.LinkedParticipants ?? new List<LinkedParticipantDto>();
        public List<NewJudiciaryParticipant> JudiciaryParticipants { get; } = optionalDto.JudiciaryParticipants ?? new List<NewJudiciaryParticipant>();
        public bool IsMultiDayFirstHearing { get; } = optionalDto.IsMultiDayFirstHearing;
        public VideoSupplier? ConferenceSupplier { get; set; } = requiredDto.Supplier;
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
            await context.Entry(videoHearing).Reference(x => x.CaseType).LoadAsync();
            var languages = await context.InterpreterLanguages.Where(x => x.Live).ToListAsync();
            var participants = await hearingService.AddParticipantToService(videoHearing, command.Participants, languages);

            await hearingService.CreateParticipantLinks(participants, command.LinkedParticipants);

            foreach (var newJudiciaryParticipant in command.JudiciaryParticipants)
                await hearingService.AddJudiciaryParticipantToVideoHearing(videoHearing, newJudiciaryParticipant, languages);
            
            videoHearing.AddCases(command.Cases);

            if (command.Endpoints.Count != 0)
                AddEndpointToHearing(command, videoHearing, languages);
            
            if (command.ConferenceSupplier.HasValue)
                videoHearing.OverrideSupplier(command.ConferenceSupplier.Value);
            
            videoHearing.UpdateBookingStatusJudgeRequirement();
            
            AddScreenings(command, videoHearing);
            await context.SaveChangesAsync();
            command.NewHearingId = videoHearing.Id;
        }

        private void AddScreenings(CreateVideoHearingCommand command, VideoHearing videoHearing)
        {
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
        }

        private static void AddEndpointToHearing(CreateVideoHearingCommand command, VideoHearing videoHearing, List<InterpreterLanguage> languages)
        {
            var newEndpoints = new List<Endpoint>();
            foreach (var dto in command.Endpoints)
            {
                var endpoint = new Endpoint(dto.ExternalParticipantId, dto.DisplayName, dto.Sip, dto.Pin);

                if (!dto.LinkedParticipantEmails.IsNullOrEmpty())
                {
                    var participantsLinked =
                        EndpointParticipantHelper.CheckAndReturnParticipantsLinkedToEndpoint(dto.LinkedParticipantEmails, videoHearing.GetParticipants().ToList());
                
                    foreach (var participant in participantsLinked)
                        endpoint.AddLinkedParticipant(participant);
                }
                
                endpoint.UpdateExternalIds(dto.ExternalParticipantId, dto.MeasuresExternalId);
                var language = languages.GetLanguage(dto.LanguageCode, "Hearing");
                endpoint.UpdateLanguagePreferences(language, dto.OtherLanguage);
                newEndpoints.Add(endpoint);
            }

            videoHearing.AddEndpoints(newEndpoints);
        }
    }
}