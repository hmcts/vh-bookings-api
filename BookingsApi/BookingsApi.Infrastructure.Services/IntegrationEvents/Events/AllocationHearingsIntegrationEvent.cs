﻿using System;
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
            Hearings = hearings.Select(HearingAllocationDtoMapper.MapToDto).ToList();
            AllocatedCso = (allocatedCso != null) ? UserDtoMapper.MapToDto(allocatedCso) : null;
        }

        public List<HearingAllocationDto> Hearings { get; }
        public UserDto AllocatedCso { get; }
    }
}