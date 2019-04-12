using System;
using System.Linq;
using System.Threading.Tasks;
using Bookings.Api.Contract.Requests;
using FluentValidation;

namespace Bookings.API.Validations
{
    public class UpdateParticipantRequestValidation : AbstractValidator<UpdateParticipantRequest>
    {
        public static readonly string NoDisplayNameErrorMessage = "Display name is required";

        public UpdateParticipantRequestValidation()
        {
          RuleFor(x => x.DisplayName).NotEmpty().WithMessage(NoDisplayNameErrorMessage);
        }

        
    }
}