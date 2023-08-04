using BookingsApi.Contract.V2.Responses;

namespace BookingsApi.Mappings.V2
{
    public class CaseToResponseV2Mapper
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