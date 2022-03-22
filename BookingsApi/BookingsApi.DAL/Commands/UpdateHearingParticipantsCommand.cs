using BookingsApi.DAL.Commands.Core;
using BookingsApi.DAL.Dtos;
using BookingsApi.DAL.Exceptions;
using BookingsApi.DAL.Services;
using BookingsApi.Domain;
using BookingsApi.Domain.Participants;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookingsApi.DAL.Commands
{
    public class UpdateHearingParticipantsCommand : ICommand
    {
        public UpdateHearingParticipantsCommand(Guid hearingId, List<ExistingParticipantDetails> existingParticipants,
            List<NewParticipant> newParticipants, List<Guid> removedParticipantIds, List<LinkedParticipantDto> linkedParticipants)
        {
            HearingId = hearingId;
            ExistingParticipants = existingParticipants;
            NewParticipants = newParticipants;
            RemovedParticipantIds = removedParticipantIds;
            LinkedParticipants = linkedParticipants ?? new List<LinkedParticipantDto>();
        }

        public List<ExistingParticipantDetails> ExistingParticipants { get; set; }
        public List<NewParticipant> NewParticipants { get; set; }
        public List<Guid> RemovedParticipantIds { get; set; }
        public Guid HearingId { get; set; }
        public List<LinkedParticipantDto> LinkedParticipants { get; set; }
    }
    
    public class UpdateHearingParticipantsCommandHandler : ICommandHandler<UpdateHearingParticipantsCommand>
    {
        private readonly BookingsDbContext _context;
        private readonly IHearingService _hearingService;

        public UpdateHearingParticipantsCommandHandler(BookingsDbContext context, IHearingService hearingService)
        {
            _context = context;
            _hearingService = hearingService;
        }

        public async Task Handle(UpdateHearingParticipantsCommand command)
        {
            var hearing = await _context.VideoHearings
                .Include(x => x.Participants).ThenInclude(x=> x.Person.Organisation)
                .Include(x => x.Participants).ThenInclude(x => x.HearingRole.UserRole)
                .Include(x => x.Participants).ThenInclude(x => x.CaseRole)
                .Include(x => x.Participants).ThenInclude(x => x.LinkedParticipants)
                .Include(h => h.Endpoints).ThenInclude(x => x.DefenceAdvocate)
                .SingleOrDefaultAsync(x => x.Id == command.HearingId);
                        
            if (hearing == null)
            {
                throw new HearingNotFoundException(command.HearingId);
            }

            _context.Update(hearing);
            
            foreach (var removedParticipantId in command.RemovedParticipantIds)
                hearing.RemoveParticipantById(removedParticipantId, false);
            
            await _hearingService.AddParticipantToService(hearing, command.NewParticipants);

            _hearingService.ValidateHostCount(hearing.Participants);
            
            var participants = hearing.GetParticipants().ToList();
            foreach (var newExistingParticipantDetails in command.ExistingParticipants)
            {
                var existingParticipant = participants.FirstOrDefault(x => x.Id == newExistingParticipantDetails.ParticipantId);

                if (existingParticipant == null)
                {
                    throw new ParticipantNotFoundException(newExistingParticipantDetails.ParticipantId);
                }

                existingParticipant.LinkedParticipants.Clear();

                existingParticipant.UpdateParticipantDetails(newExistingParticipantDetails.Title, newExistingParticipantDetails.DisplayName,
                    newExistingParticipantDetails.TelephoneNumber, newExistingParticipantDetails.OrganisationName);

                if (existingParticipant.HearingRole.UserRole.IsRepresentative)
                {
                   ((Representative)existingParticipant).UpdateRepresentativeDetails(
                        newExistingParticipantDetails.RepresentativeInformation.Representee);
                }
            }
            
            await _hearingService.CreateParticipantLinks(participants, command.LinkedParticipants);
            
            await _context.SaveChangesAsync();
        }
    }

    public class ExistingParticipantDetails
    {
        public Guid ParticipantId { get; set; }
        public string Title { get; set; }
        public string DisplayName { get; set; }
        public string TelephoneNumber { get; set; }
        public string CaseRoleName { get; set; }
        public string HearingRoleName { get; set; }
        public string OrganisationName { get; set; }
        public Person Person { get; set; }
        public RepresentativeInformation RepresentativeInformation { get; set; }
    }
}