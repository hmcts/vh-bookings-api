using BookingsApi.DAL.Dtos;
using BookingsApi.DAL.Services;
using BookingsApi.Domain.Extensions;
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
            NewParticipants = [..newParticipants];
            RemovedParticipantIds = removedParticipantIds;
            LinkedParticipants = linkedParticipants ?? new List<LinkedParticipantDto>();
        }

        public List<ExistingParticipantDetails> ExistingParticipants { get; }
        public List<NewParticipant> NewParticipants { get; }
        public List<Guid> RemovedParticipantIds { get; }
        public Guid HearingId { get; set; }
        public List<LinkedParticipantDto> LinkedParticipants { get; }
    }
    
    public class UpdateHearingParticipantsCommandHandler(BookingsDbContext context, IHearingService hearingService)
        : ICommandHandler<UpdateHearingParticipantsCommand>
    {
        public async Task Handle(UpdateHearingParticipantsCommand command)
        {
            var hearing = await context.VideoHearings
                .Include(x=> x.CaseType)
                .Include(x => x.Participants).ThenInclude(x=> x.Person.Organisation)
                .Include(x => x.JudiciaryParticipants).ThenInclude(x=> x.JudiciaryPerson)
                .Include(x=> x.JudiciaryParticipants).ThenInclude(x=> x.InterpreterLanguage)
                .Include(x => x.Participants).ThenInclude(x => x.HearingRole.UserRole)
                .Include(x => x.Participants)
                .Include(x => x.Participants).ThenInclude(x => x.LinkedParticipants)
                .Include(x=> x.Participants).ThenInclude(x=> x.InterpreterLanguage)
                .Include(h => h.Endpoints).ThenInclude(x => x.DefenceAdvocate)
                .Include(x=> x.Endpoints).ThenInclude(x=> x.InterpreterLanguage)
                // keep the following includes for the screening entities - cannot auto include due to cyclic dependency
                .Include(x => x.Participants).ThenInclude(x => x.Screening).ThenInclude(x=> x.ScreeningEntities).ThenInclude(x=> x.Participant)
                .Include(x => x.Participants).ThenInclude(x => x.Screening).ThenInclude(x=> x.ScreeningEntities).ThenInclude(x=> x.Endpoint)
                .Include(x => x.Endpoints).ThenInclude(x => x.Screening).ThenInclude(x=> x.ScreeningEntities).ThenInclude(x=> x.Participant)
                .Include(x => x.Endpoints).ThenInclude(x => x.Screening).ThenInclude(x=> x.ScreeningEntities).ThenInclude(x=> x.Endpoint)
                .AsSplitQuery()
                .SingleOrDefaultAsync(x => x.Id == command.HearingId);
                        
            if (hearing == null)
            {
                throw new HearingNotFoundException(command.HearingId);
            }

            context.Update(hearing);

            foreach (var removedParticipantId in command.RemovedParticipantIds)
            {
                var participantBeingRemoved = hearing.GetParticipants().Single(x => x.Id == removedParticipantId);
                hearing.RemoveParticipant(participantBeingRemoved, false);
                // remove the participant from the screening options of the existing participants
                foreach (var existingParticipantScreening in command.ExistingParticipants
                             .Where(existingParticipantScreening => existingParticipantScreening.Screening.ProtectedFrom
                             .Exists(protectedFrom => string.Equals(protectedFrom, participantBeingRemoved.ExternalReferenceId))))
                    existingParticipantScreening.Screening.ProtectedFrom.Remove(participantBeingRemoved.ExternalReferenceId);
            }

            // only query languages required
            var languageCodesRequired = command.NewParticipants.Where(x => x.InterpreterLanguageCode != null)
                .Select(x => x.InterpreterLanguageCode).Distinct().ToList();
            command.ExistingParticipants.Where(x => x.InterpreterLanguageCode != null)
                .Select(x => x.InterpreterLanguageCode).Distinct().ToList().ForEach(x => languageCodesRequired.Add(x));

            var languages = await context.InterpreterLanguages.Where(x => x.Live && languageCodesRequired.Contains(x.Code)).ToListAsync();
           
            //add new participants
            await hearingService.AddParticipantToService(hearing, command.NewParticipants, languages);
            
            //process existing participants
            var participants = hearing.GetParticipants().ToList();
            foreach (var newExistingParticipantDetails in command.ExistingParticipants)
            {
                ProcessExistingParticipant(participants, newExistingParticipantDetails, languages);
            }
            
            hearing.UpdateBookingStatusJudgeRequirement();
            await hearingService.CreateParticipantLinks(participants, command.LinkedParticipants);
            await context.SaveChangesAsync();
            
            foreach(var existingParticipantScreening in command.ExistingParticipants)
            {
                var participant = participants.Single(x=> x.Id == existingParticipantScreening.ParticipantId);
                hearingService.UpdateParticipantScreeningRequirement(hearing, participant, existingParticipantScreening.Screening);
            }
            
            foreach(var participantForScreening in (command.NewParticipants).Where(x=> x.Screening != null))
            {
                var participant = participants.Single(x=> x.Person.ContactEmail == participantForScreening.Person.ContactEmail);
                var screeningDto = participantForScreening.Screening;
                hearingService.UpdateParticipantScreeningRequirement(hearing, participant, screeningDto);
            }
            
            await context.SaveChangesAsync();
        }

        private static void ProcessExistingParticipant(List<Participant> participants,
            ExistingParticipantDetails newExistingParticipantDetails, List<InterpreterLanguage> languages)
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

            var language = languages.GetLanguage(newExistingParticipantDetails.InterpreterLanguageCode);
            existingParticipant.UpdateLanguagePreferences(language, newExistingParticipantDetails.OtherLanguage);
            existingParticipant.ExternalReferenceId = newExistingParticipantDetails.ExternalReferenceId;
            existingParticipant.MeasuresExternalId = newExistingParticipantDetails.MeasuresExternalId;
        }
    }
}