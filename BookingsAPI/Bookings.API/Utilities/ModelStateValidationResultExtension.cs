using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Bookings.API.Utilities
{
    /// <summary>
    /// Helper to add validation results to model states
    /// </summary>
    public static class ModelStateValidationResultExtension
    {
        public static void AddTo(this ValidationResult validationResult, ModelStateDictionary modelState)
        {
            if (validationResult.IsValid)
                return;

            foreach (var failure in validationResult.Errors)
                modelState.AddModelError(failure.PropertyName, failure.ErrorMessage);
        }
    }
}
