using System.Collections.Generic;
using System.Linq;
using BookingsApi.Contract.V1.Responses;
using BookingsApi.Domain;

namespace BookingsApi.Mappings
{
    public static class VhoWorkHoursToResponseMapper
    {
        public static List<VhoWorkHoursResponse> Map(List<VhoWorkHours> vhoWorkHours)
        {
            return vhoWorkHours.Select(vwh => new VhoWorkHoursResponse
            {
                DayOfWeekId = vwh.DayOfWeekId,
                StartTime = vwh.StartTime,
                EndTime = vwh.EndTime,
                DayOfWeek = vwh.DayOfWeek?.Day
            }).ToList();
        }
    }
}
