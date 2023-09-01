using BookingsApi.Contract.V1.Requests;

namespace BookingsApi.Contract.V1.Responses
{
    public class JudiciaryLeaverErrorResponse
    {
        public string Message { get; set; }
        public JudiciaryLeaverRequest JudiciaryLeaverRequest { get; set; }
    }
}