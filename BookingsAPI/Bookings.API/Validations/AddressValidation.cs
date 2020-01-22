using Bookings.Api.Contract.Requests;
using FluentValidation;

namespace Bookings.API.Validations
{
    public class AddressValidation : AbstractValidator<IAddressRequest>
    {
        public static readonly string NoHouseNumberErrorMessage = "HouseNumber is required";
        public static readonly string NoStreetErrorMessage = "Street is required";
        public static readonly string NoCityErrorMessage = "City is required";
        public static readonly string NoCountyErrorMessage = "County is required";
        public static readonly string NoPostcodeErrorMessage = "Postcode is required";

        public AddressValidation()
        {
            RuleFor(x => x.HouseNumber).NotEmpty().WithMessage(NoHouseNumberErrorMessage);
            RuleFor(x => x.Street).NotEmpty().WithMessage(NoStreetErrorMessage);
            RuleFor(x => x.City).NotEmpty().WithMessage(NoCityErrorMessage);
            RuleFor(x => x.County).NotEmpty().WithMessage(NoCountyErrorMessage);
            RuleFor(x => x.Postcode).NotEmpty().WithMessage(NoPostcodeErrorMessage);
        }
    }
}
