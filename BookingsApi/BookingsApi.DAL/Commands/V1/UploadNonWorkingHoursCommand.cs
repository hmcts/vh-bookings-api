using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BookingsApi.Contract.V1.Requests;
using BookingsApi.DAL.Commands.Core;
using BookingsApi.DAL.Services;
using BookingsApi.Domain;
using Microsoft.EntityFrameworkCore;

namespace BookingsApi.DAL.Commands.V1
{
    public class UploadNonWorkingHoursCommand : ICommand
    {
        public List<UploadNonWorkingHoursRequest> UploadNonWorkingHoursRequests { get; set; }
        public List<string> FailedUploadUsernames { get; set; } = new List<string>();

        public UploadNonWorkingHoursCommand(List<UploadNonWorkingHoursRequest> uploadNonWorkingHoursRequests)
        {
            UploadNonWorkingHoursRequests = uploadNonWorkingHoursRequests;
        }
    }

    public class UploadNonWorkingHoursCommandHandler : ICommandHandler<UploadNonWorkingHoursCommand>
    {
        private readonly BookingsDbContext _context;
        private readonly IHearingAllocationService _hearingAllocationService;

        public UploadNonWorkingHoursCommandHandler(BookingsDbContext context, IHearingAllocationService hearingAllocationService)
        {
            _context = context;
            _hearingAllocationService = hearingAllocationService;
        }

        public async Task Handle(UploadNonWorkingHoursCommand command)
        {
            foreach (var uploadNonWorkingHoursRequest in command.UploadNonWorkingHoursRequests)
            {
                var user = await _context.JusticeUsers
                    .SingleOrDefaultAsync(x => x.Username == uploadNonWorkingHoursRequest.Username);

                if (user == null)
                {
                    command.FailedUploadUsernames.Add(uploadNonWorkingHoursRequest.Username);
                    continue;
                }

                var vhoNonAvailabilities = _context.VhoNonAvailabilities
                    .Where(x => x.JusticeUser.Username == uploadNonWorkingHoursRequest.Username && !x.Deleted);

                var vhoNonWorkingHours = vhoNonAvailabilities
                    .SingleOrDefault(x => x.StartTime == uploadNonWorkingHoursRequest.StartTime
                        || x.EndTime == uploadNonWorkingHoursRequest.EndTime);

                bool doesVhoNonWorkingHoursExist = true;

                if (vhoNonWorkingHours == null)
                {
                    doesVhoNonWorkingHoursExist = false;
                    vhoNonWorkingHours = new VhoNonAvailability();
                }

                vhoNonWorkingHours.JusticeUserId = user.Id;
                vhoNonWorkingHours.StartTime = uploadNonWorkingHoursRequest.StartTime;
                vhoNonWorkingHours.EndTime = uploadNonWorkingHoursRequest.EndTime;

                if (doesVhoNonWorkingHoursExist)
                    _context.Update(vhoNonWorkingHours);
                else
                    _context.Add(vhoNonWorkingHours);

                await _hearingAllocationService.DeallocateFromUnavailableHearings(user.Id);
                
                await _context.SaveChangesAsync();
            }
        }
    }
}