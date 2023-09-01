using BookingsApi.Contract.V1.Requests;
using FluentValidation;
using FluentValidation.Results;
using ValidationFailure = FluentValidation.Results.ValidationFailure;

namespace BookingsApi.Validations.V1
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
            
            var requestedWorkHourIds = request.Hours.Select(h => h.Id).Where(x => x != 0).ToList();
            var foundWorkHourIds = existingHours.Select(h => h.Id).ToList();

            var requestedWorkHourIdsFound = requestedWorkHourIds.TrueForAll(foundWorkHourIds.Contains);

            if (!requestedWorkHourIdsFound)
            {
                errors.Add(new ValidationFailure(nameof(request.Hours), HourIdsNotFoundErrorMessage));
                return new ValidationResult(errors);
            }
            
            var updatedWorkHours = existingHours.ToList();
            foreach (var updatedWorkHour in updatedWorkHours)
            {
                var hourInRequest = request.Hours.SingleOrDefault(h => h.Id == updatedWorkHour.Id);

                if (hourInRequest == null)
                {
                    continue;
                }
                
                updatedWorkHour.StartTime = hourInRequest.StartTime;
                updatedWorkHour.EndTime = hourInRequest.EndTime;
            }

            var hoursOverlap = CheckForOverlappingHours(updatedWorkHours);
            if (hoursOverlap)
            {
                errors.Add(new ValidationFailure(nameof(request.Hours), HoursOverlapErrorMessage));
            }

            return new ValidationResult(errors);
        }

        private static bool CheckForOverlappingHours(IList<VhoNonAvailability> nonWorkingHours)
        {
            var hoursForUser = nonWorkingHours
                .OrderBy(h => h.StartTime)
                .ToList();

            var firstHour = (VhoNonAvailability)null;
            var checkedHours = new List<VhoNonAvailability>();

            foreach (var hour in hoursForUser)
            {
                if (firstHour != null)
                {
                    checkedHours.Add(firstHour);
                    var uncheckedHours = hoursForUser.Where(x => (x.StartTime >= firstHour.StartTime && x != firstHour) && checkedHours.TrueForAll(m => m != x));
        
                    if (uncheckedHours.Any(uncheckedHour => OverlapsWith(firstHour, uncheckedHour)))
                    {
                        return true;
                    }
                }
                firstHour = hour;
            }

            return false;
        }
        
        private static bool OverlapsWith(VhoNonAvailability first, VhoNonAvailability second)
        {
            return first.EndTime > second.StartTime;
        }
    }
}
