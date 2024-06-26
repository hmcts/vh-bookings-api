﻿using BookingsApi.Contract.Interfaces.Requests;
using FluentValidation;

namespace BookingsApi.Validations.Common
{
    public class RepresentativeValidation : AbstractValidator<IRepresentativeInfoRequest>
    {
        public static readonly string NoRepresentee = "Representee is required";

        public RepresentativeValidation()
        {
            RuleFor(x => x.Representee).NotEmpty().WithMessage(NoRepresentee);
        }
    }
}
