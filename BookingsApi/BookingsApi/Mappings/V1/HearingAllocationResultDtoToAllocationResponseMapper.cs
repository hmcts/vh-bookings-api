using BookingsApi.Contract.V1.Responses;

namespace BookingsApi.Mappings.V1;

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
            ConcurrentHearingsCount = dto.ConcurrentHearingsCount,
            HasNonAvailabilityClash = dto.HasNonAvailabilityClash
        };
    }
}