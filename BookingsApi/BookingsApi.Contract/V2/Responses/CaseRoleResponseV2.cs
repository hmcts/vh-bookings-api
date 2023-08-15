using BookingsApi.Contract.Interfaces.Response;

namespace BookingsApi.Contract.V2.Responses
{
    public class CaseRoleResponseV2 : ICaseRoleResponse
    {
        public string Name { get; set; }
        public string UserRole { get; set; }
    }
}