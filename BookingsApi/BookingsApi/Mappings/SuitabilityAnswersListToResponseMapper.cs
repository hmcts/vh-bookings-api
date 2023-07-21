using BookingsApi.Domain.Participants;
using System.Collections.Generic;
using System.Linq;
using BookingsApi.Contract.V1.Responses;

namespace BookingsApi.Mappings
{
    public class SuitabilityAnswersListToResponseMapper
    {
        public List<ParticipantSuitabilityAnswerResponse> MapParticipantSuitabilityAnswerResponses(IEnumerable<Participant> participants)
        {
            var mapped = participants.Select(MapParticipantResponse);
            return mapped.OrderByDescending(x => x.UpdatedAt).ToList();
        }

        public ParticipantSuitabilityAnswerResponse MapParticipantResponse(Participant participant)
        {
            var @case = participant.Hearing.GetCases().FirstOrDefault();
            var @representative = participant as Representative;
            var answerMapper = new SuitabilityAnswerToResponseMapper();

            var response = new ParticipantSuitabilityAnswerResponse
            {
                CaseNumber = @case != null ? @case.Number : string.Empty,
                ParticipantId = participant.Id,
                Title = participant.Person.Title,
                FirstName = participant.Person.FirstName,
                LastName = participant.Person.LastName,
                HearingRole = participant.HearingRole.Name,
                UpdatedAt = participant.Questionnaire.UpdatedDate,
                Representee = @representative != null ? @representative.Representee : string.Empty,
                Answers = answerMapper.MapToResponses(participant.Questionnaire.SuitabilityAnswers)
            };

            return response;
        }
    }
}
