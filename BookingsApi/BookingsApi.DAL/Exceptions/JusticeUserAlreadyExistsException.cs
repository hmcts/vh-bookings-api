namespace BookingsApi.DAL.Exceptions;

public class JusticeUserAlreadyExistsException(string username)
    : BookingsDalException($"A justice user with the username {username} already exists");