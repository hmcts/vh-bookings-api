using BookingsApi.Contract.V2.Requests;
using BookingsApi.Mappings.V2;

namespace BookingsApi.Services
{
    public interface IUpdateHearingService
    {
        Task UpdateParticipantsV2(UpdateHearingParticipantsRequestV2 request, VideoHearing hearing, List<HearingRole> hearingRoles);
        Task UpdateEndpointsV2(UpdateHearingEndpointsRequestV2 request, VideoHearing hearing);
        Task UpdateJudiciaryParticipantsV2(UpdateJudiciaryParticipantsRequestV2 request, Guid hearingId);
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
        
        public async Task UpdateParticipantsV2(UpdateHearingParticipantsRequestV2 request, 
            VideoHearing hearing, List<HearingRole> hearingRoles)
        {
            await _hearingParticipantService.UpdateParticipantsV2(request, hearing, hearingRoles);
        }
        
        public async Task UpdateEndpointsV2(UpdateHearingEndpointsRequestV2 request, 
            VideoHearing hearing)
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

        public async Task UpdateJudiciaryParticipantsV2(UpdateJudiciaryParticipantsRequestV2 request, Guid hearingId)
        {
            // TODO if the request contains a different judge, remove them from the request and reassign them instead

            var judiciaryParticipantsToAdd = request.NewJudiciaryParticipants
                .Select(JudiciaryParticipantRequestV2ToNewJudiciaryParticipantMapper.Map)
                .ToList();

            await _judiciaryParticipantService.AddJudiciaryParticipants(judiciaryParticipantsToAdd, hearingId);

            foreach (var existingJudiciaryParticipant in request.ExistingJudiciaryParticipants)
            {
                var judiciaryParticipantToUpdate = UpdateJudiciaryParticipantRequestV2ToUpdatedJudiciaryParticipantMapper.Map(
                    existingJudiciaryParticipant.PersonalCode, existingJudiciaryParticipant);

                await _judiciaryParticipantService.UpdateJudiciaryParticipant(judiciaryParticipantToUpdate, hearingId);
            }

            foreach (var removedJudiciaryParticipant in request.RemovedJudiciaryParticipantPersonalCodes)
            {
                await _judiciaryParticipantService.RemoveJudiciaryParticipant(removedJudiciaryParticipant, hearingId);
            }
        }
    }
}
