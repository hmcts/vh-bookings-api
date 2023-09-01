using BookingsApi.Contract.V1.Requests;

namespace BookingsApi.Contract.V1.Responses
{
    public class JudiciaryPersonErrorResponse
    {
        public string Message { get; set; }
        public JudiciaryPersonRequest JudiciaryPersonRequest { get; set; }
    }
}