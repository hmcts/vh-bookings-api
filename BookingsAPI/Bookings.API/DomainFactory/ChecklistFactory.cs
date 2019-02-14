using System;
using System.Linq;
using Bookings.DAL;
using Bookings.DAL.Queries;
using Bookings.Domain;
using Bookings.Domain.Participants;

namespace Bookings.API.DomainFactory
{
    public class ChecklistFactory : IChecklistFactory
    {
        private readonly IQueryHandler _queryHandler;

        public ChecklistFactory(IQueryHandler queryHandler)
        {
            _queryHandler = queryHandler;
        }

        public Checklist GetForParticipant(Participant participant)
        {
            throw new NotImplementedException();
//            var questions =
//                _repository.GetAllWhere<ChecklistQuestion, int>(q => q.Role.ToString() == participant.HearingRole.Name);
//            return Checklist.Existing(participant, questions);
        }

        public IQueryable<Participant> GetAllParticipantsWithChecklist()
        {
            throw new NotImplementedException();
            // Since this query is using the Max function it get's evaluated client side and not in sql, this can be improved
//            return _repository.GetAll<Participant, Guid>()
//                .Where(x => x.ChecklistAnswers.Any())
//                .OrderByDescending(x => x.ChecklistAnswers.Max(c => c.CreatedAt));
        }
    }
}
