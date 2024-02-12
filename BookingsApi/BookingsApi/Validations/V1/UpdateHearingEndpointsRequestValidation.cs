using BookingsApi.Contract.V1.Requests;
using FluentValidation;

namespace BookingsApi.Validations.V1
{
    public class UpdateHearingEndpointsRequestValidation : AbstractValidator<UpdateHearingEndpointsRequest>
    {
        public UpdateHearingEndpointsRequestValidation()
        {
            RuleForEach(x => x.NewEndpoints)
                .SetValidator(new AddEndpointRequestValidation());
            
            RuleForEach(x => x.ExistingEndpoints)
                .SetValidator(new UpdateEndpointRequestValidation());
        }
    }
}
