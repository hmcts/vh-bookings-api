using FluentValidation;

namespace BookingsApi.Validations.Common;

public interface IRefDataInputValidator{}

public abstract class RefDataInputValidatorValidator<T> : AbstractValidator<T>, IRefDataInputValidator
{
    
}