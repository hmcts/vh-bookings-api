using BookingsApi.Contract.V1.Responses;

namespace BookingsApi.Mappings.V1
{
    public static class VhoNonAvailabilityWorkHoursResponseMapper
    {
        public static List<VhoNonAvailabilityWorkHoursResponse> Map(List<VhoNonAvailability> vhoWorkHours)
        {
            return vhoWorkHours.Select(vna => new VhoNonAvailabilityWorkHoursResponse
            {
                Id = vna.Id,
                StartTime = vna.StartTime,
                EndTime = vna.EndTime
            }).ToList();
        }
    }
}
