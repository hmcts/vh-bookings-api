using BookingsApi.Contract.Interfaces.Response;

namespace BookingsApi.Contract.V1.Responses
{
    public class HearingRoleResponse : IHearingRoleResponse
    {
        public string Name { get; set; }
        public string UserRole { get; set; }
    }
}