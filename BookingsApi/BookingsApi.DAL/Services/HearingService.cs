using BookingsApi.DAL.Dtos;
using BookingsApi.Domain.Extensions;
using BookingsApi.Domain.Participants;
using BookingsApi.Domain.Validations;

namespace BookingsApi.DAL.Services;

public interface IHearingService
{
    /// <summary>
    /// Add a participant to a hearing service and returns a list of all participants.
    /// This will re-use existing personnel entries before attempting to create a new one.
    /// </summary>
    /// <param name="hearing">Hearing to amend</param>
    /// <param name="participants">List of participants to add</param>
    /// <param name="languages">Language spoken by participant</param>
    /// <returns></returns>
    Task<List<Participant>> AddParticipantToService(VideoHearing hearing, List<NewParticipant> participants, List<InterpreterLanguage> languages);

    /// <summary>
    /// Update the case name of a hearing directly
    /// </summary>
    /// <param name="hearingId">Id of hearing</param>
    /// <param name="caseName">New case name</param>
    /// <returns></returns>
    Task UpdateHearingCaseName(Guid hearingId, string caseName);

    /// <summary>
    /// Update the case name of a multi-day hearing, used purely as part of the clone process
    /// </summary>
    /// <param name="hearingId">Id of the hearing</param>
    /// <param name="newCaseName">New case name</param>
    /// <returns></returns>
    Task RenameHearingForMultiDayBooking(Guid hearingId, string newCaseName);

    /// <summary>
    /// Create links between participants if any exist
    /// </summary>
    /// <param name="participants">List of participants in the hearing</param>
    /// <param name="linkedParticipantDtos">List linked participants dtos containing the linking data</param>
    /// <returns></returns>
    Task CreateParticipantLinks(List<Participant> participants, List<LinkedParticipantDto> linkedParticipantDtos);

    /// <summary>
    /// Removes the link between participants
    /// </summary>
    /// <param name="participants">All participants in a hearing</param>
    /// <param name="participant">The participants with whom to remove links</param>
    /// <returns></returns>
    Task RemoveParticipantLinks(List<Participant> participants, Participant participant);

    Task AddJudiciaryParticipantToVideoHearing(VideoHearing videoHearing, NewJudiciaryParticipant participant, List<InterpreterLanguage> languages);

    Task ReassignJudge(VideoHearing hearing, NewParticipant newJudgeParticipant);

    void UpdateParticipantScreeningRequirement(VideoHearing hearing, Participant participant,
        ScreeningDto screeningDto);

    void UpdateEndpointScreeningRequirement(VideoHearing hearing, Endpoint endpoint, ScreeningDto screeningDto);
}

//TODO: Remove all CaseRole Assignments as part of https://tools.hmcts.net/jira/browse/VIH-10899
public class HearingService(BookingsDbContext context) : IHearingService
{
    public async Task<List<Participant>> AddParticipantToService(VideoHearing hearing, List<NewParticipant> participants, List<InterpreterLanguage> languages)
    {
        var participantList = new List<Participant>();
        foreach (var participantToAdd in participants)
        {
            var existingPerson = await UpdateExistingParticipantPerson(participantToAdd);

            var language = languages.GetLanguage(participantToAdd.InterpreterLanguageCode, "Participant");

            switch (participantToAdd.HearingRole.UserRole.Name)
            {
                case "Individual":
                    var individual = hearing.AddIndividual(participantToAdd.ExternalReferenceId, existingPerson ?? participantToAdd.Person, participantToAdd.HearingRole, participantToAdd.DisplayName);
                    individual.UpdateLanguagePreferences(language, participantToAdd.OtherLanguage);
                    individual.MeasuresExternalId = participantToAdd.MeasuresExternalId;
                    UpdateOrganisationDetails(participantToAdd.Person, individual);
                    participantList.Add(individual);
                    break;
                case "Representative":
                {
                    var representative = hearing.AddRepresentative(participantToAdd.ExternalReferenceId, existingPerson ?? participantToAdd.Person,
                        participantToAdd.HearingRole, participantToAdd.DisplayName, participantToAdd.Representee);
                    representative.UpdateLanguagePreferences(language, participantToAdd.OtherLanguage);
                    representative.MeasuresExternalId = participantToAdd.MeasuresExternalId;
                    UpdateOrganisationDetails(participantToAdd.Person, representative);
                    participantList.Add(representative);
                    break;
                }
                case "Judicial Office Holder":
                {
                    var joh = hearing.AddJudicialOfficeHolder(existingPerson ?? participantToAdd.Person,
                        participantToAdd.HearingRole, participantToAdd.CaseRole, participantToAdd.DisplayName);
                    joh.UpdateLanguagePreferences(language, participantToAdd.OtherLanguage);
                    participantList.Add(joh);
                    break;
                }
                case "Judge":
                {
                    var judge = hearing.AddJudge(existingPerson ?? participantToAdd.Person, 
                        participantToAdd.HearingRole, 
                        participantToAdd.CaseRole, 
                        participantToAdd.DisplayName);
                    judge.UpdateLanguagePreferences(language, participantToAdd.OtherLanguage);
                    participantList.Add(judge);
                    break;
                }
                default:
                    throw new DomainRuleException(nameof(participantToAdd.HearingRole.UserRole.Name),
                        $"Role {participantToAdd.HearingRole.UserRole.Name} not recognised");
            }
        }
        await LoadHearingRoles(participantList);
        return participantList;
    }

