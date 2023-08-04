using BookingsApi.Contract.V1.Responses;

namespace BookingsApi.Mappings.V1
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
