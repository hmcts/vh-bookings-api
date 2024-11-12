namespace BookingsApi.DAL.Exceptions;

public class NonWorkingHoursNotFoundException(long id) : EntityNotFoundException($"Non working hour {id} does not exist");