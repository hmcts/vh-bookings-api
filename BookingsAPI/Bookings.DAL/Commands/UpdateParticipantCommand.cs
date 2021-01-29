using System;
using System.Threading.Tasks;
using Bookings.DAL.Commands.Core;
using Bookings.Domain.Participants;
using Microsoft.EntityFrameworkCore;
using Bookings.DAL.Exceptions;
using System.Linq;

namespace Bookings.DAL.Commands
{
    public class RepresentativeInformation
    {
        public string Representee { get; set; }
    }
    public class UpdateParticipantCommand : ICommand
    {
        public Guid HearingId { get; set; }
        public Guid ParticipantId { get; set; }
        public string Title { get; set; }
        public string DisplayName { get; set; }
        public string TelephoneNumber { get; set; }
        public string OrganisationName { get; set; }
        public Participant UpdatedParticipant { get; set; }
        public RepresentativeInformation RepresentativeInformation { get; set; }

        public UpdateParticipantCommand(Guid hearingId, Guid participantId, string title, string displayName, string telphoneNumber, 
            string organisationName, RepresentativeInformation representativeInformation)
        {
            HearingId = hearingId;
            ParticipantId = participantId;
            Title = title;
            DisplayName = displayName;
            TelephoneNumber = telphoneNumber;
            OrganisationName = organisationName;
            RepresentativeInformation = representativeInformation;
        }
    }

    public class UpdateParticipantCommandHandler : ICommandHandler<UpdateParticipantCommand>
    {
        private readonly BookingsDbContext _context;

        public UpdateParticipantCommandHandler(BookingsDbContext context)
        {
            _context = context;
        }

        public async Task Handle(UpdateParticipantCommand command)
        {
            var hearing = await _context.VideoHearings
                .Include(x => x.Participants).ThenInclude(x => x.Person.Organisation)
                .Include(x => x.Participants).ThenInclude(x => x.HearingRole.UserRole)
                .Include(x => x.Participants).ThenInclude(x => x.CaseRole)
                .SingleOrDefaultAsync(x => x.Id == command.HearingId);

            if (hearing == null)
            {
                throw new HearingNotFoundException(command.HearingId);
            }

            var participants = hearing.GetParticipants().ToList();

            var participant = participants.FirstOrDefault(x => x.Id == command.ParticipantId);

            if (participant == null)
            {
                throw new ParticipantNotFoundException(command.ParticipantId);
            }

            participant.UpdateParticipantDetails(command.Title, command.DisplayName, command.TelephoneNumber, command.OrganisationName);

            if (participant.HearingRole.UserRole.IsRepresentative)
            {
                ((Representative)participant).UpdateRepresentativeDetails(
                    command.RepresentativeInformation.Representee);
            }
            await _context.SaveChangesAsync();

            command.UpdatedParticipant = participant;
        }
    }
}