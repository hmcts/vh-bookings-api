using BookingsApi.Infrastructure.Services;
using BookingsApi.Infrastructure.Services.AsynchronousProcesses;

namespace BookingsApi.Services;

/// <summary>
/// Represents common actions between routes and controllers across versions.
/// Name subject to change once we have a better idea of what this class will do.
/// </summary>
public interface IBookingService
{
    /// <summary>
    /// Save a new hearing, and return the saved hearing.
    /// </summary>
    /// <param name="command"></param>
    /// <returns></returns>
    Task<VideoHearing> SaveNewHearing(CreateVideoHearingCommand command);
    
    /// <summary>
    /// Save a new hearing, publish a message to the event bus, and return the saved hearing.
    /// </summary>
    /// <param name="command"></param>
    /// <param name="isMultiDay"></param>
    /// <returns></returns>
    Task<VideoHearing> SaveNewHearingAndPublish(CreateVideoHearingCommand command, bool isMultiDay);
    
    /// <summary>
    /// Send a message to the service bus to publish the booking of a new hearing
    /// </summary>
    /// <param name="videoHearing"></param>
    /// <param name="isMultiDay"></param>
    /// <returns></returns>
    Task PublishNewHearing(VideoHearing videoHearing, bool isMultiDay);
    
    /// <summary>
    /// Send a message to the service bus to publish the booking of a new multi-day hearing
    /// </summary>
    /// <param name="videoHearing"></param>
    /// <param name="totalDays"></param>
    /// <param name="videoHearingUpdateDate"></param>
    /// <returns></returns>
    Task PublishMultiDayHearing(VideoHearing videoHearing, int totalDays, DateTime videoHearingUpdateDate);

    /// <summary>
    /// Send a message to the service bus to publish the booking of edit a new multi-day hearing
    /// </summary>
    /// <param name="videoHearing"></param>
    /// <param name="totalDays"></param>
    /// <param name="videoHearingUpdateDate"></param>
    /// <returns></returns>
    Task PublishEditMultiDayHearing(VideoHearing videoHearing, int totalDays, DateTime videoHearingUpdateDate);

    /// <summary>
    /// Send a message to the service bus to publish the update of a hearing
    /// </summary>
    /// <param name="updateHearingCommand"></param>
    /// <param name="originalHearing"></param>
    /// <returns></returns>
    Task<VideoHearing> UpdateHearingAndPublish(UpdateHearingCommand updateHearingCommand, VideoHearing originalHearing);

    /// <summary>
    /// Send a message to the service bus to publish the cancellation of a hearing
    /// </summary>
    /// <param name="videoHearing"></param>
    /// <returns></returns>
    Task PublishHearingCancelled(VideoHearing videoHearing);
    
    /// <summary>
    /// Get a Hearing by Id
    /// </summary>
    /// <param name="hearingId"></param>
    /// <returns>Hearing</returns>
    Task<VideoHearing> GetHearingById(Guid hearingId);

    /// <summary>
    /// Update status of a hearing and publish a message to the service bus
    /// </summary>
    /// <param name="videoHearing"></param>
    /// <param name="status"></param>
    /// <param name="updatedBy"></param>
    /// <param name="reason"></param>
    /// <returns></returns>
    Task UpdateHearingStatus(VideoHearing videoHearing, BookingStatus status, string updatedBy, string reason);
}

public class BookingService : IBookingService
{
    private readonly IEventPublisher _eventPublisher;
    private readonly ICommandHandler _commandHandler;
    private readonly IQueryHandler _queryHandler;
    private readonly IBookingAsynchronousProcess _bookingAsynchronousProcess;
    private readonly IFirstdayOfMultidayBookingAsynchronousProcess _firstdayOfMultidayBookingAsyncProcess;
    private readonly IClonedBookingAsynchronousProcess _clonedBookingAsynchronousProcess;
    private readonly ICreateConferenceAsynchronousProcess _createConferenceAsynchronousProcess;
    private readonly IFeatureToggles _featureToggles;
        
    public BookingService(IEventPublisher eventPublisher, ICommandHandler commandHandler, IQueryHandler queryHandler,
        IBookingAsynchronousProcess bookingAsynchronousProcess,
        IFirstdayOfMultidayBookingAsynchronousProcess firstdayOfMultidayBookingAsyncProcess,
        IClonedBookingAsynchronousProcess clonedBookingAsynchronousProcess, ICreateConferenceAsynchronousProcess createConferenceAsynchronousProcess,
        IFeatureToggles featureToggles)
    {
        _eventPublisher = eventPublisher;
        _commandHandler = commandHandler;
        _queryHandler = queryHandler;
        _bookingAsynchronousProcess = bookingAsynchronousProcess;
        _firstdayOfMultidayBookingAsyncProcess = firstdayOfMultidayBookingAsyncProcess;
        _clonedBookingAsynchronousProcess = clonedBookingAsynchronousProcess;
        _createConferenceAsynchronousProcess = createConferenceAsynchronousProcess;
        _featureToggles = featureToggles;
    }

    public async Task<VideoHearing> SaveNewHearing(CreateVideoHearingCommand command)
    {
        await _commandHandler.Handle(command);
        
        var getHearingByIdQuery = new GetHearingByIdQuery(command.NewHearingId);
        var queriedVideoHearing = await _queryHandler.Handle<GetHearingByIdQuery, VideoHearing>(getHearingByIdQuery);

        await _createConferenceAsynchronousProcess.Start(queriedVideoHearing);
        
        return queriedVideoHearing;
    }
    
