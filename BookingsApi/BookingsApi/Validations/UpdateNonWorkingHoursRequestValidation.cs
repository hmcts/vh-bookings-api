using System.Collections.Generic;
using System.Linq;
using BookingsApi.Contract.Requests;
using BookingsApi.Domain;
using FluentValidation;
using FluentValidation.Results;

namespace BookingsApi.Validations
{
    public class UpdateNonWorkingHoursRequestValidation : AbstractValidator<UpdateNonWorkingHoursRequest>
    {
        public const string HoursEmptyErrorMessage = "Hours cannot be null or empty";
        public const string HoursOverlapErrorMessage = "Hours cannot overlap for a single user";
        public const string HourIdsNotFoundErrorMessage = "Hour ids not found";
        
        public UpdateNonWorkingHoursRequestValidation()
        {
            RuleFor(x => x.Hours)
                .NotEmpty().WithMessage(HoursEmptyErrorMessage);

            RuleForEach(x => x.Hours)
                .SetValidator(new NonWorkingHoursRequestValidation());
        }

        public ValidationResult ValidateHours(UpdateNonWorkingHoursRequest request, IList<VhoNonAvailability> existingHours)
        {
            var errors = new List<ValidationFailure>();
            
            var requestedWorkHourIds = request.Hours.Select(h => h.Id).ToList();
            var foundWorkHourIds = existingHours.Select(h => h.Id).ToList();

            var requestedWorkHourIdsFound = requestedWorkHourIds.All(foundWorkHourIds.Contains);

            if (!requestedWorkHourIdsFound)
            {
                errors.Add(new ValidationFailure(nameof(request.Hours), HourIdsNotFoundErrorMessage));
                return new ValidationResult(errors);
            }
            
            var newWorkHours = existingHours.ToList();
            foreach (var newWorkHour in newWorkHours)
            {
                var requestedHour = request.Hours.SingleOrDefault(h => h.Id == newWorkHour.Id);

                newWorkHour.StartTime = requestedHour.StartTime;
                newWorkHour.EndTime = requestedHour.EndTime;
            }

            var datesOverlap = CheckOverlappingDates(newWorkHours);
            if (datesOverlap)
            {
                errors.Add(new ValidationFailure(nameof(request.Hours), HoursOverlapErrorMessage));
            }

            return new ValidationResult(errors);
        }

        private static bool CheckOverlappingDates(IList<VhoNonAvailability> nonWorkingHours)
        {
            var userIds = nonWorkingHours.Select(h => h.JusticeUserId)
                .Distinct()
                .ToList();

            foreach (var userId in userIds)
            {
                var hoursForUser = nonWorkingHours
                    .Where(h => h.JusticeUserId == userId)
                    .OrderBy(h => h.StartTime)
                    .ToList();

                var firstHour = (VhoNonAvailability)null;
                var checkedHours = new List<VhoNonAvailability>();
    
                foreach (var hour in hoursForUser)
                {
                    if (firstHour != null)
                    {
                        checkedHours.Add(firstHour);
                        var uncheckedHours = hoursForUser.Where(x => (x.StartTime >= firstHour.StartTime && x != firstHour) && checkedHours.All(m => m != x));
            
                        if (uncheckedHours.Any(uncheckedHour => OverlapsWith(firstHour, uncheckedHour)))
                        {
                            return true;
                        }
                    }
                    firstHour = hour;
                }
            }

            return false;
        }
        
        private static bool OverlapsWith(VhoNonAvailability first, VhoNonAvailability second)
        {
            return first.EndTime > second.StartTime;
        }
    }
}
