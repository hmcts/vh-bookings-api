using BookingsApi.Contract.V2.Requests;
using FluentValidation;

namespace BookingsApi.Validations.V2
{
    public class UpdateHearingEndpointsRequestValidationV2 : AbstractValidator<UpdateHearingEndpointsRequestV2>
    {
        public UpdateHearingEndpointsRequestValidationV2()
        {
            RuleForEach(x => x.NewEndpoints)
                .SetValidator(new EndpointRequestValidationV2());
            
            RuleForEach(x => x.ExistingEndpoints)
                .SetValidator(new EndpointRequestValidationV2());
        }
    }
}
