using BookingsApi.Domain.Configuration;
using BookingsApi.Domain.RefData;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BookingsApi.Domain
{
    public class JusticeUser : TrackableEntity<Guid>
    {
        public JusticeUser()
        {
            VhoNonAvailability = new List<VhoNonAvailability>();
            VhoWorkHours = new List<VhoWorkHours>();
            Allocations = new List<Allocation>();
        }


        public JusticeUser(string firstName, string lastname, string contactEmail, string username, UserRole userRole) : this()
        {
            FirstName = firstName;
            Lastname = lastname;
            ContactEmail = contactEmail;
            Username = username;
            // UserRole = userRole;
            UserRoleId = userRole.Id;
            CreatedDate = DateTime.UtcNow;
        }

        public string FirstName { get; set; }
        public string Lastname { get; set; }
        public string ContactEmail { get; set; }
        public string Username { get; set; }
        public string Telephone { get; set; }
        public int UserRoleId { get; set; }
        public UserRole UserRole { get; set; }
        public string CreatedBy { get; set; }

        public virtual IList<VhoNonAvailability> VhoNonAvailability { get; protected set; }
        public virtual IList<VhoWorkHours> VhoWorkHours { get; protected set; }
        public virtual IList<Allocation> Allocations { get; protected set; }

        public bool IsAvailable(DateTime startDate, DateTime endDate, AllocateHearingConfiguration configuration)
        {
            return IsDateBetweenWorkingHours(startDate, endDate, configuration) &&
                   !IsDuringNonAvailableHours(startDate, endDate);
        }

        public bool IsDuringNonAvailableHours(DateTime startDate, DateTime endDate)
        {
            var nonAvailabilities = VhoNonAvailability
                .Where(na => na.StartTime <= endDate)
                .Where(na => startDate <= na.EndTime)
                .Where(na => !na.Deleted)
                .ToList();
            
            return nonAvailabilities.Any();
        }

        public bool IsDateBetweenWorkingHours(DateTime startDate, DateTime endDate, AllocateHearingConfiguration configuration)
        {
            var workHours = VhoWorkHours
                .FirstOrDefault(wh => wh.SystemDayOfWeek == startDate.DayOfWeek);
            
            if (workHours == null)
            {
                return false;
            }
            
            var workHourStartTime = workHours.StartTime;
            var workHourEndTime = workHours.EndTime;
            
            if (workHourStartTime < startDate.TimeOfDay && workHourEndTime < startDate.TimeOfDay)
            {
                return false;
            }
            
            if (workHourStartTime > endDate.TimeOfDay && workHourEndTime > endDate.TimeOfDay)
            {
                return false;
            }
            
            return (workHourStartTime <= startDate.TimeOfDay || configuration.AllowHearingToStartBeforeWorkStartTime) && 
                   (workHourEndTime >= endDate.TimeOfDay || configuration.AllowHearingToEndAfterWorkEndTime);
        }
    }
}