using BookingsApi.Contract.V1.Requests;
using FluentValidation;
using FluentValidation.Results;
using ValidationFailure = FluentValidation.Results.ValidationFailure;

namespace BookingsApi.Validations.V1;

public class UploadWorkHoursRequestsValidation : AbstractValidator<List<UploadWorkHoursRequest>>
{
    public ValidationResult ValidateRequests(List<UploadWorkHoursRequest> requests)
    {
        var errors = new List<ValidationFailure>();

        CheckForDuplicateUsers(requests, errors);

        foreach (var request in requests)
        {
            var daysAreValid = CheckDaysAreValid(request.WorkingHours, request.Username, errors);

            if (!daysAreValid) continue;
                
            ValidateWorkhours(request, errors);
        }

        return new ValidationResult(errors);
    }

    private static void ValidateWorkhours(UploadWorkHoursRequest request, List<ValidationFailure> errors)
    {
        foreach (var workHours in request.WorkingHours)
        {
            if (CheckForNullStartDate(workHours))
            {
                errors.Add(new ValidationFailure($"{request.Username}, Day Number {workHours.DayOfWeekId}", "Day contains a blank start/end time along with a populated start/end time."));
                continue;
            }

            if (ValidateStartEndTimeMinutes(request, errors, workHours)) 
                continue;

            if (workHours.EndTime < workHours.StartTime)
            {
                errors.Add(new ValidationFailure($"{request.Username}, Day Number {workHours.DayOfWeekId}", $"End time {workHours.EndTime} is before start time {workHours.StartTime}."));
            }
        }
    }

    private static bool ValidateStartEndTimeMinutes(UploadWorkHoursRequest request, List<ValidationFailure> errors, WorkingHours workHours)
    {
        if (workHours.StartTimeMinutes < 0 || workHours.StartTimeMinutes > 59)
        {
            errors.Add(new ValidationFailure($"{request.Username}, Day Number {workHours.DayOfWeekId}", $"Start time minutes is not within 0-59."));
            return true;
        }

        if (workHours.EndTimeMinutes < 0 || workHours.EndTimeMinutes > 59)
        {
            errors.Add(new ValidationFailure($"{request.Username}, Day Number {workHours.DayOfWeekId}", $"End time minutes is not within 0-59."));
            return true;
        }

        return false;
    }

    private static bool CheckForNullStartDate(WorkingHours workHours)
    {
        var containsNullTime = workHours.GetType().GetProperties()
            .Where(pi => pi.PropertyType == typeof(int?))
            .Select(pi => (int?)pi.GetValue(workHours))
            .Any(value => value == null);

        var containsPopulatedTime = workHours.GetType().GetProperties()
            .Where(pi => pi.PropertyType == typeof(int?))
            .Select(pi => (int?)pi.GetValue(workHours))
            .Any(value => value != null);

        return containsNullTime && containsPopulatedTime;
    }

    private static void CheckForDuplicateUsers(List<UploadWorkHoursRequest> requests, List<ValidationFailure> errors)
    {
        var duplicateEntries = requests
            .Select(e => e.Username)
            .GroupBy(x => x)
            .SelectMany(g => g.Skip(1))
            .ToList();

        if (duplicateEntries.Count != 0)
        {
            foreach (var user in duplicateEntries)
            {
                errors.Add(new ValidationFailure($"{user}", "Multiple entries for user. Only one row per user required"));
            }
        }
    }

    private static bool CheckDaysAreValid(IEnumerable<WorkingHours> workHours, 
        string username, 
        ICollection<ValidationFailure> errors)
    {
        var requiredDayOfWeekIds = new List<int> { 1, 2, 3, 4, 5, 6, 7 };
            
        // Use hash sets to compare the lists regardless of sequential order
        var dayOfWeekIdsHashSet = new HashSet<int>(workHours.Select(wh => wh.DayOfWeekId));
        var requiredDayOfWeekIdsHashSet = new HashSet<int>(requiredDayOfWeekIds);

        var daysAreValid = dayOfWeekIdsHashSet.SetEquals(requiredDayOfWeekIdsHashSet);

        if (!daysAreValid)
        {
            errors.Add(new ValidationFailure($"{username}, Day Numbers", "Must specify one entry for each day of the week for days 1-7"));
        }

        return daysAreValid;
    }
}