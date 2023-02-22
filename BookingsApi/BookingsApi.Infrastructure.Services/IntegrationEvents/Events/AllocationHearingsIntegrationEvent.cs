using System;
using System.Collections.Generic;
using System.Linq;
using BookingsApi.Domain;
using BookingsApi.Domain.Participants;
using BookingsApi.Infrastructure.Services.Dtos;

namespace BookingsApi.Infrastructure.Services.IntegrationEvents.Events
{
    public class AllocationHearingsIntegrationEvent: IIntegrationEvent
    {
        public AllocationHearingsIntegrationEvent(List<VideoHearing> hearings, JusticeUser allocatedCso)
        {
            Hearings = hearings.Select(HearingDtoMapper.MapToDto).ToList();
            AllocatedCso = UserDtoMapper.MapToDto(allocatedCso);
        }

        public List<HearingDto> Hearings { get; }

        public UserDto AllocatedCso { get; }
    }
}