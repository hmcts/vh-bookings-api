using BookingsApi.Contract.V1.Requests;
using BookingsApi.Contract.V2.Requests;
using BookingsApi.Contract.V2.Requests.Enums;
using BookingsApi.Domain.Dtos;
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
            await _hearingParticipantService.UpdateParticipants(request, hearing, sendNotification: false);
        }
        
        public async Task UpdateParticipantsV2(UpdateHearingParticipantsRequestV2 request, 
            VideoHearing hearing, List<HearingRole> hearingRoles)
        {
            await _hearingParticipantService.UpdateParticipantsV2(request, hearing, hearingRoles, sendNotification: false);
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
                await _endpointService.UpdateEndpoint(hearing, endpointToUpdate.Id,
                    [new(endpointToUpdate.DefenceAdvocateContactEmail, LinkedParticipantType.DefenceAdvocate)], 
                    null, null, endpointToUpdate.DisplayName);
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
                var newEp = EndpointToResponseV2Mapper.MapRequestToNewEndpointDto(endpointToAdd, _randomGenerator, _kinlyConfiguration.SipAddressStem);
                await _endpointService.AddEndpoint(hearing.Id, newEp);
            }

            foreach (var endpointToUpdate in request.ExistingEndpoints)
            {
                var endpointParticipants = endpointToUpdate.EndpointParticipants.Select(x => new NewEndpointParticipantDto(x.ContactEmail, (LinkedParticipantType)x.Type)).ToList();
                
                var (endpointParticipantsAdded, endpointParticipantsRemoved) 
                    = GetNewAndRemovedEndpointParticipants(hearing, endpointToUpdate.Id, endpointParticipants);

                await _endpointService.UpdateEndpoint(hearing, endpointToUpdate.Id, endpointParticipants, endpointParticipantsAdded, endpointParticipantsRemoved, endpointToUpdate.DisplayName);
            }

            foreach (var endpointIdToRemove in request.RemovedEndpointIds)
                await _endpointService.RemoveEndpoint(hearing, endpointIdToRemove);
        }

        private static (List<string> endpointParticipantsAdded, List<string> endpointParticipantsRemoved) 
            GetNewAndRemovedEndpointParticipants(VideoHearing hearing, Guid endpointId, List<NewEndpointParticipantDto> endpointParticipants)
        {
            var previousEndpointParticipants = hearing.GetEndpoints().Single(x => x.Id == endpointId)
                .GetLinkedParticipants()?.Select(x => x.Person.ContactEmail)
                .ToList() ?? [];

            //Where endpointParticipants not exist in previousListOfEndpointParticipants is added endpointParticipant
            var endpointParticipantsAdded = endpointParticipants
                .Where(x => !previousEndpointParticipants.Contains(x.ContactEmail))
                .Select(x => x.ContactEmail)
                .ToList();
            
            //Where previousListOfEndpointParticipants not exist in endpointParticipants is removed endpointParticipant
            var endpointParticipantsRemoved = previousEndpointParticipants
                .Where(x => endpointParticipants.TrueForAll(e => e.ContactEmail != x))
                .ToList();
            return (endpointParticipantsAdded, endpointParticipantsRemoved);
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
                await _judiciaryParticipantService.ReassignJudiciaryJudge(hearing.Id, newJudiciaryJudge, sendNotification: false);
            }

            var judiciaryParticipantsToAdd = request.NewJudiciaryParticipants
                // Filter out judges, as we reassign them above instead
                .Where(jp => jp.HearingRoleCode != JudiciaryParticipantHearingRoleCodeV2.Judge)
                .Select(JudiciaryParticipantRequestV2ToNewJudiciaryParticipantMapper.Map)
                .ToList();

            await _judiciaryParticipantService.AddJudiciaryParticipants(judiciaryParticipantsToAdd, hearing.Id, sendNotification: false);

            foreach (var judiciaryParticipant in request.ExistingJudiciaryParticipants)
            {
                var judiciaryParticipantToUpdate = UpdateJudiciaryParticipantRequestV2ToUpdatedJudiciaryParticipantMapper.Map(
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
