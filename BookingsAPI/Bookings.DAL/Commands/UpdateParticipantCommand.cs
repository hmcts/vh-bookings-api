using System;
using System.Threading.Tasks;
using Bookings.DAL.Commands.Core;
using Bookings.Domain.Participants;
using Microsoft.EntityFrameworkCore;
using Bookings.DAL.Exceptions;
using System.Linq;

namespace Bookings.DAL.Commands
{
    public class UpdateParticipantCommand : ICommand
    {
        public Guid HearingId { get; set; }
        public Guid ParticipantId { get; set; }
        public string Title { get; set; }
        public string DisplayName { get; set; }
        public string TelephoneNumber { get; set; }
        public string Street { get; set; }
        public string HouseNumber { get; set; }
        public string City { get; set; }
        public string County { get; set; }
        public string Postcode { get; set; }
        public string OrganisationName { get; set; }
        public Participant UpdatedParticipant { get; set; }

        public UpdateParticipantCommand(Guid hearingId, Guid participantId, string title, string displayName, string telphoneNumber, string street, string houseNumber, string city, string county, string postcode, string organisationName)
        {
            HearingId = hearingId;
            ParticipantId = participantId;
            Title = title;
            DisplayName = displayName;
            TelephoneNumber = telphoneNumber;
            Street = street;
            HouseNumber = houseNumber;
            City = city;
            County = county;
            Postcode = postcode;
            OrganisationName = organisationName;
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
            //var hearing = await _context.VideoHearings.FindAsync(command.HearingId);

            var hearing = await _context.VideoHearings
              .Include("Participants.Person")
              .Include("Participants.Person.Address")
              .Include("Participants.Person.Organisation")
              .Include("HearingCases.Case")
              .Include(x => x.CaseType)
              .ThenInclude(x => x.CaseRoles)
              .ThenInclude(x => x.HearingRoles)
              .ThenInclude(x => x.UserRole)
              .SingleOrDefaultAsync(x => x.Id == command.HearingId);

            if (hearing == null)
            {
                throw new HearingNotFoundException(command.HearingId);
            }

            var participants = hearing.GetParticipants().ToList();

            var participant = participants.FirstOrDefault(x => x.Id == command.ParticipantId);

            if (participant == null)
            {
                throw new ParticipantNotFoundException(command.HearingId);
            }

            participant.UpdateParticipantDetails(command.Title, command.DisplayName, command.TelephoneNumber, command.Street, command.HouseNumber, command.City, command.County, command.Postcode, command.OrganisationName);

            await _context.SaveChangesAsync();

            command.UpdatedParticipant = participant;

           
        }
    }
}