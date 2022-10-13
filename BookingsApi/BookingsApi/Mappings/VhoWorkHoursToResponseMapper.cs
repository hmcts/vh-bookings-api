using System.Collections.Generic;
using System.Linq;
using BookingsApi.Contract.Responses;
using BookingsApi.Domain;

namespace BookingsApi.Mappings
{
    public static class VhoSearchResponseMapper
    {
        public static VhoSearchResponse Map(List<VhoWorkHours> vhoWorkHours)
        {
            return new VhoSearchResponse
            {
                Username = vhoWorkHours.First().JusticeUser?.Username,
                VhoWorkHours = vhoWorkHours.Select(vwh => new VhoWorkHoursResponse
                {
                    DayOfWeekId = vwh.DayOfWeekId,
                    StartTime = vwh.StartTime,
                    EndTime = vwh.EndTime,
                    DayOfWeek = vwh.DayOfWeek?.Day
                }).ToList()
            };
        }
    }
}
