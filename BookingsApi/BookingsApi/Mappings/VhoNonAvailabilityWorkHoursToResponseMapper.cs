using System.Collections.Generic;
using System.Linq;
using BookingsApi.Contract.Responses;
using BookingsApi.Domain;

namespace BookingsApi.Mappings
{
    public static class VhoNonAvailabilityWorkHoursResponseMapper
    {
        public static List<VhoNonAvailabilityWorkHoursResponse> Map(List<VhoNonAvailability> vhoWorkHours)
        {
            return vhoWorkHours.Select(vna => new VhoNonAvailabilityWorkHoursResponse
            {
                StartTime = vna.StartTime,
                EndTime = vna.EndTime
            }).ToList();
        }
    }
}
