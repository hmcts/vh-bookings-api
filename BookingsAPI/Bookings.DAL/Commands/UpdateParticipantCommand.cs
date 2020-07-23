using System;
using System.Threading.Tasks;
using Bookings.DAL.Commands.Core;
using Bookings.Domain.Participants;
using Microsoft.EntityFrameworkCore;
using Bookings.DAL.Exceptions;
using System.Linq;
using Bookings.Domain;

namespace Bookings.DAL.Commands
{
    public class RepresentativeInformation
    {
        public string Reference { get; set; }
        public string Representee { get; set; }
    }
    public class UpdateParticipantCommand : ICommand
    {
        public Guid ParticipantId { get; set; }
        public string Title { get; set; }
        public string DisplayName { get; set; }
        public string TelephoneNumber { get; set; }
        public string OrganisationName { get; set; }
        public Participant UpdatedParticipant { get; set; }
        public VideoHearing VideoHearing { get; set; }
        public RepresentativeInformation RepresentativeInformation { get; set; }

        public UpdateParticipantCommand(Guid participantId, string title, string displayName, string telphoneNumber, 
            string organisationName, VideoHearing videoHearing, RepresentativeInformation representativeInformation)
        {
            ParticipantId = participantId;
            Title = title;
            DisplayName = displayName;
            TelephoneNumber = telphoneNumber;
            OrganisationName = organisationName;
            VideoHearing = videoHearing;
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
            var participants = command.VideoHearing.GetParticipants().ToList();

            var participant = participants.FirstOrDefault(x => x.Id == command.ParticipantId);

            if (participant == null)
            {
                throw new ParticipantNotFoundException(command.ParticipantId);
            }

            participant.UpdateParticipantDetails(command.Title, command.DisplayName, command.TelephoneNumber, command.OrganisationName);

            if (participant.HearingRole.UserRole.IsRepresentative)
            {
                ((Representative)participant).UpdateRepresentativeDetails(
                    command.RepresentativeInformation.Reference,
                    command.RepresentativeInformation.Representee);
            }
            await _context.SaveChangesAsync();

            command.UpdatedParticipant = participant;
        }
    }
}