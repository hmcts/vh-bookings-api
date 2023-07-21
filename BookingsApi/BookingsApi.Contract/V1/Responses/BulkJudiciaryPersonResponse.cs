using System.Collections.Generic;

namespace BookingsApi.Contract.V1.Responses
{
    public class BulkJudiciaryPersonResponse
    {
        public BulkJudiciaryPersonResponse()
        {
            ErroredRequests = new List<JudiciaryPersonErrorResponse>();
        }
            
        public List<JudiciaryPersonErrorResponse> ErroredRequests { get; set; }
    }
}