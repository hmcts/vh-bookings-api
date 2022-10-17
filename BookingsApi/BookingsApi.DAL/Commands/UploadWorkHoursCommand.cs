using BookingsApi.Contract.Requests;
using BookingsApi.DAL.Commands.Core;
using BookingsApi.Domain;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookingsApi.DAL.Commands
{
    public class UploadWorkHoursCommand : ICommand
    {
        public List<UploadWorkHoursRequest> UploadWorkHoursRequests { get; set; }
        public List<string> FailedUploadUsernames { get; set; } = new List<string>();

        public UploadWorkHoursCommand(List<UploadWorkHoursRequest> uploadWorkHoursRequests)
        {
            UploadWorkHoursRequests = uploadWorkHoursRequests;
        }
    }

    public class UploadWorkHoursCommandHandler : ICommandHandler<UploadWorkHoursCommand>
    {
        private readonly BookingsDbContext _context;

        public UploadWorkHoursCommandHandler(BookingsDbContext context)
        {
            _context = context;
        }

        public async Task Handle(UploadWorkHoursCommand command)
        {
            foreach (var uploadWorkHoursRequest in command.UploadWorkHoursRequests)
            {
                var user = await _context.JusticeUsers
                    .SingleOrDefaultAsync(x => x.Username == uploadWorkHoursRequest.Username);

                if (user == null)
                {
                    command.FailedUploadUsernames.Add(uploadWorkHoursRequest.Username);
                    continue;
                }

                var vhoWorkHours = _context.VhoWorkHours
                    .Where(x => x.JusticeUser.Username == uploadWorkHoursRequest.Username);

                foreach (var workHours in uploadWorkHoursRequest.WorkingHours)
                {
                    var vhoWorkHour = vhoWorkHours
                        .SingleOrDefault(x => x.DayOfWeekId == workHours.DayOfWeekId);

                    bool doesVhoWorkHourExist = true;

                    if (vhoWorkHour == null)
                    {
                        doesVhoWorkHourExist = false;
                        vhoWorkHour = new VhoWorkHours();
                    }

                    vhoWorkHour.DayOfWeekId = workHours.DayOfWeekId;
                    vhoWorkHour.JusticeUserId = user.Id;
                    vhoWorkHour.StartTime = workHours.StartTime;
                    vhoWorkHour.EndTime = workHours.EndTime;

                    if (doesVhoWorkHourExist) 
                        _context.Update(vhoWorkHour);
                    else 
                        _context.Add(vhoWorkHour);
                }
            }
            await _context.SaveChangesAsync();
        }
    }
}