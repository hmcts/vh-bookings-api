using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BookingsApi.Contract.Requests;
using BookingsApi.DAL.Commands.Core;
using BookingsApi.DAL.Services;
using BookingsApi.Domain;
using Microsoft.EntityFrameworkCore;

namespace BookingsApi.DAL.Commands.V1
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
        private readonly IHearingAllocationService _hearingAllocationService;

        public UploadWorkHoursCommandHandler(BookingsDbContext context, IHearingAllocationService hearingAllocationService)
        {
            _context = context;
            _hearingAllocationService = hearingAllocationService;
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

                await _hearingAllocationService.DeallocateFromUnavailableHearings(user.Id);
            }
            await _context.SaveChangesAsync();
        }
    }
}