    public async Task<VideoHearing> SaveNewHearingAndPublish(CreateVideoHearingCommand command, bool isMultiDay)
    {
        await _commandHandler.Handle(command);
        
        var getHearingByIdQuery = new GetHearingByIdQuery(command.NewHearingId);
        var queriedVideoHearing = await _queryHandler.Handle<GetHearingByIdQuery, VideoHearing>(getHearingByIdQuery);
        await PublishNewHearing(queriedVideoHearing, isMultiDay);

        return queriedVideoHearing;
    }
    
    public async Task PublishNewHearing(VideoHearing videoHearing, bool isMultiDay)
    {
        if (isMultiDay)
        {
            await _firstdayOfMultidayBookingAsyncProcess.Start(videoHearing);
        }
        else 
        {
            await _bookingAsynchronousProcess.Start(videoHearing);
        }
    }

    public async Task PublishMultiDayHearing(VideoHearing videoHearing, int totalDays, DateTime videoHearingUpdateDate)
    {
        await _clonedBookingAsynchronousProcess.Start(videoHearing, totalDays, videoHearingUpdateDate);
    }
    
    public async Task PublishEditMultiDayHearing(VideoHearing videoHearing, int totalDays, DateTime videoHearingUpdateDate)
    {
        await _clonedBookingAsynchronousProcess.Start(videoHearing, totalDays, videoHearingUpdateDate, true);
    }
    
    public async Task PublishHearingCancelled(VideoHearing videoHearing)
    {
        if (videoHearing.Status == BookingStatus.Created)
        {
            // publish the event only for confirmed(created) hearing  
            await _eventPublisher.PublishAsync(new HearingCancelledIntegrationEvent(videoHearing.Id));
        }
    }

    public async Task<VideoHearing> GetHearingById(Guid hearingId)
    {
        var query = new GetHearingByIdQuery(hearingId);
        var hearing = await _queryHandler.Handle<GetHearingByIdQuery, VideoHearing>(query);

        return hearing;
    }

    public async Task<VideoHearing> UpdateHearingAndPublish(UpdateHearingCommand updateHearingCommand, VideoHearing originalHearing)
    {
        if (_featureToggles.MultiDayBookingEnhancementsEnabled() && originalHearing.SourceId != null)
        {
            await ValidateScheduleUpdate(updateHearingCommand.ScheduledDateTime, originalHearing.SourceId.Value);
        }
        
        await _commandHandler.Handle(updateHearingCommand);
        var updatedHearing = await _queryHandler.Handle<GetHearingByIdQuery, VideoHearing>(new GetHearingByIdQuery(originalHearing.Id));
        if (updatedHearing.Status is not BookingStatus.Created and not BookingStatus.ConfirmedWithoutJudge) return updatedHearing;

        await _eventPublisher.PublishAsync(new HearingDetailsUpdatedIntegrationEvent(updatedHearing));

        await PublishHearingUpdateNotificationToParticipants(originalHearing, updatedHearing);

        return updatedHearing;
    }

    public async Task UpdateHearingStatus(VideoHearing videoHearing, BookingStatus status, string updatedBy, string reason)
    {
        var command = new UpdateHearingStatusCommand(videoHearing.Id, status, updatedBy, reason);
        await _commandHandler.Handle(command);

        if (status == BookingStatus.Cancelled) 
            await PublishHearingCancelled(videoHearing);
    }

    private async Task PublishHearingUpdateNotificationToParticipants(VideoHearing originalHearing, VideoHearing updatedHearing)
    {
        if (updatedHearing.ScheduledDateTime.Ticks != originalHearing.ScheduledDateTime.Ticks)
        {
            var @case = originalHearing.GetCases()[0];
            foreach (var participant in originalHearing.Participants)
            {
                var participantDto = ParticipantDtoMapper.MapToDto(participant, originalHearing.OtherInformation);
                await _eventPublisher.PublishAsync(new HearingAmendmentNotificationEvent(EventDtoMappers.MapToHearingConfirmationDto(originalHearing.Id, 
                    originalHearing.ScheduledDateTime, participantDto, @case),  updatedHearing.ScheduledDateTime));
            }
            foreach (var judiciaryParticipant in originalHearing.JudiciaryParticipants)
            {
                var participantDto = ParticipantDtoMapper.MapToDto(judiciaryParticipant);
                await _eventPublisher.PublishAsync(new HearingAmendmentNotificationEvent(EventDtoMappers.MapToHearingConfirmationDto(originalHearing.Id, 
                    originalHearing.ScheduledDateTime, participantDto, @case),  updatedHearing.ScheduledDateTime));
            }
        }
    }

    private async Task ValidateScheduleUpdate(DateTime newScheduledDateTime, Guid sourceId)
    {
        var hearingsInGroupQuery = new GetHearingsByGroupIdQuery(sourceId);
        var hearingsInGroup = await _queryHandler.Handle<GetHearingsByGroupIdQuery, List<VideoHearing>>(hearingsInGroupQuery);
        
        if (hearingsInGroup.Exists(h => h.ScheduledDateTime.Date == newScheduledDateTime.Date))
        {
            throw new DomainRuleException("ScheduledDateTime", 
                DomainRuleErrorMessages.CannotBeOnSameDateAsOtherHearingInGroup);
        }
    }
}