using BookingsApi.DAL.Dtos;
using BookingsApi.DAL.Services;
using BookingsApi.Domain.Participants;

namespace BookingsApi.DAL.Commands
{
    public class UpdateHearingParticipantsCommand : ICommand
    {
        public UpdateHearingParticipantsCommand(Guid hearingId, List<ExistingParticipantDetails> existingParticipants,
            List<NewParticipant> newParticipants, List<Guid> removedParticipantIds, List<LinkedParticipantDto> linkedParticipants)
        {
            HearingId = hearingId;
            ExistingParticipants = existingParticipants;
            NewParticipants = new List<NewParticipant>(newParticipants);
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
                .Include(x=> x.CaseType)
                .Include(x => x.Participants).ThenInclude(x=> x.Person.Organisation)
                .Include(x => x.JudiciaryParticipants).ThenInclude(x=> x.JudiciaryPerson)
                .Include(x => x.Participants).ThenInclude(x => x.HearingRole.UserRole)
                .Include(x => x.Participants).ThenInclude(x => x.CaseRole)
                .Include(x => x.Participants).ThenInclude(x => x.LinkedParticipants)
                .Include(h => h.Endpoints)
                    .ThenInclude(x => x.EndpointParticipants)
                    .ThenInclude(x => x.Participant)
                    .ThenInclude(x => x.Person)
                .SingleOrDefaultAsync(x => x.Id == command.HearingId);
                        
            if (hearing == null)
            {
                throw new HearingNotFoundException(command.HearingId);
            }

            _context.Update(hearing);

            foreach (var removedParticipantId in command.RemovedParticipantIds)
            {
                var participant = hearing.GetParticipants().Single(x => x.Id == removedParticipantId);
                
                if (participant.HearingRole.IsJudge() && command.NewParticipants.Exists(x => x.HearingRole.IsJudge()))
                {
                    var newJudgeParticipant = command.NewParticipants.Single(x => x.HearingRole.IsJudge());
                    await _hearingService.ReassignJudge(hearing, newJudgeParticipant);

                    command.NewParticipants.Remove(newJudgeParticipant);
                    continue;
                }

                var endpoint = hearing
                    .GetEndpoints()
                    .FirstOrDefault(x =>
                        x.EndpointParticipants.FirstOrDefault(ep => ep.ParticipantId == removedParticipantId) != null);

                if (endpoint != null)
                    endpoint.RemoveLinkedParticipant(hearing.GetParticipants().Single(x => x.Id == removedParticipantId));
                
                
                hearing.RemoveParticipantById(removedParticipantId, false);
            }
            
            await _hearingService.AddParticipantToService(hearing, command.NewParticipants);
            
            var participants = hearing.GetParticipants().ToList();
            foreach (var newExistingParticipantDetails in command.ExistingParticipants)
            {
                var existingParticipant = participants.Find(x => x.Id == newExistingParticipantDetails.ParticipantId);

                if (existingParticipant == null)
                {
                    throw new ParticipantNotFoundException(newExistingParticipantDetails.ParticipantId);
                }

                existingParticipant.LinkedParticipants.Clear();

                if (newExistingParticipantDetails.IsContactEmailNew)
                {
                    // new users must have their username set to contact email (this gets updated after creating the user)
                    existingParticipant.UpdateParticipantDetails(newExistingParticipantDetails.Title, newExistingParticipantDetails.DisplayName,
                        newExistingParticipantDetails.TelephoneNumber, newExistingParticipantDetails.OrganisationName, newExistingParticipantDetails.Person.ContactEmail);
                    existingParticipant.Person.UpdateUsername(newExistingParticipantDetails.Person.ContactEmail);
                }
                else
                {
                    existingParticipant.UpdateParticipantDetails(newExistingParticipantDetails.Title,
                        newExistingParticipantDetails.DisplayName,
                        newExistingParticipantDetails.TelephoneNumber, newExistingParticipantDetails.OrganisationName);
                }

                if (existingParticipant.HearingRole.UserRole.IsRepresentative)
                {
                   ((Representative)existingParticipant).UpdateRepresentativeDetails(
                        newExistingParticipantDetails.RepresentativeInformation.Representee);
                }
            }
            hearing.UpdateBookingStatusJudgeRequirement();
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
        public bool IsContactEmailNew { get; set; }
    }
}