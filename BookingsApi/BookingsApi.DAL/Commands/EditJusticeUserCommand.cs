using System;
using System.Linq;
using System.Threading.Tasks;
using BookingsApi.DAL.Commands.Core;
using BookingsApi.DAL.Exceptions;
using BookingsApi.Domain;
using BookingsApi.Domain.Enumerations;
using BookingsApi.Domain.RefData;
using Microsoft.EntityFrameworkCore;

namespace BookingsApi.DAL.Commands
{
    public class EditJusticeUserCommand : ICommand
    {
        public Guid Id { get; }
        public string Username { get; }
        public int[] Roles { get; }

        public EditJusticeUserCommand(Guid id, string username, params int[] role)
        {
            Id = id;
            Username = username;
            Roles = role;
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
            var justiceUser = await _context.JusticeUsers
                .Include(ju => ju.JusticeUserRoles)
                .ThenInclude(jur => jur.UserRole)
                .FirstOrDefaultAsync(x => x.Id == command.Id);

            if (justiceUser == null)
                throw new JusticeUserNotFoundException(command.Id);
            
            _context.Update(justiceUser);
            
            await UpdateJusticeUserRoles(command, justiceUser);
            await _context.SaveChangesAsync();
        }

        private async Task UpdateJusticeUserRoles(EditJusticeUserCommand command, JusticeUser justiceUser)
        {
            var selectedRoles = await _context.UserRoles.Where(ur => command.Roles.Contains(ur.Id)).ToListAsync();
            var currentUserRoles = justiceUser.JusticeUserRoles.Select(jur => jur.UserRole).ToList();
            
            var rolesToRemove = currentUserRoles.Except(selectedRoles);
            var removeEntities = justiceUser.JusticeUserRoles
                .Where(ju => rolesToRemove.Contains(ju.UserRole));

            var rolesToAdd = selectedRoles.Except(currentUserRoles);
            var addEntities = rolesToAdd.Select(userRole => new JusticeUserRole(justiceUser, userRole)).ToList();

            _context.RemoveRange(removeEntities);
            _context.AddRange(addEntities);
        }
    }
}