    private async Task<Person> UpdateExistingParticipantPerson(NewParticipant participantToAdd)
    {
        Person existingPerson = null;
        if (participantToAdd.Person.ContactEmail != null)
        {
            existingPerson = await context.Persons
                .Include("Organisation")
                .SingleOrDefaultAsync(x => x.ContactEmail == participantToAdd.Person.ContactEmail);
        }

        if(existingPerson != null)
        {
            var person = participantToAdd.Person;
            existingPerson.UpdatePerson(person.FirstName, person.LastName, person.Title, person.TelephoneNumber);
        }

        return existingPerson;
    }

    public async Task ReassignJudge(VideoHearing hearing, NewParticipant newJudgeParticipant)
    {
        var person = await context.Persons.FirstAsync(x => x.ContactEmail == newJudgeParticipant.Person.ContactEmail);
        var judge = new Judge(person, newJudgeParticipant.HearingRole, newJudgeParticipant.CaseRole)
        {
            DisplayName = newJudgeParticipant.DisplayName
        };
        hearing.ReassignJudge(judge);
    }

    public void UpdateParticipantScreeningRequirement(VideoHearing hearing, Participant participant, ScreeningDto screeningDto)
    {
        if(participant.Screening == null && screeningDto == null) return;
        if (participant.Screening != null && screeningDto == null)
        {
            context.Entry(participant.Screening).State = EntityState.Deleted;
            participant.RemoveScreening();
            return;
        }
            
        var screeningExists = participant.Screening != null;
        hearing.AssignScreeningForParticipant(participant, screeningDto.ScreeningType, screeningDto.ProtectedFrom);
        // ef core does not automatically track entities created by domain methods
        context.Entry(participant.Screening).State = screeningExists? EntityState.Modified : EntityState.Added;
        foreach (var screeningEntity in participant.Screening.ScreeningEntities)
        {
            context.Entry(screeningEntity).State = EntityState.Added;
        }
    }

    public void UpdateEndpointScreeningRequirement(VideoHearing hearing, Endpoint endpoint,
        ScreeningDto screeningDto)
    {
        if(endpoint.Screening == null && screeningDto == null) return;
        if (endpoint.Screening != null && screeningDto == null)
        {
            context.Entry(endpoint.Screening).State = EntityState.Deleted;
            endpoint.RemoveScreening();
            return;
        }
            
        var screeningExists = endpoint.Screening != null;
        hearing.AssignScreeningForEndpoint(endpoint, screeningDto.ScreeningType, screeningDto.ProtectedFrom);
        // ef core does not automatically track entities created by domain methods
        context.Entry(endpoint.Screening).State = screeningExists? EntityState.Modified : EntityState.Added;
        foreach (var screeningEntity in endpoint.Screening.ScreeningEntities)
        {
            context.Entry(screeningEntity).State = EntityState.Added;
        }
    }

    private async Task LoadHearingRoles(List<Participant> participantList)
    {
        foreach (var participant in participantList)
        {
            participant.HearingRole = await context.HearingRoles
                .Include(h => h.UserRole)
                .FirstOrDefaultAsync(x => x.Id == participant.HearingRoleId);
        }
    }
        
    public async Task UpdateHearingCaseName(Guid hearingId, string caseName)
    {
        var hearing = await context.VideoHearings.Include(x => x.HearingCases).ThenInclude(h => h.Case)
            .FirstAsync(x => x.Id == hearingId);
        var existingCase = hearing.GetCases()[0];
        hearing.UpdateCase(new Case(existingCase.Number, caseName)
        {
            IsLeadCase = existingCase.IsLeadCase
        });
        await context.SaveChangesAsync();
    }

