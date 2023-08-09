using BookingsApi.Contract.Interfaces.Response;

namespace BookingsApi.Contract.V1.Responses
{
    public class CaseRoleResponse : ICaseRoleResponse
    {
        public string Name { get; set; }
        public string UserRole { get; set; }
    }
}