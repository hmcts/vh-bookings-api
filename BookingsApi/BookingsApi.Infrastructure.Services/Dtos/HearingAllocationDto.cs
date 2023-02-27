using System;

namespace BookingsApi.Infrastructure.Services.Dtos
{
    public class HearingAllocationDto : HearingDto 
    {
        public string JudgeDisplayName { get; set;}
    }
}