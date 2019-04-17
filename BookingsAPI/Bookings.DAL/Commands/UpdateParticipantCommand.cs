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
    public class NewAddress
    {
        public string Street { get; set; }
        public string HouseNumber { get; set; }
        public string City { get; set; }
        public string County { get; set; }
        public string Postcode { get; set; }
    }
    public class RepresentativeInformation
    {
        public string SolicitorsReference { get; set; }
        public string Representee { get; set; }
    }
    public class UpdateParticipantCommand : ICommand
    {
        public Guid ParticipantId { get; set; }
        public string Title { get; set; }
        public string DisplayName { get; set; }
        public string TelephoneNumber { get; set; }
        public NewAddress NewAddress { get; set; }
        public string OrganisationName { get; set; }
        public Participant UpdatedParticipant { get; set; }
        public VideoHearing VideoHearing { get; set; }
        public RepresentativeInformation RepresentativeInformation { get; set; }

        public UpdateParticipantCommand(Guid participantId, string title, string displayName, string telphoneNumber, 
            NewAddress address, string organisationName, VideoHearing videoHearing, RepresentativeInformation representativeInformation)
        {
            ParticipantId = participantId;
            Title = title;
            DisplayName = displayName;
            TelephoneNumber = telphoneNumber;
            OrganisationName = organisationName;
            NewAddress = address;
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

            participant.UpdateParticipantDetails(command.Title, command.DisplayName, command.TelephoneNumber, command.NewAddress.Street, command.NewAddress.HouseNumber, command.NewAddress.City, command.NewAddress.County, command.NewAddress.Postcode, command.OrganisationName);

            if (participant.HearingRole.UserRole.IsRepresentative)
            {
                ((Representative)participant).UpdateRepresentativeDetails(
                    command.RepresentativeInformation.SolicitorsReference,
                    command.RepresentativeInformation.Representee);
            }
            await _context.SaveChangesAsync();

            command.UpdatedParticipant = participant;
        }
    }
}