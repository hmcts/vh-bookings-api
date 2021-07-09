using BookingsApi.Contract.Requests;

namespace BookingsApi.Contract.Responses
{
    public class JudiciaryLeaverErrorResponse
    {
        public string Message { get; set; }
        public JudiciaryLeaverRequest JudiciaryLeaverRequest { get; set; }
    }
}