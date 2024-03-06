using BookingsApi.Contract.V1.Requests;
using BookingsApi.Contract.V2.Requests;
using BookingsApi.Contract.V2.Requests.Enums;
using BookingsApi.Mappings.V1;
using BookingsApi.Mappings.V2;

namespace BookingsApi.Services
{
    public interface IUpdateHearingService
    {
        Task UpdateParticipantsV1(UpdateHearingParticipantsRequest request, VideoHearing hearing);
        Task UpdateParticipantsV2(UpdateHearingParticipantsRequestV2 request, VideoHearing hearing, List<HearingRole> hearingRoles);
        Task UpdateEndpointsV1(UpdateHearingEndpointsRequest request, VideoHearing hearing);
        Task UpdateEndpointsV2(UpdateHearingEndpointsRequestV2 request, VideoHearing hearing);
        Task UpdateJudiciaryParticipantsV2(UpdateJudiciaryParticipantsRequestV2 request, VideoHearing hearing);
    }
    
    public class UpdateHearingService : IUpdateHearingService
    {
        private readonly IHearingParticipantService _hearingParticipantService;
        private readonly IEndpointService _endpointService;
        private readonly IRandomGenerator _randomGenerator;
        private readonly KinlyConfiguration _kinlyConfiguration;
        private readonly IJudiciaryParticipantService _judiciaryParticipantService;

        public UpdateHearingService(IHearingParticipantService hearingParticipantService,
            IEndpointService endpointService,
            IRandomGenerator randomGenerator,
            IOptions<KinlyConfiguration> kinlyConfiguration,
            IJudiciaryParticipantService judiciaryParticipantService)
        {
            _hearingParticipantService = hearingParticipantService;
            _endpointService = endpointService;
            _randomGenerator = randomGenerator;
            _kinlyConfiguration = kinlyConfiguration.Value;
            _judiciaryParticipantService = judiciaryParticipantService;
        }

        public async Task UpdateParticipantsV1(UpdateHearingParticipantsRequest request, VideoHearing hearing)
        {
            await _hearingParticipantService.UpdateParticipants(request, hearing);
        }
        
        public async Task UpdateParticipantsV2(UpdateHearingParticipantsRequestV2 request, 
            VideoHearing hearing, List<HearingRole> hearingRoles)
        {
            await _hearingParticipantService.UpdateParticipantsV2(request, hearing, hearingRoles, false);
        }
        
        public async Task UpdateEndpointsV1(UpdateHearingEndpointsRequest request, VideoHearing hearing)
        {
            foreach (var endpointToAdd in request.NewEndpoints)
            {
                var newEp = EndpointToResponseMapper.MapRequestToNewEndpointDto(endpointToAdd, _randomGenerator,
                    _kinlyConfiguration.SipAddressStem);

                await _endpointService.AddEndpoint(hearing.Id, newEp);
            }

            foreach (var endpointToUpdate in request.ExistingEndpoints)
            {
                await _endpointService.UpdateEndpoint(hearing, endpointToUpdate.Id, endpointToUpdate.DefenceAdvocateContactEmail, endpointToUpdate.DisplayName);
            }

            foreach (var endpointIdToRemove in request.RemovedEndpointIds)
            {
                await _endpointService.RemoveEndpoint(hearing, endpointIdToRemove);
            }
        }
        
        public async Task UpdateEndpointsV2(UpdateHearingEndpointsRequestV2 request, VideoHearing hearing)
        {
            foreach (var endpointToAdd in request.NewEndpoints)
            {
                var newEp = EndpointToResponseV2Mapper.MapRequestToNewEndpointDto(endpointToAdd, _randomGenerator,
                    _kinlyConfiguration.SipAddressStem);

                await _endpointService.AddEndpoint(hearing.Id, newEp);
            }

            foreach (var endpointToUpdate in request.ExistingEndpoints)
            {
                await _endpointService.UpdateEndpoint(hearing, endpointToUpdate.Id, endpointToUpdate.DefenceAdvocateContactEmail, endpointToUpdate.DisplayName);
            }

            foreach (var endpointIdToRemove in request.RemovedEndpointIds)
            {
                await _endpointService.RemoveEndpoint(hearing, endpointIdToRemove);
            }
        }

        public async Task UpdateJudiciaryParticipantsV2(UpdateJudiciaryParticipantsRequestV2 request, VideoHearing hearing)
        {
            var oldJudge = hearing.GetJudiciaryParticipants().FirstOrDefault(jp => jp.HearingRoleCode == JudiciaryParticipantHearingRoleCode.Judge);
            
            var newJudge = request.NewJudiciaryParticipants.Find(jp => jp.HearingRoleCode == JudiciaryParticipantHearingRoleCodeV2.Judge);
            if (newJudge != null)
            {
                var newJudiciaryJudge = new NewJudiciaryJudge
                {
                    DisplayName = newJudge.DisplayName,
                    PersonalCode = newJudge.PersonalCode,
                    OptionalContactEmail = newJudge.ContactEmail,
                    OptionalContactTelephone = newJudge.ContactTelephone
                };
                await _judiciaryParticipantService.ReassignJudiciaryJudge(hearing.Id, newJudiciaryJudge, false);
            }

            var judiciaryParticipantsToAdd = request.NewJudiciaryParticipants
                // Filter out judges, as we reassign them above instead
                .Where(jp => jp.HearingRoleCode != JudiciaryParticipantHearingRoleCodeV2.Judge)
                .Select(JudiciaryParticipantRequestV2ToNewJudiciaryParticipantMapper.Map)
                .ToList();

            await _judiciaryParticipantService.AddJudiciaryParticipants(judiciaryParticipantsToAdd, hearing.Id, false);

            foreach (var existingJudiciaryParticipant in request.ExistingJudiciaryParticipants)
            {
                var judiciaryParticipantToUpdate = UpdateJudiciaryParticipantRequestV2ToUpdatedJudiciaryParticipantMapper.Map(
                    existingJudiciaryParticipant.PersonalCode, existingJudiciaryParticipant);

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
