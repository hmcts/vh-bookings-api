using System.Collections.Generic;

namespace BookingsApi.Contract.Requests
{
    public class BulkJudiciaryPersonResponse
    {
        public BulkJudiciaryPersonResponse()
        {
            ErroredRequests = new List<JudiciaryPersonErrorResponse>();
        }
            
        public List<JudiciaryPersonErrorResponse> ErroredRequests { get; set; }
    }

    public class JudiciaryPersonErrorResponse
    {
        public string Message { get; set; }
        public JudiciaryPersonRequest JudiciaryPersonRequest { get; set; }
    }
}