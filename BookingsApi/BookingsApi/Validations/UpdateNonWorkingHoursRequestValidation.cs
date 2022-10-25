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

        private bool CheckOverlappingDates(IList<VhoNonAvailability> nonWorkingHours)
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

                var first = (VhoNonAvailability)null;
                var checkedHours = new List<VhoNonAvailability>();
    
                foreach (var hour in hoursForUser)
                {
                    if (first != null)
                    {
                        checkedHours.Add(first);
                        var uncheckedHours = hoursForUser.Where(x => (x.StartTime >= first.StartTime && !(x == first)) && !checkedHours.Any(m => m == x));
            
                        foreach (var uncheckedHour in uncheckedHours)
                        {
                            if (OverlapsWith(first, uncheckedHour))
                            {
                                return true;
                            }
                        }
                    }
                    first = hour;

                    bool OverlapsWith(VhoNonAvailability first, VhoNonAvailability second)
                    {
                        return first.EndTime > second.StartTime;
                    }
                }
            }

            return false;
        }
    }
}
