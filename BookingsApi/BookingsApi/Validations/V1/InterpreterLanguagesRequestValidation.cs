using BookingsApi.Contract.V1.Requests;
using FluentValidation;

namespace BookingsApi.Validations.V1;

public class BulkUpdateInterpreterLanguagesRequestValidation : AbstractValidator<List<InterpreterLanguagesRequest>>
{
    public const string DuplicateCodesError = "Codes must be unique";

    public BulkUpdateInterpreterLanguagesRequestValidation()
    {
        RuleForEach(x => x).SetValidator(new InterpreterLanguagesRequestValidation());
        // make sure all the codes are unique
        RuleFor(x => x).Must(x => x.Select(y => y.Code).Distinct().Count() == x.Count).WithMessage(DuplicateCodesError);
    }
}

public class InterpreterLanguagesRequestValidation : AbstractValidator<InterpreterLanguagesRequest>
{
    public const string NoCodeError = "Code is required";
    public const string NoValueError = "Value is required";
        
    public InterpreterLanguagesRequestValidation()
    {
        RuleFor(x => x.Code).NotEmpty().WithMessage(NoCodeError);
        RuleFor(x => x.Value).NotEmpty().WithMessage(NoValueError);
    }
}