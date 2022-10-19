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
            var justiceUser = vhoWorkHours.First().JusticeUser;
            return new VhoSearchResponse
            {
                Username = justiceUser?.Username,
                FirstName = justiceUser?.FirstName,
                Lastname = justiceUser?.Lastname,
                UserRole = justiceUser?.UserRole?.Name,
                ContactEmail = justiceUser?.ContactEmail,
                Telephone = justiceUser?.Telephone,
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
