using BookingsApi.Contract.Responses;
using BookingsApi.DAL.Dtos;

namespace BookingsApi.Mappings;

public static class HearingAllocationResultDtoToAllocationResponseMapper
{
    public static HearingAllocationsResponse Map(HearingAllocationResultDto dto)
    {
        return new HearingAllocationsResponse
        {
            HearingId = dto.HearingId,
            Duration = dto.Duration,
            AllocatedCso = dto.AllocatedCso,
            CaseType = dto.CaseType,
            CaseNumber = dto.CaseNumber,
            ScheduledDateTime = dto.ScheduledDateTime,
            HasWorkHoursClash = dto.HasWorkHoursClash,
            ConcurrentHearingsCount = dto.ConcurrentHearingsCount
        };
    }
}