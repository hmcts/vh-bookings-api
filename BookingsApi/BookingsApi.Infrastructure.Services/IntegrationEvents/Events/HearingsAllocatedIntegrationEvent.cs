using System.Collections.Generic;
using System.Linq;
using BookingsApi.Domain;
using BookingsApi.Infrastructure.Services.Dtos;

namespace BookingsApi.Infrastructure.Services.IntegrationEvents.Events
{
    public class HearingsAllocatedIntegrationEvent(List<VideoHearing> hearings, JusticeUser allocatedCso)
        : IIntegrationEvent
    {
        public List<HearingDto> Hearings { get; } = hearings.Select(HearingDtoMapper.MapToDto).ToList();

        public JusticeUserDto AllocatedCso { get; } =
            (allocatedCso != null) ? JusticeUserDtoMapper.MapToDto(allocatedCso) : null;
    }
}