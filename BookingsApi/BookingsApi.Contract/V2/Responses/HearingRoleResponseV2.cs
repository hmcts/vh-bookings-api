using BookingsApi.Contract.Interfaces.Response;

namespace BookingsApi.Contract.V2.Responses
{
    public class HearingRoleResponseV2 : IHearingRoleResponse
    {
        public string Name { get; set; }
        public string UserRole { get; set; }
        public string Code { get; set; }
        public string WelshName { get; set; }
    }
}