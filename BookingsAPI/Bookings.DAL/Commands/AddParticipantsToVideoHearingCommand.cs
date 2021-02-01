using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Bookings.DAL.Commands.Core;
using Bookings.DAL.Dtos;
using Bookings.DAL.Exceptions;
using Bookings.Domain;
using Bookings.Domain.RefData;
using Microsoft.EntityFrameworkCore;

namespace Bookings.DAL.Commands
{

    public class NewParticipant
    {
        public Person Person { get; set; }
        public CaseRole CaseRole { get; set; }
        public HearingRole HearingRole { get; set; }
        public string Representee { get; set; }
        public string DisplayName { get; set; }
    }
    
    public class AddParticipantsToVideoHearingCommand : ICommand
    {
        public AddParticipantsToVideoHearingCommand(Guid hearingId, List<NewParticipant> participants, List<LinkedParticipantDto> linkedParticipants)
        {
            HearingId = hearingId;
            Participants = participants;
            LinkedParticipants = linkedParticipants ?? new List<LinkedParticipantDto>();
        }

        public List<NewParticipant> Participants { get; set; }
        public Guid HearingId { get; set; }
        public List<LinkedParticipantDto> LinkedParticipants { get; set; }
    }
    
    public class AddParticipantsToVideoHearingCommandHandler : ICommandHandler<AddParticipantsToVideoHearingCommand>
    {
        private readonly BookingsDbContext _context;
        private readonly IHearingService _hearingService;

        public AddParticipantsToVideoHearingCommandHandler(BookingsDbContext context, IHearingService hearingService)
        {
            _context = context;
            _hearingService = hearingService;
        }

        public async Task Handle(AddParticipantsToVideoHearingCommand command)
        {
            var hearing = await _context.VideoHearings
                .Include(x => x.Participants).ThenInclude(x=> x.Person.Organisation)
                .Include(x => x.Participants).ThenInclude(x => x.HearingRole.UserRole)
                .Include(x => x.Participants).ThenInclude(x => x.CaseRole)
                .Include(x => x.Participants).ThenInclude(x => x.LinkedParticipants)
                .SingleOrDefaultAsync(x => x.Id == command.HearingId);
            
            if (hearing == null)
            {
                throw new HearingNotFoundException(command.HearingId);
            }

            _context.Update(hearing);
            
            var participants = await _hearingService.AddParticipantToService(hearing, command.Participants);

            var participantLinks = await _hearingService.CreateParticipantLinks(participants, command.LinkedParticipants);
            
            foreach (var participantLink in participantLinks)
            {
                var interpreteeLink = new LinkedParticipant(participantLink.LinkedId, 
                    participantLink.ParticipantId, participantLink.Type);
                
                await _context.LinkedParticipant.AddRangeAsync(participantLink, interpreteeLink);
            }
            
            await _context.SaveChangesAsync();
        }
    }
}