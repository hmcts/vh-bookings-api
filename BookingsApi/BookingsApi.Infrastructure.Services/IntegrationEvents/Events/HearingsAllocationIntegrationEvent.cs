using System.Collections.Generic;
using System.Linq;
using BookingsApi.Domain;
using BookingsApi.Infrastructure.Services.Dtos;

namespace BookingsApi.Infrastructure.Services.IntegrationEvents.Events
{
    public class HearingsAllocationIntegrationEvent : IIntegrationEvent
    {
        public List<HearingDto> Hearings { get; }
        public JusticeUserDto AllocatedCso { get; }
        
        public HearingsAllocationIntegrationEvent(List<VideoHearing> hearings, JusticeUser allocatedCso)
        {
            Hearings = hearings.Select(HearingDtoMapper.MapToDto).ToList();
            AllocatedCso = JusticeUserDtoMapper.MapToDto(allocatedCso);
        }
    }
}