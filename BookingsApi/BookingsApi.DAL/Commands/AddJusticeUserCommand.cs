namespace BookingsApi.DAL.Commands;

public class AddJusticeUserCommand(
    string firstName,
    string lastname,
    string username,
    string contactEmail,
    string createdBy,
    params int[] roleIds)
    : ICommand
{
    public string FirstName { get; } = firstName;
    public string Lastname { get; } = lastname;
    public string Username { get; } = username;
    public string ContactEmail { get; } = contactEmail;
    public string Telephone { get; init; }
    public string CreatedBy { get; } = createdBy;
    public int[] RoleIds { get; } = roleIds;
}

public class AddJusticeUserCommandHandler(BookingsDbContext context) : ICommandHandler<AddJusticeUserCommand>
{
    public async Task Handle(AddJusticeUserCommand command)
    {
        var roles = await context.UserRoles.Where(x => command.RoleIds.Contains(x.Id)).ToArrayAsync();

        if (await context.JusticeUsers.IgnoreQueryFilters().AnyAsync(x => x.Username.ToLower() == command.Username.ToLower()))
        {
            throw new JusticeUserAlreadyExistsException(command.Username);
        }
            
        var justiceUser = new JusticeUser(command.FirstName, command.Lastname, command.ContactEmail, command.Username) {
            Telephone = command.Telephone,
            CreatedBy = command.CreatedBy
        };
        justiceUser.AddRoles(roles);
        await context.AddAsync(justiceUser);
        await context.SaveChangesAsync();
    }
}