using BookingsApi.Contract.V2.Responses;

namespace BookingsApi.Mappings.V2
{
    /// <summary>
    /// Mapper for mapping a case to a V2 response
    /// </summary>
    public static class CaseToResponseV2Mapper
    {
        /// <summary>
        /// Maps a case to a V2 response
        /// </summary>
        /// <param name="case"></param>
        /// <returns></returns>
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