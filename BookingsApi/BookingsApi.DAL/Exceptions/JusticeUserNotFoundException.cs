namespace BookingsApi.DAL.Exceptions;

public class JusticeUserNotFoundException : EntityNotFoundException
{
    public JusticeUserNotFoundException(Guid id) : base($"Justice user with id {id} not found") { }
        
    public JusticeUserNotFoundException(string username) : base($"Justice user with username {username} not found") { }
}