using BookingsApi.Domain.Configuration;
using BookingsApi.Domain.Extensions;
using BookingsApi.Domain.RefData;
using System;
using System.Collections.Generic;
using System.Linq;
using BookingsApi.Domain.Validations;

namespace BookingsApi.Domain
{
    public class JusticeUser : TrackableEntity<Guid>
    {
        public JusticeUser()
        {
            Id = Guid.NewGuid();
            VhoNonAvailability = new List<VhoNonAvailability>();
            VhoWorkHours = new List<VhoWorkHours>();
            Allocations = new List<Allocation>();
            JusticeUserRoles = new List<JusticeUserRole>();
        }


        public JusticeUser(string firstName, string lastname, string contactEmail, string username) : this()
        {
            FirstName = firstName;
            Lastname = lastname;
            ContactEmail = contactEmail;
            Username = username;
        }

        public string FirstName { get; set; }
        public string Lastname { get; set; }
        public string ContactEmail { get; set; }
        public string Username { get; set; }
        public string Telephone { get; set; }
        public int? UserRoleId { get; set; }
        public string CreatedBy { get; set; }
        public bool Deleted { get; private set; }
        public virtual IList<JusticeUserRole> JusticeUserRoles { get; set; }
        public virtual IList<VhoNonAvailability> VhoNonAvailability { get; protected set; }
        public virtual IList<VhoWorkHours> VhoWorkHours { get; protected set; }
        public virtual IList<Allocation> Allocations { get; protected set; }

        public void AddOrUpdateWorkHour(DayOfWeek dayOfWeek, TimeSpan startTime, TimeSpan endTime)
        {
            var existingHour = VhoWorkHours.SingleOrDefault(hours => !hours.Deleted && hours.DayOfWeekId == dayOfWeek.Id);
            if (existingHour == null)
            {
                VhoWorkHours.Add(new VhoWorkHours()
                {
                    DayOfWeek = dayOfWeek,
                    StartTime = startTime,
                    EndTime = endTime,
                    JusticeUser = this
                });
            }
            else
            {
                existingHour.StartTime = startTime;
                existingHour.EndTime = endTime;
                UpdatedDate = DateTime.UtcNow;
            }
        }

        public void AddOrUpdateNonAvailability(DateTime startTime, DateTime endTime)
        {
            var vhoNonWorkingHours =
                VhoNonAvailability.SingleOrDefault(x => x.StartTime == startTime || x.EndTime == endTime);

            if (vhoNonWorkingHours == null)
            {
                VhoNonAvailability.Add(new VhoNonAvailability()
                {
                    StartTime = startTime,
                    EndTime = endTime,
                    JusticeUser = this
                });
            }
            else
            {
                vhoNonWorkingHours.StartTime = startTime;
                vhoNonWorkingHours.EndTime = endTime;
                vhoNonWorkingHours.Deleted = false;
                // should we undelete if being re-added?
            }
        }

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
            var localStartDate = startDate.ToGmt();
            var localEndDate = endDate.ToGmt();

            var workHours = VhoWorkHours.FirstOrDefault(wh => wh.SystemDayOfWeek == localStartDate.DayOfWeek);
            
            if (workHours == null)
            {
                return false;
            }
            
            var workHourStartTime = workHours.StartTime;
            var workHourEndTime = workHours.EndTime;
            
            if (workHourStartTime < localStartDate.TimeOfDay && workHourEndTime < localStartDate.TimeOfDay)
            {
                return false;
            }
            
            if (workHourStartTime > localEndDate.TimeOfDay && workHourEndTime > localEndDate.TimeOfDay)
            {
                return false;
            }
            
            return (workHourStartTime <= localStartDate.TimeOfDay || configuration.AllowHearingToStartBeforeWorkStartTime) && 
                   (workHourEndTime >= localEndDate.TimeOfDay || configuration.AllowHearingToEndAfterWorkEndTime);
        }
        
        public void AddRoles(params UserRole[] userRoles)
        {
            foreach (var userRole in userRoles)
            {
                JusticeUserRoles.Add(new JusticeUserRole(this, userRole));
            }
        }

        public void Delete()
        {
            Deleted = true;
            VhoWorkHours.Clear();
            VhoNonAvailability.Clear();
            
            foreach (var hearing in Allocations.Select(a => a.Hearing))
            {
                hearing.Deallocate();
            }
        }

        public void Restore()
        {
            Deleted = false;
        }
        
        public bool IsTeamLeader() => JusticeUserRoles.Any(jur => jur.UserRole.IsVhTeamLead);

        public void RemoveNonAvailability(VhoNonAvailability nonAvailability)
        {
            var existing = VhoNonAvailability.SingleOrDefault(x => x.Id == nonAvailability.Id);
            if (existing == null)
            {
                throw new DomainRuleException("JusticeUser", "NonAvailability does not exist");
            }
            VhoNonAvailability.Remove(nonAvailability);
        }
    }
}