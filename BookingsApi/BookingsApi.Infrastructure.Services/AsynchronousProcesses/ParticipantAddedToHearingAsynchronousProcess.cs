﻿using BookingsApi.Common.Services;
using BookingsApi.Domain;
using BookingsApi.Infrastructure.Services.Publishers;
using System.Threading.Tasks;

namespace BookingsApi.Infrastructure.Services.AsynchronousProcesses
{
    public interface IParticipantAddedToHearingAsynchronousProcess
    {
        Task Start(VideoHearing videoHearing);
    }

    public class ParticipantAddedToHearingAsynchronousProcess : IParticipantAddedToHearingAsynchronousProcess
    {
        private readonly IEventPublisherFactory _publisherFactory;

        public ParticipantAddedToHearingAsynchronousProcess(IEventPublisherFactory publisherFactory)
        {
            _publisherFactory = publisherFactory;
        }

        public async Task Start(VideoHearing videoHearing)
        {
            await _publisherFactory.Get(EventType.NewParticipantWelcomeEmailEvent).PublishAsync(videoHearing);
            await _publisherFactory.Get(EventType.NewParticipantHearingConfirmationEvent).PublishAsync(videoHearing);
            await _publisherFactory.Get(EventType.ExistingParticipantHearingConfirmationEvent).PublishAsync(videoHearing);
        }
    }
}
