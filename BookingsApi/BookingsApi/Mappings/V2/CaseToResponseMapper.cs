using BookingsApi.Contract.V2.Responses;
using BookingsApi.Domain;

namespace BookingsApi.Mappings.V2
{
    public class CaseToResponseMapper
    {
        public CaseResponseV2 MapCaseToResponse(Case @case)
        {
            return new CaseResponseV2
            {
                Name = @case.Name,
                Number = @case.Number,
                IsLeadCase = @case.IsLeadCase
            };
        }
    }
}