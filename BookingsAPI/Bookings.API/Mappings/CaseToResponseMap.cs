using Bookings.Api.Contract.Responses;
using Bookings.Domain;

namespace Bookings.API.Mappings
{
    public class CaseToResponseMap
    {
        public CaseResponse MapDomainToResponse(Case @case)
        {
            return new CaseResponse
            {
                Name = @case.Name,
                Number = @case.Number,
                IsLeadCase = @case.IsLeadCase
            };
        }
    }
}