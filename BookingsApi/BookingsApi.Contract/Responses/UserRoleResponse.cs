namespace BookingsApi.Contract.Responses;

public class UserRoleResponse
{
    public UserRoleResponse(int id, string name)
    {
        Id = id;
        Name = name;
    }
    public int Id { get; }
    public string Name { get; }
}