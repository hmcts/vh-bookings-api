using BookingsApi.Contract.Responses;
using BookingsApi.Domain;

namespace BookingsApi.Mappings
{
    public class CaseToResponseMapper
    {
        public CaseResponse MapCaseToResponse(Case @case)
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