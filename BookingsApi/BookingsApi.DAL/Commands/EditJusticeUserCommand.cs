using System;
using System.Threading.Tasks;
using BookingsApi.DAL.Commands.Core;
using BookingsApi.DAL.Exceptions;
using BookingsApi.Domain.Enumerations;
using BookingsApi.Domain.RefData;
using Microsoft.EntityFrameworkCore;

namespace BookingsApi.DAL.Commands
{
    public class EditJusticeUserCommand : ICommand
    {
        public Guid Id { get; }
        public string Username { get; }
        public int RoleId { get; }

        public EditJusticeUserCommand(Guid id, string username, int roleId)
        {
            Id = id;
            Username = username;
            RoleId = roleId;
        }
    }

    public class EditJusticeUserCommandHandler : ICommandHandler<EditJusticeUserCommand>
    {
        private readonly BookingsDbContext _context;

        public EditJusticeUserCommandHandler(BookingsDbContext context)
        {
            _context = context;
        }

        public async Task Handle(EditJusticeUserCommand command)
        {
            var roleName = command.RoleId == (int)UserRoleId.VhTeamLead ? "Video hearings team lead" : "Video hearings officer";
            var role = new UserRole(command.RoleId, roleName);

            var justiceUser = await _context.JusticeUsers
                .SingleOrDefaultAsync(x => x.Username == command.Username);

            if (justiceUser == null)
            {
                throw new JusticeUserNotFoundException(command.Id);
            }

            justiceUser.UserRole = role;
            _context.Update(justiceUser);
            await _context.SaveChangesAsync();
        }
    }
}