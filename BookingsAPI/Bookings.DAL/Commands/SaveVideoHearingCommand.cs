using System.Threading.Tasks;
using Bookings.Domain;

namespace Bookings.DAL.Commands
{
    public class SaveVideoHearingCommand : ICommand
    {
        public SaveVideoHearingCommand(VideoHearing videoHearing)
        {
            VideoHearing = videoHearing;
        }

        public VideoHearing VideoHearing { get; set; }
    }

    public class SaveVideoHearingCommandHandler : ICommandHandler<SaveVideoHearingCommand>
    {
        private readonly BookingsDbContext _context;

        public SaveVideoHearingCommandHandler(BookingsDbContext context)
        {
            _context = context;
        }

        public async Task Handle(SaveVideoHearingCommand command)
        {
            _context.VideoHearings.Add(command.VideoHearing);
            await _context.SaveChangesAsync();
        }
    }
}