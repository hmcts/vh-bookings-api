using System;
using System.Linq;
using System.Threading.Tasks;
using BookingsApi.DAL.Commands.Core;
using BookingsApi.DAL.Exceptions;
using BookingsApi.Domain;
using BookingsApi.Domain.Enumerations;
using BookingsApi.Domain.RefData;

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
        public int RoleId { get; }

        public AddJusticeUserCommand(string firstName, string lastname, string username, string contactEmail,
            string createdBy, int roleId)
        {
            FirstName = firstName;
            Lastname = lastname;
            Username = username;
            ContactEmail = contactEmail;
            CreatedBy = createdBy;
            RoleId = roleId;
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
            var roleName = command.RoleId == (int)UserRoleId.VhTeamLead ? "Video hearings team lead" : "Video hearings officer";
            var role = new UserRole(command.RoleId, roleName);

            if (_context.JusticeUsers.Any(x => x.Username.ToLower() == command.Username.ToLower()))
            {
                throw new JusticeUserAlreadyExistsException(command.Username);
            }
            
            var justiceUser = new JusticeUser(command.FirstName, command.Lastname, command.ContactEmail,
                command.Username, role)
            {
                Telephone = command.Telephone,
                CreatedBy = command.CreatedBy
            };

            await _context.AddAsync(justiceUser);
            await _context.SaveChangesAsync();
        }
    }
}