    public async Task RenameHearingForMultiDayBooking(Guid hearingId, string newCaseName)
    {
        var hearing = await context.VideoHearings.Include(x => x.HearingCases).ThenInclude(h => h.Case)
            .FirstAsync(x => x.Id == hearingId);
        hearing.RenameHearingForMultiDayBooking(newCaseName);
        await context.SaveChangesAsync();
    }

    public Task CreateParticipantLinks(List<Participant> participants, List<LinkedParticipantDto> linkedParticipantDtos)
    {
        var linkedParticipants = new List<LinkedParticipant>();

        foreach (var linkedParticipantDto in linkedParticipantDtos)
        {
            var interpretee =
                participants.Single(x =>
                    x.Person.ContactEmail.Equals(linkedParticipantDto.ParticipantContactEmail,
                        StringComparison.CurrentCultureIgnoreCase));
            var interpreter =
                participants.Single(x =>
                    x.Person.ContactEmail.Equals(linkedParticipantDto.LinkedParticipantContactEmail,
                        StringComparison.CurrentCultureIgnoreCase));

            var linkedParticipant = new LinkedParticipant(interpretee.Id, interpreter.Id,
                linkedParticipantDto.Type);

            linkedParticipants.Add(linkedParticipant);

            UpdateParticipantsWithLinks(interpretee, interpreter, linkedParticipantDto.Type);
        }

        return Task.FromResult(linkedParticipants);
    }

    public Task RemoveParticipantLinks(List<Participant> participants, Participant participant)
    {
        var linkedParticipants = participant.LinkedParticipants.Select(l => new LinkedParticipant(l.ParticipantId, l.LinkedId, l.Type)).ToList();
        foreach (var lp in linkedParticipants)
        {
            var interpreter = participants.Single(x => x.Id == lp.ParticipantId);
            var interpretee = participants.Single(x => x.Id == lp.LinkedId);

            var lp1 = interpreter.LinkedParticipants.Single(x => x.LinkedId == interpretee.Id);
            var lp2 = interpretee.LinkedParticipants.Single(x => x.LinkedId == interpreter.Id);

            interpretee.RemoveLink(lp2);
            interpreter.RemoveLink(lp1);
        }
        return Task.CompletedTask;
    }

    public async Task AddJudiciaryParticipantToVideoHearing(VideoHearing videoHearing, NewJudiciaryParticipant participant, List<InterpreterLanguage> languages)
    {
        var judiciaryPerson = await context.JudiciaryPersons
            .SingleOrDefaultAsync(x => x.PersonalCode == participant.PersonalCode);

        if (judiciaryPerson == null)
            throw new JudiciaryPersonNotFoundException(participant.PersonalCode);

        var interpreterLanguage = languages.GetLanguage(participant.InterpreterLanguageCode, "JudiciaryParticipant");
        var otherLanguage = participant.OtherLanguage;
        switch (participant.HearingRoleCode)
        {
            case JudiciaryParticipantHearingRoleCode.Judge:
                videoHearing.AddJudiciaryJudge(judiciaryPerson, participant.DisplayName, participant.OptionalContactEmail, participant.OptionalContactTelephone, interpreterLanguage, otherLanguage);
                break;
            case JudiciaryParticipantHearingRoleCode.PanelMember:
                videoHearing.AddJudiciaryPanelMember(judiciaryPerson, participant.DisplayName, otherLanguage:otherLanguage, interpreterLanguage:interpreterLanguage);
                break;
            default:
                throw new ArgumentException($"Role {participant.HearingRoleCode} not recognised");
        }
    }

    private static void UpdateParticipantsWithLinks(Participant participant1, Participant participant2, LinkedParticipantType linkType)
    {
        participant1.AddLink(participant2.Id, linkType);
        participant2.AddLink(participant1.Id, linkType);
    }

    private static void UpdateOrganisationDetails(Person newPersonDetails, Participant participantToUpdate)
    {
        var newOrganisation = newPersonDetails.Organisation;
        var existingPerson = participantToUpdate.Person;
        participantToUpdate.UpdateParticipantDetails(existingPerson.Title, participantToUpdate.DisplayName, newPersonDetails.TelephoneNumber, newOrganisation?.Name);
    }
}