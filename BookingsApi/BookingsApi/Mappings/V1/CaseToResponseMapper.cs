using BookingsApi.Contract.V1.Responses;

namespace BookingsApi.Mappings.V1
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