using BookingsApi.Contract.V2.Responses;

namespace BookingsApi.Mappings.V2
{
    public static class CaseToResponseV2Mapper
    {
        public static CaseResponseV2 MapCaseToResponse(Case @case)
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