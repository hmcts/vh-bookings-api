using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using DomainValidationFailures = Bookings.Domain.Validations.ValidationFailures;
using FluentValidationFailure = FluentValidation.Results.ValidationFailure;

namespace BookingsApi.Extensions
{
    public static class ModelStateExtensions
    {
        public static void AddFluentValidationErrors(this ModelStateDictionary modelState, IEnumerable<FluentValidationFailure> validationFailures)
        {
            foreach (var failure in validationFailures)
            {
                modelState.AddModelError(failure.PropertyName, failure.ErrorMessage);
            }
        }
        
        public static void AddDomainRuleErrors(this ModelStateDictionary modelState, DomainValidationFailures validationFailures)
        {
            validationFailures.ForEach(x => modelState.AddModelError(x.Name, x.Message));
        }
    }
}