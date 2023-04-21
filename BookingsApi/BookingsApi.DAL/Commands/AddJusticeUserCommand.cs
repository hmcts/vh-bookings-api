using System.Linq;
using System.Threading.Tasks;
using BookingsApi.DAL.Commands.Core;
using BookingsApi.DAL.Exceptions;
using BookingsApi.Domain;
using Microsoft.EntityFrameworkCore;

namespace BookingsApi.DAL.Commands
{
    public class AddJusticeUserCommand : ICommand
    {
        public string FirstName { get; }
        public string Lastname { get; }
        public string Username { get; }
        public string ContactEmail { get; }
        public string Telephone { get; set; }
        public string CreatedBy { get; }
        public int[] RoleIds { get; }

        public AddJusticeUserCommand(string firstName, string lastname, string username, string contactEmail, string createdBy, params int[] roleIds)
        {
            FirstName = firstName;
            Lastname = lastname;
            Username = username;
            ContactEmail = contactEmail;
            CreatedBy = createdBy;
            RoleIds = roleIds;
        }
    }

    public class AddJusticeUserCommandHandler : ICommandHandler<AddJusticeUserCommand>
    {
        private readonly BookingsDbContext _context;

        public AddJusticeUserCommandHandler(BookingsDbContext context)
        {
            _context = context;
        }

        public async Task Handle(AddJusticeUserCommand command)
        {
            var roles = await _context.UserRoles.Where(x => command.RoleIds.Contains(x.Id)).ToArrayAsync();
            
            if (_context.JusticeUsers.IgnoreQueryFilters().Any(x => x.Username.ToLower() == command.Username.ToLower()))
            {
                throw new JusticeUserAlreadyExistsException(command.Username);
            }
            
            var justiceUser = new JusticeUser(command.FirstName, command.Lastname, command.ContactEmail, command.Username) {
                Telephone = command.Telephone,
                CreatedBy = command.CreatedBy
            };
            justiceUser.AddRoles(roles);
            await _context.AddAsync(justiceUser);
            await _context.SaveChangesAsync();
        }
    }
}