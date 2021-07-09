using System.Collections.Generic;

namespace BookingsApi.Contract.Responses
{
    public class BulkJudiciaryLeaverResponse
    {
        public BulkJudiciaryLeaverResponse()
        {
            ErroredRequests = new List<JudiciaryLeaverErrorResponse>();
        }
            
        public List<JudiciaryLeaverErrorResponse> ErroredRequests { get; set; }
    }
}