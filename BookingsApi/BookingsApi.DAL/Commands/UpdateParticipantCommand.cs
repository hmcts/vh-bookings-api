using BookingsApi.Domain.Participants;
using BookingsApi.DAL.Dtos;
using BookingsApi.DAL.Services;

namespace BookingsApi.DAL.Commands
{
    public class RepresentativeInformation
    {
        public string Representee { get; set; }
    }

    public class AdditionalInformation
    {
        public string FirstName { get; private set; }
        public string MiddleNames { get; set; }
        public string LastName { get; private set; }
        public bool IsContactEmailNew { get; set; }

        public AdditionalInformation(string firstName, string lastName)
        {
            FirstName = firstName;
            LastName = lastName;
        }
    }
    
    public class UpdateParticipantCommand : ICommand
    {
        public Guid HearingId { get; }
        public Guid ParticipantId { get; }
        public string Title { get; }
        public string DisplayName { get; }
        public string TelephoneNumber { get; }
        public string OrganisationName { get; }
        public Participant UpdatedParticipant { get; set; }
        public RepresentativeInformation RepresentativeInformation { get; }
        public AdditionalInformation AdditionalInformation { get; }
        public string ContactEmail { get; set; }
        public List<LinkedParticipantDto> LinkedParticipants { get; }

        public UpdateParticipantCommand(Guid hearingId, Guid participantId, string title, string displayName, string telephoneNumber,
            string organisationName, RepresentativeInformation representativeInformation, List<LinkedParticipantDto> linkedParticipants,
            AdditionalInformation additionalInformation = null, string contactEmail = null)
        {
            HearingId = hearingId;
            ParticipantId = participantId;
            Title = title;
            DisplayName = displayName;
            TelephoneNumber = telephoneNumber;
            OrganisationName = organisationName;
            RepresentativeInformation = representativeInformation;
            AdditionalInformation = additionalInformation;
            ContactEmail = contactEmail;
            LinkedParticipants = linkedParticipants ?? new List<LinkedParticipantDto>();
        }
    }

    public class UpdateParticipantCommandHandler : ICommandHandler<UpdateParticipantCommand>
    {
        private readonly BookingsDbContext _context;
        private readonly IHearingService _hearingService;

        public UpdateParticipantCommandHandler(BookingsDbContext context, IHearingService hearingService)
        {
            _context = context;
            _hearingService = hearingService;
        }

        public async Task Handle(UpdateParticipantCommand command)
        {
            var hearing = await _context.VideoHearings
                .Include(x => x.Participants).ThenInclude(x => x.Person.Organisation)
                .Include(x => x.Participants).ThenInclude(x => x.HearingRole.UserRole)
                .Include(x => x.Participants).ThenInclude(x => x.CaseRole)
                .Include(x => x.Participants).ThenInclude(x => x.LinkedParticipants)
                .Include(x=> x.Participants).ThenInclude(x=> x.InterpreterLanguage)
                .SingleOrDefaultAsync(x => x.Id == command.HearingId);

            if (hearing == null)
            {
                throw new HearingNotFoundException(command.HearingId);
            }

            var participants = hearing.GetParticipants().ToList();

            var participant = participants.Find(x => x.Id == command.ParticipantId);

            if (participant == null)
            {
                throw new ParticipantNotFoundException(command.ParticipantId);
            }

            if (command.LinkedParticipants.Any() && participant.LinkedParticipants.Any())
            {
                await _hearingService.RemoveParticipantLinks(participants, participant);
            }
            await _hearingService.CreateParticipantLinks(participants, command.LinkedParticipants);

            if (!string.IsNullOrEmpty(command.ContactEmail))
            {
                var existingPerson = await _context.Persons.FirstOrDefaultAsync(x => x.ContactEmail.Trim() == command.ContactEmail.Trim());
                if (existingPerson != null && existingPerson.Id != participant.PersonId)
                {
                    participant.ChangePerson(existingPerson);
                }
            }

            if (command.AdditionalInformation?.IsContactEmailNew == true)
            {
                participant.UpdateParticipantDetails(command.Title, command.DisplayName, command.TelephoneNumber,
                    command.OrganisationName, command.ContactEmail);
                participant.Person.UpdateUsername(command.ContactEmail);
            }
            else
            {
                participant.UpdateParticipantDetails(command.Title, command.DisplayName, command.TelephoneNumber,
                    command.OrganisationName, command.ContactEmail);
            }

            if (command.AdditionalInformation != null)
            {
                participant.UpdateParticipantDetails(
                    command.AdditionalInformation.FirstName, 
                    command.AdditionalInformation.LastName, 
                    middleNames: command.AdditionalInformation.MiddleNames);
            }

            if (participant.HearingRole.UserRole.IsRepresentative)
            {
                ((Representative)participant).UpdateRepresentativeDetails(
                    command.RepresentativeInformation.Representee);
            }

            hearing.UpdateBookingStatusJudgeRequirement();
            await _context.SaveChangesAsync();

            command.UpdatedParticipant = participant;
        }
    }
}