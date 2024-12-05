using BookingsApi.Contract.V2.Enums;
using BookingsApi.Contract.V2.Requests;
using BookingsApi.Mappings.V2;
using BookingsApi.Mappings.V2.Extensions;
using ContractJudiciaryParticipantHearingRoleCode = BookingsApi.Contract.V2.Enums.JudiciaryParticipantHearingRoleCode;
using DomainJudiciaryParticipantHearingRoleCode = BookingsApi.Domain.Enumerations.JudiciaryParticipantHearingRoleCode;

namespace BookingsApi.Services
{
    public interface IUpdateHearingService
    {
        /// <summary>
        /// Update participants for a hearing, including adding, updating, removing participants and linked participants
        /// </summary>
        /// <param name="request">Proposed changes</param>
        /// <param name="hearing">Hearing to update</param>
        /// <param name="hearingRoles">Available roles</param>
        /// <returns></returns>
        Task UpdateParticipantsV2(UpdateHearingParticipantsRequestV2 request, VideoHearing hearing, List<HearingRole> hearingRoles);
        
        /// <summary>
        /// Update endpoints for a hearing, including adding, updating, removing endpoints
        /// </summary>
        /// <param name="request">Proposed changes</param>
        /// <param name="hearing">Hearing to update</param>
        /// <returns></returns>
        Task UpdateEndpointsV2(UpdateHearingEndpointsRequestV2 request, VideoHearing hearing);
        
        /// <summary>
        /// Update judiciary participants for a hearing, including adding, updating, removing judiciary participants
        /// </summary>
        /// <param name="request">Proposed changes</param>
        /// <param name="hearing">Hearing to update</param>
        /// <returns></returns>
        Task UpdateJudiciaryParticipantsV2(UpdateJudiciaryParticipantsRequest request, VideoHearing hearing);
    }
    
    public class UpdateHearingService : IUpdateHearingService
    {
        private readonly IHearingParticipantService _hearingParticipantService;
        private readonly IEndpointService _endpointService;
        private readonly IRandomGenerator _randomGenerator;
        private readonly IJudiciaryParticipantService _judiciaryParticipantService;

        public UpdateHearingService(IHearingParticipantService hearingParticipantService,
            IEndpointService endpointService,
            IRandomGenerator randomGenerator,
            IOptions<SupplierConfiguration> supplierConfiguration,
            IJudiciaryParticipantService judiciaryParticipantService, IFeatureToggles featureToggles)
        {
            _hearingParticipantService = hearingParticipantService;
            _endpointService = endpointService;
            _randomGenerator = randomGenerator;
            _judiciaryParticipantService = judiciaryParticipantService;
        }

        /// <inheritdoc />
        public async Task UpdateParticipantsV2(UpdateHearingParticipantsRequestV2 request, 
            VideoHearing hearing, List<HearingRole> hearingRoles)
        {
            await _hearingParticipantService.UpdateParticipantsV2(request, hearing, hearingRoles, sendNotification: false);
        }

        /// <inheritdoc />
        public async Task UpdateEndpointsV2(UpdateHearingEndpointsRequestV2 request, VideoHearing hearing)
        {
            var sipAddressStem = _endpointService.GetSipAddressStem((BookingSupplier)hearing.ConferenceSupplier);
            foreach (var endpointToAdd in request.NewEndpoints)
            {
                var newEp = EndpointToResponseV2Mapper.MapRequestToNewEndpointDto(endpointToAdd, _randomGenerator,
                    sipAddressStem);

                await _endpointService.AddEndpoint(hearing.Id, newEp);
            }

            foreach (var endpointToUpdate in request.ExistingEndpoints)
            {
                await _endpointService.UpdateEndpoint(hearing, endpointToUpdate.Id,
                    endpointToUpdate.DefenceAdvocateContactEmail, endpointToUpdate.DisplayName,
                    endpointToUpdate.InterpreterLanguageCode, endpointToUpdate.OtherLanguage, endpointToUpdate.Screening?.MapToDalDto());
            }

            foreach (var endpointIdToRemove in request.RemovedEndpointIds)
            {
                await _endpointService.RemoveEndpoint(hearing, endpointIdToRemove);
            }
        }

        /// <inheritdoc />
        public async Task UpdateJudiciaryParticipantsV2(UpdateJudiciaryParticipantsRequest request, VideoHearing hearing)
        {
            var oldJudge = hearing.GetJudiciaryParticipants().FirstOrDefault(jp => jp.HearingRoleCode == DomainJudiciaryParticipantHearingRoleCode.Judge);
            
            var newJudge = request.NewJudiciaryParticipants.Find(jp => jp.HearingRoleCode == ContractJudiciaryParticipantHearingRoleCode.Judge);
            if (newJudge != null)
            {
                var newJudiciaryJudge = new NewJudiciaryJudge
                {
                    DisplayName = newJudge.DisplayName,
                    PersonalCode = newJudge.PersonalCode,
                    OptionalContactEmail = newJudge.ContactEmail,
                    OptionalContactTelephone = newJudge.ContactTelephone
                };
                await _judiciaryParticipantService.ReassignJudiciaryJudge(hearing.Id, newJudiciaryJudge, sendNotification: false);
            }

            var judiciaryParticipantsToAdd = request.NewJudiciaryParticipants
                // Filter out judges, as we reassign them above instead
                .Where(jp => jp.HearingRoleCode != ContractJudiciaryParticipantHearingRoleCode.Judge)
                .Select(JudiciaryParticipantRequestToNewJudiciaryParticipantMapper.Map)
                .ToList();

            await _judiciaryParticipantService.AddJudiciaryParticipants(judiciaryParticipantsToAdd, hearing.Id, sendNotification: false);

            foreach (var judiciaryParticipant in request.ExistingJudiciaryParticipants)
            {
                var judiciaryParticipantToUpdate = UpdateJudiciaryParticipantRequestToUpdatedJudiciaryParticipantMapper.Map(
                    judiciaryParticipant.PersonalCode, judiciaryParticipant);
                
                var originalJudiciaryParticipant = hearing.JudiciaryParticipants.SingleOrDefault(x => x.JudiciaryPerson.PersonalCode == judiciaryParticipant.PersonalCode);
                if (originalJudiciaryParticipant == null)
                {
                    // For consistency with the update participants functionality, non-existing judiciary participants are skipped in the update
                    continue;
                }

                await _judiciaryParticipantService.UpdateJudiciaryParticipant(judiciaryParticipantToUpdate, hearing.Id);
            }

            var judiciaryParticipantPersonalCodesToRemove = request.RemovedJudiciaryParticipantPersonalCodes;
            if (newJudge != null)
            {
                // Filter out judges, as we reassign them above instead
                judiciaryParticipantPersonalCodesToRemove = judiciaryParticipantPersonalCodesToRemove
                    .Where(c => c != oldJudge?.JudiciaryPerson.PersonalCode)
                    .ToList();
            }
            
            foreach (var removedJudiciaryParticipant in judiciaryParticipantPersonalCodesToRemove)
            {
                await _judiciaryParticipantService.RemoveJudiciaryParticipant(removedJudiciaryParticipant, hearing.Id);
            }
        }
    }
}
