using Bookings.DAL.Commands.Core;
using Bookings.DAL.Exceptions;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

namespace Bookings.DAL.Commands
{
    public class UpdateZipStatusCommand : ICommand
    {

        public Guid HearingId { get; set; }
        public bool ZipStatus { get; set; }
        public UpdateZipStatusCommand(Guid hearingId, bool zipStatus)
        {
            HearingId = hearingId;
            ZipStatus = zipStatus;
        }
    }
    public class UpdateZipStatusCommandHandler : ICommandHandler<UpdateZipStatusCommand>
    {
        private readonly BookingsDbContext _context;

        public UpdateZipStatusCommandHandler(BookingsDbContext context)
        {
            _context = context;
        }

        public async Task Handle(UpdateZipStatusCommand command)
        {
            var hearing = await _context.VideoHearings.SingleOrDefaultAsync(x => x.Id == command.HearingId);

            if (hearing == null)
            {
                throw new HearingNotFoundException(command.HearingId);
            }

            hearing.ZipSuccess = command.ZipStatus;
            await _context.SaveChangesAsync();
        }
    }
}
