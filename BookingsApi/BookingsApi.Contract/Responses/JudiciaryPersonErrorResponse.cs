using BookingsApi.Contract.Requests;

namespace BookingsApi.Contract.Responses
{
    public class JudiciaryPersonErrorResponse
    {
        public string Message { get; set; }
        public JudiciaryPersonRequest JudiciaryPersonRequest { get; set; }
    }
}