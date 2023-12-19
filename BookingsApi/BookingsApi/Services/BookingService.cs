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
    /// <returns></returns>
    Task PublishMultiDayHearing(VideoHearing videoHearing, int totalDays);

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
}

public class BookingService : IBookingService
{
    private readonly IEventPublisher _eventPublisher;
    private readonly ICommandHandler _commandHandler;
    private readonly IQueryHandler _queryHandler;
    private readonly IBookingAsynchronousProcess _bookingAsynchronousProcess;
    private readonly IFirstdayOfMultidayBookingAsynchronousProcess _firstdayOfMultidayBookingAsyncProcess;
    private readonly IClonedBookingAsynchronousProcess _clonedBookingAsynchronousProcess;
    public BookingService(IEventPublisher eventPublisher, ICommandHandler commandHandler, IQueryHandler queryHandler,
        IBookingAsynchronousProcess bookingAsynchronousProcess,
        IFirstdayOfMultidayBookingAsynchronousProcess firstdayOfMultidayBookingAsyncProcess,
        IClonedBookingAsynchronousProcess clonedBookingAsynchronousProcess)
    {
        _eventPublisher = eventPublisher;
        _commandHandler = commandHandler;
        _queryHandler = queryHandler;
        _bookingAsynchronousProcess = bookingAsynchronousProcess;
        _firstdayOfMultidayBookingAsyncProcess = firstdayOfMultidayBookingAsyncProcess;
        _clonedBookingAsynchronousProcess = clonedBookingAsynchronousProcess;
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

    public async Task PublishMultiDayHearing(VideoHearing videoHearing, int totalDays)
    {
        await _clonedBookingAsynchronousProcess.Start(videoHearing, totalDays);
    }
    
    public async Task PublishHearingCancelled(VideoHearing videoHearing)
    {
        if (videoHearing.Status == BookingStatus.Created)
        {
            // publish the event only for confirmed(created) hearing  
            await _eventPublisher.PublishAsync(new HearingCancelledIntegrationEvent(videoHearing.Id));
        }
    }

    public async Task<VideoHearing> UpdateHearingAndPublish(UpdateHearingCommand updateHearingCommand, VideoHearing originalHearing)
    {
        await _commandHandler.Handle(updateHearingCommand);
        var updatedHearing = await _queryHandler.Handle<GetHearingByIdQuery, VideoHearing>(new GetHearingByIdQuery(originalHearing.Id));
        if (updatedHearing.Status != BookingStatus.Created) return updatedHearing;

        await _eventPublisher.PublishAsync(new HearingDetailsUpdatedIntegrationEvent(updatedHearing));

        if (updatedHearing.ScheduledDateTime.Ticks != originalHearing.ScheduledDateTime.Ticks)
        {
            var @case = originalHearing.GetCases()[0];
            foreach (var participant in originalHearing.Participants)
            {
                var participantDto = ParticipantDtoMapper.MapToDto(participant, originalHearing.OtherInformation);
                
                await _eventPublisher.PublishAsync(new HearingAmendmentNotificationEvent(EventDtoMappers.MapToHearingConfirmationDto(originalHearing.Id, 
                        originalHearing.ScheduledDateTime, participantDto, @case),  updatedHearing.ScheduledDateTime));
            }
        }

        return updatedHearing;
    }
}