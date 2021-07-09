using System.Collections.Generic;

namespace BookingsApi.Contract.Responses
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