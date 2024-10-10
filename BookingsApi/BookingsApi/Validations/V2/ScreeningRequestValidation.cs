using System.Data;
using BookingsApi.Contract.V2.Requests;
using FluentValidation;
using ScreeningType = BookingsApi.Contract.V2.Enums.ScreeningType;

namespace BookingsApi.Validations.V2;

public class ScreeningRequestValidation : AbstractValidator<ScreeningRequest>
{
    public ScreeningRequestValidation()
    {
        RuleFor(x => x.Type).IsInEnum();

        RuleFor(x => x.ProtectedFrom).NotEmpty()
            .ForEach(entry => entry.NotNull())
            .When(x => x.Type == ScreeningType.Specific);

    }
}